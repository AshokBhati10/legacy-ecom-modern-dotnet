import { get } from './client';
import type { CategoryDto, ProductDetailDto, ProductCardDto, ProductListDto } from '../types';

export interface ListingParams {
  categoryId?: number;
  q?: string;
  page?: number;
  pageSize?: number;
}

export const catalogApi = {
  list: (params: ListingParams = {}) => {
    const search = new URLSearchParams();
    if (params.categoryId) search.set('categoryId', String(params.categoryId));
    if (params.q) search.set('q', params.q);
    if (params.page) search.set('page', String(params.page));
    if (params.pageSize) search.set('pageSize', String(params.pageSize));
    const qs = search.toString();
    return get<ProductListDto>(`/api/products${qs ? `?${qs}` : ''}`);
  },
  detail: (id: number) => get<ProductDetailDto>(`/api/products/${id}`),
  featured: (take = 8) => get<ProductCardDto[]>(`/api/products/featured?take=${take}`),
  categories: () => get<CategoryDto[]>('/api/categories'),
  categoryTree: (parentId?: number | null) =>
    get<CategoryDto[]>(`/api/categories/tree${parentId ? `?parentId=${parentId}` : ''}`),
};
