import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';

import { AuthenticationService } from '../services';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
  form: any = {};
  hide = true;
  loading = false;
  error = '';

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authenticationService: AuthenticationService
  ) {
    if (this.authenticationService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    document.body.className = 'login-bg';
  }

  ngOnDestroy(): void {
    document.body.className = '';
  }

  onSubmit(): void {
    this.loading = true;
    this.authenticationService.login(this.form.email, this.form.password).pipe(first()).subscribe({
      next: (user) => {
        if (user.roles[0] == 'COADAPT.Subadministrator') {
          this.router.navigate(['studies']);
        } else if (user.roles[0] == 'COADAPT.Supervisor') {
          this.router.navigate(['sites']);
        } else if (user.roles[0] == 'COADAPT.Therapist') {
          this.router.navigate(['participants']);
        } else if (user.roles[0] == 'COADAPT.Participant') {
          this.router.navigate(['individual/psychological']);
        } else {
          // get return url from route parameters or default to '/'
          const returnUrl = this.route.snapshot.queryParams.returnUrl || '/';
          this.router.navigate([returnUrl]);
        }
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
