import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NbToastrService } from '@nebular/theme';
import { CookieServiceService } from 'src/app/services/cookie-service.service';
import { RoutingServiceService } from 'src/app/services/routing-service.service';

class UserNgModel {
  username: string;
  password: string;
  appCode: number;
}
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {

  public user: UserNgModel = new UserNgModel();

  public loginRequestSpinner = false;
  constructor(
    private cookieService: CookieServiceService,
    private toasterService: NbToastrService,
    private routingService: RoutingServiceService,
  ) {
  }

  ngOnInit(): void {
    this.user = new UserNgModel();
    if (this.cookieService.isLoggedIn()) { // Means if login is already done
      this.navigateFromLoginToDestination();
    }
  }

  async login(): Promise<void> {
    this.loginRequestSpinner = true;
  }

  private navigateFromLoginToDestination(): void {
    // this.routingService.navigateToDashboard();
  }

}
