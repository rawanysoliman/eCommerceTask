import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
    const auth = inject(AuthService);

    const token = auth.accessToken;
    const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

    return next(authReq).pipe(
        catchError((err: HttpErrorResponse) => {
            if (err.status === 401 && auth.refreshToken) {
                // try refresh once, then retry original
                return auth.refresh().pipe(
                    switchMap((newToken) => {
                        const retryReq = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
                        return next(retryReq);
                    }),
                    catchError((refreshErr) => {
                        auth.logout();
                        return throwError(() => refreshErr);
                    })
                );
            }
            return throwError(() => err);
        })
    );
}


