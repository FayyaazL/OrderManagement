import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../../../core/services/order.service';
import { Order, OrderStatus } from '../../../../core/models/order.model';
import { getApiErrorMessage } from '../../../../shared/utils/api-error';
import { StatusPillComponent } from '../../../../shared/components/status-pill/status-pill.component';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [DatePipe, CurrencyPipe, RouterLink, StatusPillComponent],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.scss'
})
export class OrderListComponent implements OnInit {
  private readonly ordersApi = inject(OrderService);
  orders = signal<Order[]>([]);
  loading = signal(true);
  error = signal('');
  search = signal('');
  statusFilter = signal('All');

  filteredOrders = computed(() => {
    const term = this.search().trim().toLowerCase();
    return this.orders().filter(order => {
      const matchesSearch = !term ||
        order.customerName.toLowerCase().includes(term) ||
        order.externalReference.toLowerCase().includes(term);
      const matchesStatus = this.statusFilter() === 'All' || order.status === this.statusFilter();
      return matchesSearch && matchesStatus;
    });
  });

  readonly statuses: Array<'All' | OrderStatus> = ['All', 'Pending', 'Confirmed', 'Fulfilled', 'Cancelled'];

  ngOnInit(): void { this.loadOrders(); }

  loadOrders(): void {
    this.loading.set(true);
    this.error.set('');
    this.ordersApi.getAll().subscribe({
      next: orders => {
        this.orders.set([...orders].sort((a, b) =>
          new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime()));
        this.loading.set(false);
      },
      error: error => {
        this.error.set(getApiErrorMessage(error));
        this.loading.set(false);
      }
    });
  }

  setSearch(value: string): void { this.search.set(value); }
  setStatus(value: string): void { this.statusFilter.set(value); }

  count(status: OrderStatus): number {
    return this.orders().filter(order => order.status === status).length;
  }

  trackById(_index: number, order: Order): string { return order.id; }
}
