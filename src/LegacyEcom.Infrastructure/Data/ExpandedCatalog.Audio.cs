namespace LegacyEcom.Infrastructure.Data;

/// <summary>Audio category expansion (~28 products).</summary>
internal static class ExpandedCatalogAudio
{
    internal static void Add(List<ExpandedCatalog.ProductDef> list)
    {
        // Bluetooth Speaker (7)
        list.Add(P("AUD-SPK-211", "Thunder Bluetooth Speaker", "thunder-bluetooth-speaker",
            "360-degree sound portable speaker.",
            "Thunder portable Bluetooth speaker with 360-degree sound, 20-hour battery, and IPX7 waterproofing.",
            89.99m, 79.99m, "audio", "speaker-1.jpg", 71, true));
        list.Add(P("AUD-SPK-212", "Mini Cube Speaker", "mini-cube-speaker",
            "Pocket-size speaker with big sound.",
            "Mini Cube pocket-size Bluetooth speaker with surprisingly big sound and 8-hour battery.",
            29.99m, null, "audio", "speaker-1.jpg", 105));
        list.Add(P("AUD-SPK-213", "Party Speaker with Lights", "party-speaker-with-lights",
            "100W party speaker with LED show.",
            "100W party Bluetooth speaker with LED light show, mic input, and 15-hour battery.",
            149.99m, null, "audio", "speaker-1.jpg", 44));
        list.Add(P("AUD-SPK-214", "Bookshelf Bluetooth Speakers", "bookshelf-bluetooth-speakers",
            "Pair of powered bookshelf speakers.",
            "Pair of powered bookshelf Bluetooth speakers with rich stereo sound for home audio.",
            129.99m, null, "audio", "speaker-1.jpg", 38));
        list.Add(P("AUD-SPK-215", "Outdoor Rock Speaker", "outdoor-rock-speaker",
            "Weatherproof speaker disguised as rock.",
            "Weatherproof outdoor Bluetooth speaker disguised as a garden rock.",
            79.99m, null, "audio", "speaker-1.jpg", 52));
        list.Add(P("AUD-SPK-216", "Shower Bluetooth Speaker", "shower-bluetooth-speaker",
            "Suction-cup speaker for the shower.",
            "Suction-cup Bluetooth shower speaker with IPX7 waterproofing and 6-hour battery.",
            24.99m, null, "audio", "speaker-1.jpg", 88));
        list.Add(P("AUD-SPK-217", "Smart Speaker with Voice", "smart-speaker-with-voice",
            "Voice assistant smart speaker.",
            "Smart speaker with built-in voice assistant, room-filling sound, and smart home control.",
            99.99m, 89.99m, "audio", "speaker-1.jpg", 63));

        // Soundbar (7)
        list.Add(P("AUD-SBR-221", "Soundbar 2.1 with Subwoofer", "soundbar-2-1-with-subwoofer",
            "2.1 soundbar with wireless sub.",
            "2.1-channel soundbar with wireless subwoofer, Dolby Audio, and HDMI ARC.",
            179.99m, null, "audio", "soundbar-1.jpg", 46));
        list.Add(P("AUD-SBR-222", "Compact Soundbar", "compact-soundbar",
            "Slim soundbar for small TVs.",
            "Compact slim soundbar perfect for TVs up to 43 inches, with Bluetooth.",
            89.99m, 79.99m, "audio", "soundbar-1.jpg", 59));
        list.Add(P("AUD-SBR-223", "Dolby Atmos Soundbar", "dolby-atmos-soundbar",
            "5.1.2 Atmos soundbar system.",
            "5.1.2 Dolby Atmos soundbar with up-firing drivers for immersive height effects.",
            399.99m, null, "audio", "soundbar-1.jpg", 24));
        list.Add(P("AUD-SBR-224", "Soundbar with Built-in Sub", "soundbar-with-built-in-sub",
            "All-in-one soundbar, no separate sub.",
            "All-in-one soundbar with built-in dual subwoofers, no extra box needed.",
            149.99m, null, "audio", "soundbar-1.jpg", 41));
        list.Add(P("AUD-SBR-225", "Gaming Soundbar", "gaming-soundbar",
            "Low-latency soundbar for gaming.",
            "Gaming soundbar with low-latency mode, RGB lighting, and 3D audio.",
            119.99m, null, "audio", "soundbar-1.jpg", 37));
        list.Add(P("AUD-SBR-226", "Outdoor Soundbar", "outdoor-soundbar",
            "Weatherproof patio soundbar.",
            "Weatherproof outdoor soundbar for patios with IPX5 rating.",
            249.99m, null, "audio", "soundbar-1.jpg", 22));
        list.Add(P("AUD-SBR-227", "Mini Soundbar for PC", "mini-soundbar-for-pc",
            "Desktop soundbar with USB-C.",
            "Mini desktop soundbar with USB-C audio, perfect for PC setups.",
            59.99m, null, "audio", "soundbar-1.jpg", 68));

        // Wired Earphones (7)
        list.Add(P("AUD-ERP-231", "Hi-Fi Wired Earphones", "hi-fi-wired-earphones",
            "Audiophile earphones with MMCX.",
            "Hi-Fi wired earphones with detachable MMCX cable and balanced armature drivers.",
            79.99m, null, "audio", "earphones-1.jpg", 74));
        list.Add(P("AUD-ERP-232", "USB-C Earphones", "usb-c-earphones",
            "Digital USB-C earphones with DAC.",
            "USB-C earphones with built-in DAC for high-quality digital audio.",
            34.99m, null, "audio", "earphones-1.jpg", 96));
        list.Add(P("AUD-ERP-233", "Studio Monitor Earphones", "studio-monitor-earphones",
            "Flat-response in-ear monitors.",
            "In-ear monitor earphones with flat studio response for mixing.",
            119.99m, 99.99m, "audio", "earphones-1.jpg", 45));
        list.Add(P("AUD-ERP-234", "Sport Wired Earphones", "sport-wired-earphones",
            "Sweat-proof with ear hooks.",
            "Sweat-proof wired sport earphones with secure ear hooks.",
            24.99m, null, "audio", "earphones-1.jpg", 82));
        list.Add(P("AUD-ERP-235", "Wooden Earphones", "wooden-earphones",
            "Natural wood housing earphones.",
            "Earphones with natural wood housings for warm, rich sound.",
            59.99m, null, "audio", "earphones-1.jpg", 57));
        list.Add(P("AUD-ERP-236", "Gaming Earphones with Mic", "gaming-earphones-with-mic",
            "Low-latency gaming earphones.",
            "Gaming earphones with boom mic and low-latency mode.",
            39.99m, null, "audio", "earphones-1.jpg", 69));
        list.Add(P("AUD-ERP-237", "Sleep Earphones", "sleep-earphones",
            "Ultra-slim for side sleepers.",
            "Ultra-slim sleep earphones comfortable for side sleepers.",
            29.99m, null, "audio", "earphones-1.jpg", 78));

        // Microphone (7)
        list.Add(P("AUD-MIC-241", "USB Condenser Microphone", "usb-condenser-microphone",
            "Studio-quality USB mic.",
            "Studio-quality USB condenser microphone with cardioid pattern, perfect for podcasts.",
            89.99m, null, "audio", "mic-1.jpg", 53));
        list.Add(P("AUD-MIC-242", "Wireless Lavalier Mic", "wireless-lavalier-mic",
            "Clip-on wireless mic for video.",
            "Wireless lavalier microphone with 200m range for vlogging and interviews.",
            69.99m, 59.99m, "audio", "mic-1.jpg", 66));
        list.Add(P("AUD-MIC-243", "Dynamic Broadcast Mic", "dynamic-broadcast-mic",
            "Broadcast dynamic microphone.",
            "Broadcast-quality dynamic microphone with shock mount for streaming.",
            129.99m, null, "audio", "mic-1.jpg", 39));
        list.Add(P("AUD-MIC-244", "Shotgun Microphone", "shotgun-microphone",
            "Directional mic for cameras.",
            "Shotgun microphone with supercardioid pickup for cameras and recorders.",
            99.99m, null, "audio", "mic-1.jpg", 44));
        list.Add(P("AUD-MIC-245", "USB Gaming Microphone", "usb-gaming-microphone",
            "RGB mic for streamers.",
            "USB gaming microphone with RGB lighting and tap-to-mute.",
            59.99m, null, "audio", "mic-1.jpg", 71));
        list.Add(P("AUD-MIC-246", "Handheld Wireless Mic", "handheld-wireless-mic",
            "UHF wireless handheld microphone.",
            "UHF wireless handheld microphone for karaoke and events.",
            79.99m, null, "audio", "mic-1.jpg", 48));
        list.Add(P("AUD-MIC-247", "Boundary Conference Mic", "boundary-conference-mic",
            "Table mic for meeting rooms.",
            "Boundary conference microphone with 360-degree pickup for meeting rooms.",
            149.99m, null, "audio", "mic-1.jpg", 26));
    }

    private static ExpandedCatalog.ProductDef P(
        string sku, string name, string slug,
        string shortDescription, string description,
        decimal price, decimal? salePrice, string categorySlug,
        string imageFile, int stock, bool isFeatured = false) =>
        new(sku, name, slug, shortDescription, description,
            price, salePrice, categorySlug, imageFile, stock, isFeatured);
}
