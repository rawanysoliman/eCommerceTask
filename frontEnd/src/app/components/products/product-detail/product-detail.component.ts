import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { ProductModel } from '../../../shared/models';
import { AuthService } from '../../../services/auth.service';
import { API_CONFIG } from '../../../core/api.config';

@Component({
    selector: 'app-product-detail',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './product-detail.component.html',
    styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
    private route = inject(ActivatedRoute);
    svc = inject(ProductService);
    auth = inject(AuthService);
    product?: ProductModel;
    base = API_CONFIG.baseUrl.replace(/\/api$/, '');

    ngOnInit(): void {
        const id = Number(this.route.snapshot.paramMap.get('id'));
        if (!id) return;
        this.svc.getById(id).subscribe(p => this.product = p);
    }

    buildImageUrl(path?: string | null): string {
        if (!path) return ''; // no image
        if (path.startsWith('http://') || path.startsWith('https://')) return path; // full URL
        return `${API_CONFIG.baseUrl.replace(/\/?api$/, '')}/${path.replace(/^\/+/, '')}`;
    }
}


