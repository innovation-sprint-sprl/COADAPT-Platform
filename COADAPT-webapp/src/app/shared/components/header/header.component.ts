import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

import { LoginUser } from '../../../models';
import { AuthenticationService } from '../../../services';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  @Output() toggleSideBarForMe: EventEmitter<any> = new EventEmitter();

  currentUser: LoginUser;
  currentUserRole: string;

  constructor(private router: Router, private authenticationService: AuthenticationService) {
    this.authenticationService.currentUser.subscribe((x) => (this.currentUser = x));
    this.currentUserRole = (this.currentUser && this.currentUser.roles) ? this.currentUser.roles[0].replace('COADAPT.', '') : '';
  }

  ngOnInit(): void { }

  toggleSideBar() {
    this.toggleSideBarForMe.emit();
    setTimeout(() => {
      window.dispatchEvent(new Event('resize'));
    }, 300);
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }
}
