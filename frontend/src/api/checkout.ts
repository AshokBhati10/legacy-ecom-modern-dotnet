import { get, post } from './client';
import type {
  AddressRequest,
  CheckoutSummaryDto,
  PlaceOrderRequest,
  PlaceOrderResponse,
  ShippingOptionsResponse,
} from '../types';

export const checkoutApi = {
  getAddress: () => get<AddressRequest | undefined>('/api/checkout/address'),
  setAddress: (address: AddressRequest) => post<undefined>('/api/checkout/address', address),
  shippingOptions: () => get<ShippingOptionsResponse>('/api/checkout/shipping-options'),
  setShippingMethod: (shippingMethod: string) =>
    post<{ shippingMethod: string }>('/api/checkout/shipping', { shippingMethod }),
  summary: (shippingMethod?: string) =>
    get<CheckoutSummaryDto>(
      `/api/checkout/summary${shippingMethod ? `?shippingMethod=${encodeURIComponent(shippingMethod)}` : ''}`,
    ),
  placeOrder: (input: PlaceOrderRequest) =>
    post<PlaceOrderResponse>('/api/checkout/place-order', input),
};
