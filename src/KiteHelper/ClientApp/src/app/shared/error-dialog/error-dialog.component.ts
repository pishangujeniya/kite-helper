import { Component, Input, OnInit } from '@angular/core';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.css'],
})
export class ErrorDialogComponent implements OnInit {

  @Input() public errorTitle: string;
  @Input() public errorMessage: string;

  constructor(private clipboard: Clipboard) { }

  ngOnInit(): void {
  }

  copyToClipboard(): void {
    this.clipboard.copy(this.errorMessage);
  }

}
