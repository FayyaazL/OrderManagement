import { Component, input } from '@angular/core';
import { OrderStatus } from '../../../core/models/order.model';

@Component({
  selector: 'app-status-pill',
  standalone: true,
  template: `<span class="pill" [class.pending]="status() === 'Pending'"
    [class.confirmed]="status() === 'Confirmed'"
    [class.fulfilled]="status() === 'Fulfilled'"
    [class.cancelled]="status() === 'Cancelled'">{{ status() }}</span>`,
  styles: [`
    .pill { display:inline-flex; align-items:center; border-radius:20px; padding:5px 10px; font-size:11px; font-weight:700; }
    .pending { color:#9a6700; background:#fff3d6; }
    .confirmed { color:#2457b2; background:#e8efff; }
    .fulfilled { color:#087b56; background:#dcf7eb; }
    .cancelled { color:#a33b47; background:#ffeaec; }
  `]
})
export class StatusPillComponent {
  status = input.required<OrderStatus>();
}
