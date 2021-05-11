import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService } from '../../../services'

import { APIv1 } from '../../../constants';
import { COADAPTUser, Organization } from './../../../models';

@Component({
  selector: 'app-organizations-edit',
  templateUrl: './organizations-edit.component.html',
  styleUrls: ['./organizations-edit.component.scss']
})
export class OrganizationsEditComponent implements OnInit {
  private route: ActivatedRouteSnapshot;

  public id;

  public error = '';
  public loading = false;
  public subAdmins: COADAPTUser[];
  public selected;

  updateOrganizationForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    subAdministratorId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private titleService: Title, private activatedRoute: ActivatedRoute, private repository: RepositoryService) {
    this.route = activatedRoute.snapshot;
  }

  public getAllSubAdmins(): void {
    this.repository.getData(APIv1.subAdministrators).subscribe((res) => {
      this.subAdmins = (res as COADAPTUser[]);
    });
  }

  public getOrganization(id): void {
    let organization: Organization;
    this.repository.getData(`${APIv1.organizations}/${id}`).subscribe((res) => {
      organization = res as Organization;
      this.updateOrganizationForm = new FormGroup({
        name: new FormControl(`${organization.name}`, Validators.required),
        shortName: new FormControl(`${organization.shortname}`, Validators.required),
        subAdministratorId: new FormControl(`${organization.subAdministratorId}`, Validators.required),
      });
      this.selected = organization.subAdministratorId;
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Edit Organization | COADAPT');
    this.getAllSubAdmins();
    this.id = this.route.paramMap.get('id')
    this.getOrganization(this.id);
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.update(`${APIv1.organizations}/${this.id}`, this.updateOrganizationForm.value).subscribe({
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
