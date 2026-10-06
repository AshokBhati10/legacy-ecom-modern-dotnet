import { get } from './client';
import type { OrderDto, OrderSummaryDto } from '../types';

export const ordersApi = {
  history: () => get<OrderSummaryDto[]>('/api/orders'),
  detail: (id: number) => get<OrderDto>(`/api/orders/${id}`),
};
