import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class CookieServiceService {

  constructor(private cookieService: CookieService) { }

  public isLoggedIn(): boolean {
    const t = this.cookieService.get('is_logged_in');
    return t !== null && t !== undefined;
  }
}
