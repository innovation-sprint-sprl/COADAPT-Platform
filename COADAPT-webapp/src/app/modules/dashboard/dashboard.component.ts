import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { Role } from '../../models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  public roles: typeof Role = Role;

  constructor(private titleService: Title) {}

  ngOnInit(): void {
    this.titleService.setTitle('Dashboard | COADAPT');
  }
}
