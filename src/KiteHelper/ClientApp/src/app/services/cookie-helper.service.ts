import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class CookieHelperService {

  private readonly AuthorizationKey: string = 'Authorization';

  constructor(private cookieService: CookieService) { }

  public isLoggedIn(): boolean {
    const t = this.cookieService.get(this.AuthorizationKey);
    return t !== null && t !== undefined && t.length > 0;
  }

  public logout() {
    this.cookieService.delete(this.AuthorizationKey, '/');
  }

  public setAuthorizationToken(sessionId: string) {
    this.cookieService.set(
      this.AuthorizationKey,
      sessionId,
      {
        path: '/',
        secure: window.location.protocol === 'https:',
      },
    );
  }

  public getAuthorizationToken(): string {
    const t = this.cookieService.get(this.AuthorizationKey);
    return (t !== null && t !== undefined) ? t : '';
  }
}
