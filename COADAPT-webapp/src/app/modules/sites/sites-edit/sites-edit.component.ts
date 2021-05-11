import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { first } from 'rxjs/operators';
import { Study, Site } from 'src/app/models';

import { RepositoryService } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-sites-edit',
  templateUrl: './sites-edit.component.html',
  styleUrls: ['./sites-edit.component.scss']
})
export class SitesEditComponent implements OnInit {
  private route: ActivatedRouteSnapshot;

  public id;

  public error = '';
  public loading = false;
  public studies: Study[];
  public selected;

  public updateSiteForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    studyId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private repository: RepositoryService, private titleService: Title) {
    this.route = activatedRoute.snapshot;
  }

  public getAllStudies(): void {
    this.repository.getData(APIv1.studies).subscribe((res) => {
      this.studies = (res as Study[]);
    });
  }

  public getSite(id): void {
    let site: Site;
    this.repository.getData(`${APIv1.sites}/${id}`).subscribe((res) => {
      site = res as Site;
      this.updateSiteForm = new FormGroup({
        name: new FormControl(`${site.name}`, Validators.required),
        shortName: new FormControl(`${site.shortname}`, Validators.required),
        studyId: new FormControl(`${site.studyId}`, Validators.required),
      });
      this.selected = site.studyId;
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Edit Site | COADAPT');
    this.getAllStudies();
    this.id = this.route.paramMap.get('id')
    this.getSite(this.id);
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.update(`${APIv1.sites}/${this.id}`, this.updateSiteForm.value).subscribe({
      next: () => {
        this.router.navigate(['sites']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
