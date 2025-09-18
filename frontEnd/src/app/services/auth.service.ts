import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, map, throwError } from 'rxjs';
import { API_CONFIG } from '../core/api.config';

export interface LoginRequest {
    userNameOrEmail: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    user: { id: number; username: string; email: string };
}

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
    private http = inject(HttpClient);

    private authStateSubject = new BehaviorSubject<AuthResponse | null>(this.readFromStorage());
    readonly authState$ = this.authStateSubject.asObservable();

    get accessToken(): string | null {
        return this.authStateSubject.value?.accessToken ?? null;
    }

    get refreshToken(): string | null {
        return this.authStateSubject.value?.refreshToken ?? null;
    }

    get isAdmin(): boolean {
        return this.getRolesFromToken().includes('Admin');
    }

    private getRolesFromToken(): string[] {
        const token = this.accessToken;
        if (!token) return [];
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const claimNs = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
            const r = payload['role'] ?? payload['roles'] ?? payload[claimNs] ?? [];
            return Array.isArray(r) ? r : (r ? [String(r)] : []);
        } catch {
            return [];
        }
    }

    get displayName(): string | null {
        const token = this.accessToken;
        if (!token) return null;
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload['unique_name'] || payload['email'] || null;
        } catch {
            return null;
        }
    }

    login(request: LoginRequest): Observable<AuthResponse> {
        return this.http
            .post<AuthResponse>(`${API_CONFIG.baseUrl}/Auth/login`, request)
            .pipe(
                map((res) => {
                    this.persist(res);
                    this.authStateSubject.next(res);
                    return res;
                }),
                catchError(this.handleError)
            );
    }

    register(request: RegisterRequest) {
        return this.http.post(`${API_CONFIG.baseUrl}/Auth/register`, request).pipe(catchError(this.handleError));
    }

    logout(): void {
        localStorage.removeItem('auth');
        this.authStateSubject.next(null);
    }

    refresh() {
        const token = this.refreshToken;
        if (!token) {
            return throwError(() => new Error('No refresh token'));
        }
        return this.http
            .post<AuthResponse>(`${API_CONFIG.baseUrl}/Auth/refresh`, { refreshToken: token })
            .pipe(
                map((res) => {
                    // persist new tokens
                    const merged: AuthResponse = {
                        ...(this.authStateSubject.value ?? ({} as AuthResponse)),
                        ...res,
                    };
                    this.persist(merged);
                    this.authStateSubject.next(merged);
                    return merged.accessToken;
                }),
                catchError(this.handleError)
            );
    }

    private persist(value: AuthResponse): void {
        localStorage.setItem('auth', JSON.stringify(value));
    }

    private readFromStorage(): AuthResponse | null {
        const raw = localStorage.getItem('auth');
        if (!raw) return null;
        try {
            return JSON.parse(raw) as AuthResponse;
        } catch {
            return null;
        }
    }

    private handleError(error: HttpErrorResponse) {
        const message = error.error?.message || error.statusText || 'Request failed';
        return throwError(() => new Error(message));
    }
}


