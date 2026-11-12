export type OrderStatus = 'Pending' | 'Confirmed' | 'Fulfilled' | 'Cancelled';

export interface OrderLineRequest {
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface CreateOrderRequest {
  customerName: string;
  externalReference: string;
  lines: OrderLineRequest[];
}

export interface OrderLine {
  id: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface Order {
  id: string;
  customerName: string;
  externalReference: string;
  createdAtUtc: string;
  status: OrderStatus;
  lines: OrderLine[];
  total: number;
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}
