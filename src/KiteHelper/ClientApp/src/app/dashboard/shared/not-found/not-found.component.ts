import { Component } from '@angular/core';
import { RoutingHelperService } from 'src/app/services/routing-helper.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss'],
})
export class NotFoundComponent {

  constructor(
    private routingService: RoutingHelperService,
  ) { }

  navigateToLogin() {
    this.routingService.navigateToLogin();
  }
}
