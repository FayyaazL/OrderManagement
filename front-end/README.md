# Order Management UI

Angular standalone application for the Order Intake & Tracking API.

## Requirements

- Node.js 22.12 or later (Node 22 LTS recommended)
- npm
- Angular CLI 21 (the local npm scripts use the project CLI)

## Install and run

```bash
npm install
npm start
```

Open `http://localhost:4200`.

## Configure the API URL

Edit `src/environments/environment.ts` and set `apiUrl` to the HTTPS URL printed by the ASP.NET Core API when it starts. For example:

```ts
apiUrl: 'https://localhost:7001/api'
```

The API must allow CORS requests from `http://localhost:4200`.

## Features

- List orders, newest first
- Search by customer or external reference
- Filter by status
- Create orders with one or more lines
- Client-side input validation
- Display server-calculated totals returned by the API
- View order details
- Change status using the permitted transition buttons
- Display API errors, including duplicate external-reference conflicts

## Build

```bash
npm run build
```

## API contract

The UI expects:

- `GET /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders`
- `PATCH /api/orders/{id}/status`

Order status values are `Pending`, `Confirmed`, `Fulfilled`, and `Cancelled`.
