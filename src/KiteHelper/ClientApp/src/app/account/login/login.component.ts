import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NbDialogService, NbToastrService } from '@nebular/theme';
import { KiteApiService, KiteLoginResponseModel } from 'src/app/services/api/kite-api.service';
import { CookieHelperService } from 'src/app/services/cookie-helper.service';
import { RoutingHelperService } from 'src/app/services/routing-helper.service';
import { ErrorDialogComponent } from 'src/app/shared/error-dialog/error-dialog.component';
import { environment } from 'src/environments/environment';

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
    private cookieService: CookieHelperService,
    private toastrService: NbToastrService,
    private routingService: RoutingHelperService,
    private kiteApiService: KiteApiService,
    private dialogService: NbDialogService,
  ) {
  }

  ngOnInit(): void {
    this.user = new UserNgModel();
    if (this.cookieService.isLoggedIn()) {
      this.navigateFromLoginToDestination();
    }
  }

  async login(): Promise<void> {
    this.loginRequestSpinner = true;
    await this.kiteApiService.login(
      {
        UserName: this.user.username,
        Password: this.user.password,
        AppCode: this.user.appCode,
      },
    ).toPromise()
      .then(
        (response: HttpResponse<KiteLoginResponseModel>) => {
          this.cookieService.setAuthorizationToken(response.body.SessionId);
          this.navigateFromLoginToDestination();
        }, (errorResponse: HttpErrorResponse) => {
          console.error(errorResponse);
          if (errorResponse.status > 0) {
            this.toastrService.danger(JSON.stringify(errorResponse.error), environment.defaultErrorTitle, { duration: 5000 });
            this.dialogService.open(ErrorDialogComponent,
              {
                hasBackdrop: true,
                context: {
                  errorTitle: environment.defaultErrorTitle,
                  errorMessage: JSON.stringify(errorResponse.error),
                },
              });
          } else {
            this.toastrService.danger(environment.defaultErrorMessage, environment.defaultErrorTitle);
          }
        },
      )
      .catch(
        (exception: any) => {
          console.error(exception);
          this.toastrService.danger(environment.defaultErrorMessage, environment.defaultErrorTitle);
        },
      )
      .finally(() => {
        this.loginRequestSpinner = false;
      });
  }

  private navigateFromLoginToDestination(): void {
    this.routingService.navigateToDashboard();
  }

}
