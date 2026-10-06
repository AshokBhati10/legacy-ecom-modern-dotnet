// TypeScript mirrors of the backend DTOs (camelCase, matching ASP.NET Core
// default JSON serialization). The backend is authoritative; these shapes
// must track src/LegacyEcom.Application/DTOs.

export interface ProductCardDto {
  id: number;
  name: string;
  slug: string;
  price: number;
  salePrice: number | null;
  effectivePrice: number;
  thumbnailUrl: string | null;
}

export interface ProductImageDto {
  id: number;
  url: string;
  altText: string | null;
  displayOrder: number;
  isMain: boolean;
}

export interface ProductVariantDto {
  id: number;
  name: string;
  sku: string | null;
  priceAdjustment: number;
  stockQuantity: number;
  isActive: boolean;
}

export interface CategoryDto {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  hasChildren: boolean;
}

export interface ProductDetailDto {
  id: number;
  sku: string;
  name: string;
  slug: string;
  shortDescription: string | null;
  description: string | null;
  price: number;
  salePrice: number | null;
  effectivePrice: number;
  categoryId: number;
  categoryName: string | null;
  thumbnailUrl: string | null;
  isFeatured: boolean;
  stockQuantity: number;
  images: ProductImageDto[];
  variants: ProductVariantDto[];
  relatedProducts: ProductCardDto[];
}

export interface ProductListDto {
  products: ProductCardDto[];
  categories: CategoryDto[];
  selectedCategoryId: number | null;
  searchQuery: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CartLineDto {
  productId: number;
  variantId: number | null;
  productName: string;
  variantName: string | null;
  sku: string | null;
  thumbnailUrl: string | null;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface CartDto {
  items: CartLineDto[];
  itemCount: number;
  subTotal: number;
}

export interface MiniCartDto {
  itemCount: number;
  subTotal: number;
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
}

export interface AddressRequest {
  email: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  street: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

export interface ShippingOptionDto {
  code: string;
  name: string;
  description: string;
  cost: number;
}

export interface ShippingOptionsResponse {
  options: ShippingOptionDto[];
  selectedMethod: string | null;
  summary: CheckoutSummaryDto;
}

export interface CheckoutSummaryDto {
  items: CartLineDto[];
  itemCount: number;
  subTotal: number;
  shippingMethod: string;
  shippingCost: number;
  taxAmount: number;
  total: number;
}

export interface PlaceOrderRequest {
  paymentMethod: string;
  cardholderName?: string;
  cardNumber?: string;
  expiryMonth?: number;
  expiryYear?: number;
  cvv?: string;
}

export interface PlaceOrderResponse {
  orderId: number;
  orderNumber: string;
}

export interface OrderLineDto {
  id: number;
  productId: number;
  variantId: number | null;
  productName: string;
  variantName: string | null;
  sku: string | null;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface OrderDto {
  id: number;
  orderNumber: string;
  orderDate: string;
  status: string;
  subTotal: number;
  shippingCost: number;
  taxAmount: number;
  total: number;
  shippingMethod: string;
  paymentMethod: string;
  shipFirstName: string;
  shipLastName: string;
  shipEmail: string;
  shipPhone: string | null;
  shipStreet: string;
  shipCity: string;
  shipState: string;
  shipPostalCode: string;
  shipCountry: string;
  lines: OrderLineDto[];
}

export interface OrderSummaryDto {
  orderId: number;
  orderNumber: string;
  orderDate: string;
  status: string;
  itemCount: number;
  total: number;
}
