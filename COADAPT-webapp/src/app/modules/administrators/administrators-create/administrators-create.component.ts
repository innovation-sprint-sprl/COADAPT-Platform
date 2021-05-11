import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'

import { APIv1 } from '../../../constants';

@Component({
  selector: 'app-administrators-create',
  templateUrl: './administrators-create.component.html',
  styleUrls: ['./administrators-create.component.scss'],
})
export class AdministratorsCreateComponent implements OnInit {
  error = '';
  loading = false;

  createUserForm = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
  });


  constructor(private router: Router, private titleService: Title, private repositoryService: RepositoryService) { }

  ngOnInit(): void {
    this.titleService.setTitle('Create Administrator | COADAPT');
  }

  onSubmit(): void {
    this.loading = true;
    this.repositoryService.postData(APIv1.administrators, this.createUserForm.value).subscribe({
      next: () => {
        this.router.navigate(['administrators']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }
}
