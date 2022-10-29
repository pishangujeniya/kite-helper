import { Component, OnInit } from '@angular/core';
import { NbThemeService } from '@nebular/theme';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private themeService: NbThemeService) {
  }

  ngOnInit(): void {

  }
}
