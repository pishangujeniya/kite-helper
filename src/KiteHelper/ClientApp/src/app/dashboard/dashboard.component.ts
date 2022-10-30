import { Component, OnInit } from '@angular/core';
import { NbMenuItem } from '@nebular/theme';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {


  MENU_ITEMS: NbMenuItem[] = [
    {
      title: 'Historical Data',
      icon: 'activity-outline',
      link: '/dashboard/historical/data',
    },
  ];

  menu: NbMenuItem[] = [];
  constructor(
  ) { }
  ngOnInit() {
    this.menu = this.MENU_ITEMS;
  }

}
