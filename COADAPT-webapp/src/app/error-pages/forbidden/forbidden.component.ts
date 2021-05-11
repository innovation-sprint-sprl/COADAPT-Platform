import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-forbidden',
  templateUrl: './forbidden.component.html',
  styleUrls: ['./forbidden.component.scss']
})
export class ForbiddenComponent implements OnInit {
  public errorMessage = 'Access Denied';
  public errorExplanation = 'Please log in with the proper credentials in order to view this page.';

  constructor() { }

  ngOnInit(): void {
  }

}
