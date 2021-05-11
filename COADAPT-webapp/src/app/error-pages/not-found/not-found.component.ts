import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss'],
})
export class NotFoundComponent implements OnInit {
  public errorMessage = 'Not Found';
  public errorExplanation = 'THIS PAGE DOES NOT EXIST';

  constructor() {}

  ngOnInit(): void {}
}
