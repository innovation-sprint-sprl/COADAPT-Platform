import { Component, OnInit } from '@angular/core';

import { AuthenticationService } from '../../../services';
import { LoginUser } from '../../../models';

import { HelperConstants } from '../../../constants';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnInit {
  currentUser: LoginUser;
  currentUserRole: string;
  version: string;

  constructor(private authenticationService: AuthenticationService) {
    this.authenticationService.currentUser.subscribe((x) => (this.currentUser = x));
    this.currentUserRole = (this.currentUser && this.currentUser.roles) ? this.currentUser.roles[0].replace('COADAPT.', '') : '';
  }

  ngOnInit(): void {
    this.version = HelperConstants.version;
  }
}
