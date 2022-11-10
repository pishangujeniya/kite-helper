import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class RoutingHelperService {

  constructor(private router: Router) { }

  public navigateToDashboard() {
    this.router.navigate(['/dashboard/']);
  }

  public navigateToLogin() {
    this.router.navigate(['/account/login']);
  }

  public navigateToLogout() {
    this.router.navigate(['/account/logout']);
  }
}
