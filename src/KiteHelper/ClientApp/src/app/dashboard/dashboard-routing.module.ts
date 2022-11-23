import { RouterModule, Routes } from '@angular/router';
import { Component, NgModule } from '@angular/core';

import { DashboardComponent } from './dashboard.component';
import { NotFoundComponent } from '../shared/not-found/not-found.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
    children: [
      {
        path: 'historical',
        loadChildren: () =>
          import('./historical/historical.module').then((m) => m.HistoricalModule),
      },
      {
        path: '',
        redirectTo: 'historical',
        pathMatch: 'full',
      },
      {
        path: '**',
        component: NotFoundComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashboardRoutingModule { }
