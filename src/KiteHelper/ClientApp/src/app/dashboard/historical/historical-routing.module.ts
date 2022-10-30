import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HistoricalDataComponent } from './historical-data/historical-data.component';
import { HistoricalComponent } from './historical.component';

const routes: Routes = [
  {
    path: '',
    component: HistoricalComponent,
    children: [
      {
        path: 'data',
        component: HistoricalDataComponent,
      },
      {
        path: '',
        redirectTo: 'data',
        pathMatch: 'full',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HistoricalRoutingModule { }
