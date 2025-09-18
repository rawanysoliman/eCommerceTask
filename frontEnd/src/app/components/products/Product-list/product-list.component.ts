import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { ProductModel } from '../../../shared/models';
import { AuthService } from '../../../services/auth.service';
import { API_CONFIG } from '../../../core/api.config';

@Component({
    selector: 'app-product-list',
    standalone: true,
    imports: [CommonModule, CurrencyPipe, RouterLink],
    templateUrl: './product-list.component.html',
    styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
    svc = inject(ProductService);
    auth = inject(AuthService);
    private baseOrigin = (() => {
        try {
            const url = new URL(API_CONFIG.baseUrl);
            return `${url.protocol}//${url.host}`;
        } catch {
            return API_CONFIG.baseUrl.replace(/\/?api$/, '');
        }
    })();

    ngOnInit(): void {
        this.svc.loadAll().subscribe();
    }

    buildImageUrl(path?: string | null): string {
        if (!path) return ''; // no image
        if (path.startsWith('http://') || path.startsWith('https://')) return path; // full URL
        return `${API_CONFIG.baseUrl.replace(/\/?api$/, '')}/${path.replace(/^\/+/, '')}`;
    }

    deleteProduct(id: number) {
        this.svc.delete(id).subscribe(() => {
          this.svc.loadAll().subscribe();
        });
      }
      
}


