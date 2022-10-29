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
      title: 'E-commerce',
      icon: 'shopping-cart-outline',
      link: '/pages/dashboard',
      home: true,
    },
    {
      title: 'IoT Dashboard',
      icon: 'home-outline',
      link: '/pages/iot-dashboard',
    },
    {
      title: 'FEATURES',
      group: true,
    },
    {
      title: 'Layout',
      icon: 'layout-outline',
      children: [
        {
          title: 'Stepper',
          link: '/pages/layout/stepper',
        },
        {
          title: 'List',
          link: '/pages/layout/list',
        },
        {
          title: 'Infinite List',
          link: '/pages/layout/infinite-list',
        },
        {
          title: 'Accordion',
          link: '/pages/layout/accordion',
        },
        {
          title: 'Tabs',
          pathMatch: 'prefix',
          link: '/pages/layout/tabs',
        },
      ],
    },
  ];

  menu: NbMenuItem[] = [];
  constructor(
  ) { }
  ngOnInit() {
    this.menu = this.MENU_ITEMS;
  }

}
