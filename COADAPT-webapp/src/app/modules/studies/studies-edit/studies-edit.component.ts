import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { first } from 'rxjs/operators';
import { Organization, Study, COADAPTUser } from 'src/app/models';

import { RepositoryService } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-studies-edit',
  templateUrl: './studies-edit.component.html',
  styleUrls: ['./studies-edit.component.scss']
})
export class StudiesEditComponent implements OnInit {
  private route: ActivatedRouteSnapshot;

  public id;

  public error = '';
  public loading = false;
  public organizations: Organization[];
  public supervisors: COADAPTUser[];
  public selectedSupervisor;
  public selectedOrganization;

  public updateStudyForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    organizationId: new FormControl('', Validators.required),
    supervisorId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private repository: RepositoryService, private titleService: Title) {
    this.route = activatedRoute.snapshot;
  }

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

  public getStudy(id): void {
    let study: Study;
    this.repository.getData(`${APIv1.studies}/${id}`).subscribe((res) => {
      study = res as Study;
      this.updateStudyForm = new FormGroup({
        name: new FormControl(`${study.name}`, Validators.required),
        shortName: new FormControl(`${study.shortname}`, Validators.required),
        organizationId: new FormControl(`${study.organizationId}`, Validators.required),
        supervisorId: new FormControl(`${study.supervisorId}`, Validators.required),
      });
      this.selectedSupervisor = study.supervisorId;
      this.selectedOrganization = study.organizationId;
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Edit Study | COADAPT');
    this.getAllOrganizations();
    this.getAllSupervisors();
    this.id = this.route.paramMap.get('id')
    this.getStudy(this.id);
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.update(`${APIv1.studies}/${this.id}`, this.updateStudyForm.value).subscribe({
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
