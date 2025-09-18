import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent {
    private fb = inject(FormBuilder);
    private auth = inject(AuthService);
    private router = inject(Router);

    submitting = false;
    error: string | null = null;
    showPassword = false;

    form = this.fb.group({
        userNameOrEmail: ['', Validators.required],
        password: ['', Validators.required],
    });

    onSubmit() {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        this.error = null;
        this.submitting = true;
        this.auth.login(this.form.getRawValue() as any).subscribe({
            next: () => {
                this.submitting = false;
                this.router.navigateByUrl('/products');
            },
            error: (err) => {
                this.submitting = false;
                if (err?.status === 401) {
                    this.error = 'Invalid username/email or password.';
                } else {
                    this.error = err?.error?.message || err.message || 'Login failed';
                }
            },
        });
    }
}


