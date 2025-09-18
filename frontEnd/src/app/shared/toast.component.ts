import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from './toast.service';

@Component({
    selector: 'app-toasts',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 1080;">
      <div *ngFor="let m of toasts.messages()" class="toast show align-items-center text-bg-{{ map[m.type] }} border-0 mb-2" role="alert">
        <div class="d-flex">
          <div class="toast-body">{{ m.text }}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" (click)="toasts.dismiss(m.id)"></button>
        </div>
      </div>
    </div>
    `
})
export class ToastComponent {
    toasts = inject(ToastService);
    map: any = { success: 'success', error: 'danger', info: 'info', warning: 'warning' };
}


