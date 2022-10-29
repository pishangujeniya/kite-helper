import { Component, OnDestroy, OnInit } from '@angular/core';
import { NbMenuService, NbSidebarService } from '@nebular/theme';

import { Subject } from 'rxjs';

@Component({
  selector: 'app-header',
  styleUrls: ['./header.component.scss'],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit, OnDestroy {

  userPictureOnly = false;
  user: any;

  userMenu = [
    {
      title: 'Profile',
      link: '/dashboard/users/user-profile',
    },
    {
      title: 'Log out',
      link: '/account/logout',
    },
  ];

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private sidebarService: NbSidebarService,
    private hamburgerService: NbMenuService,
  ) { }

  ngOnInit() {
    this.user = { name: 'John Doe', picture: '../../../../assets/images/jack.png' };
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
