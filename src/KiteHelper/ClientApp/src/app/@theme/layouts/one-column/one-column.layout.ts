import { Component } from '@angular/core';

@Component({
  selector: 'app-one-column-layout',
  styleUrls: ['./one-column.layout.scss'],
  template: `
    <nb-layout class="with-scroll">
      <nb-layout-header fixed>
        <app-header></app-header>
      </nb-layout-header>

      <nb-sidebar class="menu-sidebar" tag="menu-sidebar" responsive>
        <ng-content select="nb-menu"></ng-content>
      </nb-sidebar>

      <nb-layout-column style="padding: 1rem;">
        <ng-content select="router-outlet"></ng-content>
      </nb-layout-column>

      <nb-layout-footer fixed>
        <app-footer></app-footer>
      </nb-layout-footer>
    </nb-layout>
  `,
})
export class OneColumnLayoutComponent {}
