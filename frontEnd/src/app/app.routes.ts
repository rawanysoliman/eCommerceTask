import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent) },
    { path: 'register', loadComponent: () => import('./components/register/register.component').then(m => m.RegisterComponent) },
    { path: 'products', canActivate: [authGuard], loadComponent: () => import('./components/products/Product-list/product-list.component').then(m => m.ProductListComponent) },
    { path: 'products/:id', canActivate: [authGuard], loadComponent: () => import('./components/products/product-detail/product-detail.component').then(m => m.ProductDetailComponent) },
    { path: 'admin/products/new', canActivate: [authGuard], loadComponent: () => import('./components/products/product-form/product-form.component').then(m => m.ProductFormComponent) },
    { path: 'admin/products/:id/edit', canActivate: [authGuard], loadComponent: () => import('./components/products/product-form/product-form.component').then(m => m.ProductFormComponent) },
    { path: '**', redirectTo: 'login' }
];
