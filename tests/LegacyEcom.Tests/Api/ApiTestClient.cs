using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LegacyEcom.Tests.Api;

/// <summary>
/// Minimal cookie-aware HTTP client for API tests. Manages the cookie jar
/// manually (session, auth, antiforgery) and fetches a fresh XSRF token before
/// each state-changing request — mirroring the documented browser flow
/// (tokens are bound to the authenticated identity).
/// </summary>
public class ApiTestClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _cookies = new();

    public ApiTestClient(HttpClient client) => _client = client;

    public IReadOnlyDictionary<string, string> Cookies => _cookies;

    private void StoreCookies(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var setCookies)) return;
        foreach (var header in setCookies)
        {
            var pair = header.Split(';', 2)[0].Split('=', 2);
            if (pair.Length == 2)
                _cookies[pair[0].Trim()] = pair[1].Trim();
        }
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path, object? body, bool withXsrf)
    {
        var request = new HttpRequestMessage(method, path);
        if (_cookies.Count > 0)
            request.Headers.Add("Cookie", string.Join("; ", _cookies.Select(kv => $"{kv.Key}={kv.Value}")));
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);
        if (withXsrf)
        {
            if (!_cookies.TryGetValue("XSRF-TOKEN", out var token))
                throw new InvalidOperationException("No XSRF-TOKEN cookie. Call GetXsrfTokenAsync first.");
            request.Headers.Add("X-XSRF-TOKEN", token);
        }
        return request;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object? body = null, bool withXsrf = false)
    {
        var response = await _client.SendAsync(BuildRequest(method, path, body, withXsrf));
        StoreCookies(response);
        return response;
    }

    /// <summary>GET /api/auth/xsrf-token — refreshes the antiforgery token pair.</summary>
    public async Task GetXsrfTokenAsync()
    {
        using var response = await SendAsync(HttpMethod.Get, "/api/auth/xsrf-token");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(_cookies.ContainsKey("XSRF-TOKEN"));
    }

    public Task<HttpResponseMessage> GetAsync(string path) => SendAsync(HttpMethod.Get, path);

    public async Task<HttpResponseMessage> PostAsync(string path, object? body = null)
    {
        await GetXsrfTokenAsync(); // fresh token: bound to current identity
        return await SendAsync(HttpMethod.Post, path, body, withXsrf: true);
    }

    public async Task<HttpResponseMessage> PutAsync(string path, object? body = null)
    {
        await GetXsrfTokenAsync();
        return await SendAsync(HttpMethod.Put, path, body, withXsrf: true);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string path)
    {
        await GetXsrfTokenAsync();
        return await SendAsync(HttpMethod.Delete, path, withXsrf: true);
    }

    public static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
}
