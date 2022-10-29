import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss'],
})
export class LogoutComponent implements OnInit {

  constructor(
  ) {
  }

  ngOnInit(): void {
    this.logout();
  }

  private logout(): void {
    localStorage.clear();
    sessionStorage.clear();
  }
}
