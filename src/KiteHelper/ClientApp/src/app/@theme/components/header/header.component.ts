import { HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { NbMenuService, NbSidebarService, NbToastrService } from '@nebular/theme';

import { Subject } from 'rxjs';
import { KiteApiService, ProfileResponseModel } from 'src/app/services/api/kite-api.service';
import { RoutingHelperService } from 'src/app/services/routing-helper.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-header',
  styleUrls: ['./header.component.scss'],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit, OnDestroy {

  userPictureOnly = false;
  user: {
    name: string;
    picture: string;
  };

  userMenu = [
    {
      title: 'Log out',
      link: '/account/logout',
    },
  ];

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private sidebarService: NbSidebarService,
    private hamburgerService: NbMenuService,
    private toastrService: NbToastrService,
    private kiteApiService: KiteApiService,
  ) { }

  ngOnInit() {
    this.user = { name: '', picture: './../../../../assets/images/icon.png' };
    this.getProfile();
  }

  async getProfile() {
    await this.kiteApiService.profile(
    ).toPromise()
      .then(
        (response: HttpResponse<ProfileResponseModel>) => {
          this.user.name = response.body.userName;
          this.user.picture = response.body.avatarURL;
        }, (errorResponse: HttpErrorResponse) => {
          console.error(errorResponse);
          if (errorResponse.status > 0) {
            this.toastrService.danger(JSON.stringify(errorResponse.error), environment.defaultErrorTitle);
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
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggleSidebar(): boolean {
    this.sidebarService.toggle(true, 'menu-sidebar');

    return false;
  }

  navigateHome() {
    this.hamburgerService.navigateHome();
    return false;
  }
}
