import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { Organization, COADAPTUser } from 'src/app/models';

import { RepositoryService } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-studies-create',
  templateUrl: './studies-create.component.html',
  styleUrls: ['./studies-create.component.scss']
})
export class StudiesCreateComponent implements OnInit {
  public error = '';
  public loading = false;
  public organizations: Organization[];
  public supervisors: COADAPTUser[];

  public createStudyForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    organizationId: new FormControl('', Validators.required),
    supervisorId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title) { }

  public getAllOrganizations(): void {
    this.repository.getData(APIv1.organizations).subscribe((res) => {
      this.organizations = (res as Organization[]);
    });
  }

  public getAllSupervisors(): void {
    this.repository.getData(APIv1.supervisors).subscribe((res) => {
      this.supervisors = (res as COADAPTUser[]);
    });
  }

  ngOnInit(): void {
    this.getAllOrganizations();
    this.titleService.setTitle('Create Study | COADAPT');
    this.getAllSupervisors();
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.postData(APIv1.studies, this.createStudyForm.value).subscribe({
      next: () => {
        this.router.navigate(['studies']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
