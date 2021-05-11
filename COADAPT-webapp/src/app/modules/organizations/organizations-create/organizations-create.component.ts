import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { COADAPTUser } from 'src/app/models';

import { RepositoryService } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-organizations-create',
  templateUrl: './organizations-create.component.html',
  styleUrls: ['./organizations-create.component.scss']
})
export class OrganizationsCreateComponent implements OnInit {
  error = '';
  loading = false;
  subAdmins: COADAPTUser[];

  createOrganizationForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    subAdministratorId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title) { }

  public getAllSubAdmins(): void {
    this.repository.getData(APIv1.subAdministrators).subscribe((res) => {
      this.subAdmins = (res as COADAPTUser[]);
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Create Organization | COADAPT');
    this.getAllSubAdmins();
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.postData(APIv1.organizations, this.createOrganizationForm.value).subscribe({
      next: () => {
        this.router.navigate(['organizations']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
