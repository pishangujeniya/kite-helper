import { Injectable, Injector } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { RoutingHelperService } from '../services/routing-helper.service';

@Injectable()
export class UnauthorizedInterceptor implements HttpInterceptor {

  constructor(
    private injector: Injector,
  ) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(tap(() => { },
      (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 401) {
            this.injector.get(RoutingHelperService).navigateToLogout();
          }
        }
      }));
  }
}
