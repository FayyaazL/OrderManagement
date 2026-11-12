import { Routes } from '@angular/router';
import { OrderListComponent } from './features/orders/pages/order-list/order-list.component';
import { OrderCreateComponent } from './features/orders/pages/order-create/order-create.component';
import { OrderDetailsComponent } from './features/orders/pages/order-details/order-details.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'orders' },
  { path: 'orders', component: OrderListComponent, title: 'Orders' },
  { path: 'orders/new', component: OrderCreateComponent, title: 'Create order' },
  { path: 'orders/:id', component: OrderDetailsComponent, title: 'Order details' },
  { path: '**', redirectTo: 'orders' }
];
