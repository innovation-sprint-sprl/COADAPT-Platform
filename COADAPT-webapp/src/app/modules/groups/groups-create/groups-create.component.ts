import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { Study } from 'src/app/models';

import { RepositoryService } from '../../../services'

import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-groups-create',
  templateUrl: './groups-create.component.html',
  styleUrls: ['./groups-create.component.scss']
})
export class GroupsCreateComponent implements OnInit {
  public error = '';
  public loading = false;
  public studies: Study[];

  public createGroupForm = new FormGroup({
    name: new FormControl('', Validators.required),
    shortName: new FormControl('', Validators.required),
    studyId: new FormControl('', Validators.required),
  });

  constructor(private router: Router, private titleService: Title, private repository: RepositoryService) { }

  public getAllStudies(): void {
    this.repository.getData(APIv1.studies).subscribe((res) => {
      this.studies = (res as Study[]);
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Create Group | COADAPT');
    this.getAllStudies();
  }

  onSubmit(): void {
    this.loading = true;
    this.repository.postData(APIv1.groups, this.createGroupForm.value).subscribe({
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
