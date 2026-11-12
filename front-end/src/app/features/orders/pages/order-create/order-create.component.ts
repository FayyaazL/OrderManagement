import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { OrderService } from '../../../../core/services/order.service';
import { CreateOrderRequest } from '../../../../core/models/order.model';
import { getApiErrorMessage } from '../../../../shared/utils/api-error';

@Component({
  selector: 'app-order-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './order-create.component.html',
  styleUrl: './order-create.component.scss'
})
export class OrderCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly ordersApi = inject(OrderService);
  private readonly router = inject(Router);

  submitting = signal(false);
  error = signal('');
  successMessage = signal('');

  form = this.fb.group({
    customerName: ['', [Validators.required, Validators.maxLength(200)]],
    externalReference: ['', [Validators.required, Validators.maxLength(100)]],
    lines: this.fb.array([this.createLineGroup()])
  });

  get lines(): FormArray { return this.form.controls.lines; }

  private createLineGroup() {
    return this.fb.group({
      productName: ['', [Validators.required, Validators.maxLength(200)]],
      quantity: [1, [Validators.required, Validators.min(1), Validators.pattern(/^[1-9]\d*$/)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  addLine(): void { this.lines.push(this.createLineGroup()); }

  removeLine(index: number): void {
    if (this.lines.length > 1) this.lines.removeAt(index);
  }

  get estimatedTotal(): number {
    return this.lines.controls.reduce((total, control) => {
      const value = control.getRawValue();
      return total + (Number(value.quantity) || 0) * (Number(value.unitPrice) || 0);
    }, 0);
  }

  submit(): void {
    this.error.set('');
    this.successMessage.set('');
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const request: CreateOrderRequest = {
      customerName: raw.customerName!.trim(),
      externalReference: raw.externalReference!.trim(),
      lines: raw.lines.map(line => ({
        productName: line.productName!.trim(),
        quantity: Number(line.quantity),
        unitPrice: Number(line.unitPrice)
      }))
    };

    this.submitting.set(true);
    this.ordersApi.create(request).subscribe({
      next: order => {
        this.submitting.set(false);
        void this.router.navigate(['/orders', order.id]);
      },
      error: error => {
        this.error.set(getApiErrorMessage(error));
        this.submitting.set(false);
      }
    });
  }
}
