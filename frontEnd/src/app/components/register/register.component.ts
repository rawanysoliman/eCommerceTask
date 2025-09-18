import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
    private fb = inject(FormBuilder);
    private auth = inject(AuthService);
    private router = inject(Router);

    submitting = false;
    error: string | null = null;
    success = false;
    showPassword = false;

    form = this.fb.group({
        username: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
    });

    onSubmit() {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        this.error = null;
        this.submitting = true;
        this.auth.register(this.form.getRawValue() as any).subscribe({
            next: () => {
                this.submitting = false;
                this.success = true;
                setTimeout(() => this.router.navigateByUrl('/login'), 1200);
            },
            error: (err) => {
                this.submitting = false;
                this.error = err?.error?.message || err.message || 'Registration failed';
            },
        });
    }
}


