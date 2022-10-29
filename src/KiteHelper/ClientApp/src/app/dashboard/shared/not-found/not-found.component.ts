import { Component } from '@angular/core';
import { RoutingServiceService } from 'src/app/services/routing-service.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss'],
})
export class NotFoundComponent {

  constructor(
    private routingService: RoutingServiceService,
  ) { }

  navigateToLogin() {
    this.routingService.navigateToLogin();
  }
}
