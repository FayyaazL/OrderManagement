import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Order, OrderStatus } from '../../../../core/models/order.model';
import { OrderService } from '../../../../core/services/order.service';
import { getApiErrorMessage } from '../../../../shared/utils/api-error';
import { StatusPillComponent } from '../../../../shared/components/status-pill/status-pill.component';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, StatusPillComponent],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.scss'
})
export class OrderDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly ordersApi = inject(OrderService);

  order = signal<Order | null>(null);
  loading = signal(true);
  updating = signal(false);
  error = signal('');
  updateError = signal('');
  updateSuccess = signal('');

  readonly transitions: Record<OrderStatus, OrderStatus[]> = {
    Pending: ['Confirmed', 'Cancelled'],
    Confirmed: ['Fulfilled', 'Cancelled'],
    Fulfilled: [],
    Cancelled: []
  };

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('No order ID was provided.');
      this.loading.set(false);
      return;
    }
    this.loadOrder(id);
  }

  loadOrder(id: string): void {
    this.loading.set(true);
    this.error.set('');
    this.ordersApi.getById(id).subscribe({
      next: order => { this.order.set(order); this.loading.set(false); },
      error: error => { this.error.set(getApiErrorMessage(error)); this.loading.set(false); }
    });
  }

  availableTransitions(): OrderStatus[] {
    const current = this.order();
    return current ? this.transitions[current.status] : [];
  }

  changeStatus(status: OrderStatus): void {
    const current = this.order();
    if (!current || this.updating()) return;
    this.updateError.set('');
    this.updateSuccess.set('');
    this.updating.set(true);
    this.ordersApi.updateStatus(current.id, status).subscribe({
      next: updated => {
        this.order.set(updated);
        this.updateSuccess.set(`Order status updated to ${updated.status}.`);
        this.updating.set(false);
      },
      error: error => {
        this.updateError.set(getApiErrorMessage(error));
        this.updating.set(false);
      }
    });
  }

  statusLabel(status: OrderStatus): string {
    return status === 'Fulfilled' ? 'Mark fulfilled' : status === 'Confirmed' ? 'Confirm order' : 'Cancel order';
  }
}
