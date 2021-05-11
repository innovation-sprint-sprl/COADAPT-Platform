import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'

import { APIv1 } from '../../../constants';
import { COADAPTUser } from './../../../models';

@Component({
  selector: 'app-sub-administrators-edit',
  templateUrl: './sub-administrators-edit.component.html',
  styleUrls: ['./sub-administrators-edit.component.scss']
})
export class SubAdministratorsEditComponent implements OnInit {
  private route: ActivatedRouteSnapshot;

  public id;
  public error = '';
  public loading = false;

  public updateUserForm = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
  });

  public user: COADAPTUser;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private repository: RepositoryService, private titleService: Title) {
    this.route = activatedRoute.snapshot;
  }

  public getSubAdmin(id): void {
    let user: COADAPTUser;
    this.repository.getData(`${APIv1.subAdministrators}/${id}`).subscribe((res) => {
      user = res as COADAPTUser;
      this.updateUserForm = new FormGroup({
        userName: new FormControl(`${user.user.userName}`, [Validators.required, Validators.email]),
        password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
      });
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Edit Sub-administrator | COADAPT');
    this.id = this.route.paramMap.get('id');
    this.getSubAdmin(this.id);
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.update(`${APIv1.subAdministrators}/${this.id}`, this.updateUserForm.value).subscribe({
      next: () => {
        this.router.navigate(['sub-administrators']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
