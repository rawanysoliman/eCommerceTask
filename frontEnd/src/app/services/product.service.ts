import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { API_CONFIG } from '../core/api.config';
import { AuthService } from './auth.service';
import { ProductModel } from '../shared/models';

@Injectable({ providedIn: 'root' })
export class ProductService {
    private http = inject(HttpClient);
    private auth = inject(AuthService);

    private productsSubject = new BehaviorSubject<ProductModel[]>([]);
    readonly products$ = this.productsSubject.asObservable();

    loadAll(): Observable<ProductModel[]> {
        const headers = this.buildAuthHeaders();
        return this.http
            .get<ProductModel[]>(`${API_CONFIG.baseUrl}/Products`, { headers })
            .pipe(tap((list) => this.productsSubject.next(list)));
    }

    getById(id: number): Observable<ProductModel> {
        return this.http.get<ProductModel>(`${API_CONFIG.baseUrl}/Products/${id}`, { headers: this.buildAuthHeaders() });
    }

    create(formData: FormData): Observable<ProductModel> {
        // Backend expects multipart/form-data for image upload
        return this.http.post<ProductModel>(`${API_CONFIG.baseUrl}/Products`, formData, { headers: this.buildAuthHeaders() });
    }

    update(id: number, formData: FormData): Observable<ProductModel> {
        return this.http.put<ProductModel>(`${API_CONFIG.baseUrl}/Products/${id}`, formData, { headers: this.buildAuthHeaders() });
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_CONFIG.baseUrl}/Products/${id}`, { headers: this.buildAuthHeaders() });
    }

    private buildAuthHeaders(): HttpHeaders {
        const token = this.auth.accessToken;
        let headers = new HttpHeaders();
        if (token) headers = headers.set('Authorization', `Bearer ${token}`);
        return headers;
    }
}


