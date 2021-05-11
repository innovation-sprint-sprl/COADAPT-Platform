import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { first } from 'rxjs/operators';
import { Study, Group } from 'src/app/models';

import { RepositoryService } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-groups-edit',
  templateUrl: './groups-edit.component.html',
  styleUrls: ['./groups-edit.component.scss']
})
export class GroupsEditComponent implements OnInit {
  private route: ActivatedRouteSnapshot;

  public id;

  public error = '';
  public loading = false;
  public studies: Study[];
  public selected;

  public updateGroupForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    studyId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private titleService: Title, private activatedRoute: ActivatedRoute, private repository: RepositoryService) {
    this.route = activatedRoute.snapshot;
  }

  public getAllStudies(): void {
    this.repository.getData(APIv1.studies).subscribe((res) => {
      this.studies = (res as Study[]);
    });
  }

  public getGroup(id): void {
    let group: Group;
    this.repository.getData(`${APIv1.groups}/${id}`).subscribe((res) => {
      group = res as Group;
      this.updateGroupForm = new FormGroup({
        name: new FormControl(`${group.name}`, Validators.required),
        shortName: new FormControl(`${group.shortname}`, Validators.required),
        studyId: new FormControl(`${group.studyId}`, Validators.required),
      });
      this.selected = group.studyId;
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Edit Group | COADAPT');
    this.getAllStudies();
    this.id = this.route.paramMap.get('id')
    this.getGroup(this.id);
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.update(`${APIv1.groups}/${this.id}`, this.updateGroupForm.value).subscribe({
      next: () => {
        this.router.navigate(['groups']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
