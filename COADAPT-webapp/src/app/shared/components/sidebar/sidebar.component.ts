import { Component, OnInit } from '@angular/core';

import { HelperConstants } from '../../../constants';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnInit {
  version: string;

  constructor() {}

  ngOnInit(): void {
    this.version = HelperConstants.version;
  }
}
