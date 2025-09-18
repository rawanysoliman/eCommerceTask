import { Injectable, signal } from '@angular/core';

export interface ToastMessage {
    id: number;
    text: string;
    type: 'success' | 'error' | 'info' | 'warning';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
    messages = signal<ToastMessage[]>([]);
    private counter = 0;

    show(text: string, type: ToastMessage['type'] = 'info', ms = 3000) {
        const id = ++this.counter;
        this.messages.update(list => [...list, { id, text, type }]);
        setTimeout(() => this.dismiss(id), ms);
    }

    error(text: string) { this.show(text, 'error'); }
    success(text: string) { this.show(text, 'success'); }
    info(text: string) { this.show(text, 'info'); }
    warning(text: string) { this.show(text, 'warning'); }

    dismiss(id: number) {
        this.messages.update(list => list.filter(m => m.id !== id));
    }
}


