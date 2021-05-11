import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { PageTitleService } from '../../services';

@Component({
  selector: 'app-default',
  templateUrl: './default.component.html',
  styleUrls: ['./default.component.scss'],
})
export class DefaultComponent implements OnInit {
  pageTitle = '';
  sideBarOpen = true;

  constructor(private pageTitleService: PageTitleService) { }

  ngOnInit(): void {
    this.pageTitle = this.pageTitleService.getTitle();
  }

  sideBarToggler(): void {
    this.sideBarOpen = !this.sideBarOpen;
  }
}
