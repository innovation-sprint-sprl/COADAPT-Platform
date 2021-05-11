import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'

import { APIv1 } from './../../../constants';
@Component({
  selector: 'app-sub-administrators-create',
  templateUrl: './sub-administrators-create.component.html',
  styleUrls: ['./sub-administrators-create.component.scss']
})
export class SubAdministratorsCreateComponent implements OnInit {
  error = '';
  loading = false;

  createUserForm = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
  });


  constructor(private router: Router, private repositoryService: RepositoryService, private titleService: Title) {}

  ngOnInit(): void {
    this.titleService.setTitle('Create Sub-administrator | COADAPT');
  }

  onSubmit(): void {
    this.loading = true;
    this.repositoryService.postData(APIv1.subAdministrators, this.createUserForm.value).subscribe({
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
