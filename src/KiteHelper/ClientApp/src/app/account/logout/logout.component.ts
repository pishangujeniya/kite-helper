import { Component, OnInit } from '@angular/core';
import { CookieHelperService } from 'src/app/services/cookie-helper.service';
import { RoutingHelperService } from 'src/app/services/routing-helper.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss'],
})
export class LogoutComponent implements OnInit {

  constructor(
    private cookieHelperService: CookieHelperService,
    private routingHelperService: RoutingHelperService,
  ) {
  }

  ngOnInit(): void {
    this.logout();
  }

  private logout(): void {
    this.cookieHelperService.logout();
    localStorage.clear();
    sessionStorage.clear();
    this.routingHelperService.navigateToLogin();
  }
}
