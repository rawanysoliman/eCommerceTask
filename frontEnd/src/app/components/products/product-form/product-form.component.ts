import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { ProductModel } from '../../../shared/models';      

@Component({
    selector: 'app-product-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './product-form.component.html',
    styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
    private fb = inject(FormBuilder);
    private svc = inject(ProductService);
    private route = inject(ActivatedRoute);
    private router = inject(Router);

    id?: number;
    submitting = false;
    error: string | null = null;

    form = this.fb.group({
        name: ['', [Validators.required, Validators.minLength(2)]],
        category: ['', [Validators.required, Validators.minLength(2)]],
        price: [0, [Validators.required, Validators.min(0.01)]],
        minimumQuantity: [1, [Validators.required, Validators.min(1)]],
        discountRate: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
        image: [null as File | null]
    });

    ngOnInit(): void {
        this.id = Number(this.route.snapshot.paramMap.get('id')) || undefined;
        if (this.id) {
            this.svc.getById(this.id).subscribe(p => {
                this.form.patchValue({ name: p.name, category: p.category, minimumQuantity: p.minimumQuantity, discountRate: p.discountRate, price: p.price });
            });
        }
    }

    onFileSelected(event: Event) {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length) {
            this.form.patchValue({ image: input.files[0] });
        }
    }

    onSubmit() {
        if (this.form.invalid) { this.form.markAllAsTouched(); return; }
        this.submitting = true;
        const value = this.form.getRawValue();
        const data = new FormData();
        data.append('Name', String(value.name));
        data.append('Category', String(value.category));
        data.append('MinimumQuantity', String(value.minimumQuantity));
        data.append('DiscountRate', String(value.discountRate));
        data.append('Price', String(value.price));
        if (value.image) data.append('Image', value.image);

        const obs = this.id ? this.svc.update(this.id, data) : this.svc.create(data);
        obs.subscribe({
            next: () => {
                this.submitting = false;
                this.router.navigateByUrl('/products');
            },
            error: (err) => {
                this.submitting = false;
                this.error = err?.error?.message || err.message || 'Save failed';
            }
        });
    }
}


