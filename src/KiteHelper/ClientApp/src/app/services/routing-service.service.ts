import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class RoutingServiceService {

  constructor(private router: Router) { }

  public navigateToDashboard() {
    this.router.navigate(['/dashboard/']);
  }

  public navigateToLogin() {
    this.router.navigate(['/']);
  }
}
