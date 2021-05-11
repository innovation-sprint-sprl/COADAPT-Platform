import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-therapists-create',
  templateUrl: './therapists-create.component.html',
  styleUrls: ['./therapists-create.component.scss']
})
export class TherapistsCreateComponent implements OnInit {
  error = '';
  loading = false;

  createUserForm = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
  });


  constructor(private router: Router, private repository: RepositoryService, private titleService: Title) {}

  ngOnInit(): void {
    this.titleService.setTitle('Create Therapist | COADAPT');
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.postData(APIv1.therapists, this.createUserForm.value).subscribe({
      next: () => {
        this.router.navigate(['therapists']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }
}
