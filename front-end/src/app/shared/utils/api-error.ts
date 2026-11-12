import { HttpErrorResponse } from '@angular/common/http';

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as { detail?: string; title?: string; errors?: Record<string, string[]> } | null;
    if (body?.detail) return body.detail;
    if (body?.errors) {
      return Object.values(body.errors).flat().join(' ');
    }
    if (body?.title) return body.title;
    if (error.status === 0) return 'Cannot reach the API. Check that the backend is running and the API URL is correct.';
    return `The request failed (HTTP ${error.status}).`;
  }
  return 'Something went wrong. Please try again.';
}
