import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-internal-server',
  templateUrl: './internal-server.component.html',
  styleUrls: ['./internal-server.component.scss'],
})
export class InternalServerComponent implements OnInit {
  public errorMessage = 'Internal Server Error';
  public errorExplanation = 'PLEASE TRY AGAIN LATER';

  constructor() {}

  ngOnInit() {}
}
