import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-supervisors-create',
  templateUrl: './supervisors-create.component.html',
  styleUrls: ['./supervisors-create.component.scss']
})
export class SupervisorsCreateComponent implements OnInit {
  error = '';
  loading = false;

  createUserForm = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
  });


  constructor(private router: Router, private repositoryService: RepositoryService, private titleService: Title) {}

  ngOnInit(): void {
    this.titleService.setTitle('Create Supervisor | COADAPT');
  }

  onSubmit(): void {
    this.loading = true;
    this.repositoryService.postData(APIv1.supervisors, this.createUserForm.value).subscribe({
      next: () => {
        this.router.navigate(['supervisors']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }
}
