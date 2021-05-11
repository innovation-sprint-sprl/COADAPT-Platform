import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'

import { APIv1 } from '../../../constants';
import { COADAPTUser, Site, Group } from './../../../models';

@Component({
  selector: 'app-participants-create',
  templateUrl: './participants-create.component.html',
  styleUrls: ['./participants-create.component.scss']
})
export class ParticipantsCreateComponent implements OnInit {
  public error = '';
  public loading = false;

  public therapists: COADAPTUser[];
  public sites: Site[];
  public groups: Group[];

  public createParticipantForm = new FormGroup({
    code: new FormControl('', [Validators.required]),
    userName: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, PasswordValidator.strong]),
    siteId: new FormControl('', []),
    groupId: new FormControl({value: '', disabled: true}),
    gender: new FormControl('', []),
    dateOfBirth: new FormControl('', []),
    dateOfFirstJob: new FormControl('', []),
    therapistId: new FormControl('', []),

    education: new FormControl('', []),
    region: new FormControl('', []),
    maritalStatus: new FormControl('', []),
    dateOfCurrentJob: new FormControl('', []),
    jobType: new FormControl('', []),
    startDate: new FormControl(new Date()),
    endDate: new FormControl('', [])
  });

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title) { }

  public getAllTherapists(): void {
    this.repository.getData(APIv1.therapists).subscribe((res) => {
      this.therapists = (res as COADAPTUser[]);
    });
  }

  public getAllSites(): void {
    this.repository.getData(APIv1.sites).subscribe((res) => {
      this.sites = (res as Site[]);
    });
  }

  public getGroupsOfSameStudy(studyId): void {
    this.repository.getData(APIv1.groups + '/study/' + studyId).subscribe((res) => {
      this.groups = (res as Group[]);
    });
  }

  public formatDate(dateField): void {
    const date = this.createParticipantForm.value[dateField];
    if (!date || date.length === 0 ) {
      return;
    }
    const offset = date.getTimezoneOffset();
    if (offset < 0) {
      date.setHours(12,0,0);
    }
    this.createParticipantForm.value[dateField] = date.toISOString().substring(0,10);
  }

  public formatFormDates(): void {
    Object.keys(this.createParticipantForm.controls).forEach(key => {
      if (key.toUpperCase().includes("DATE")) {
        this.formatDate(key);
      }
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Create Participant | COADAPT');
    this.getAllTherapists();
    this.getAllSites();
  }

  onSiteSelect(selectedSiteId): void {
    let studyId;
    this.sites.forEach(site => {
      if (site.id === selectedSiteId) {
        studyId = site.studyId;
      }
    });
    this.getGroupsOfSameStudy(studyId);
    this.createParticipantForm.controls['groupId'].enable();
  }
  
  onSubmit(): void {
    this.loading = true;
    this.formatFormDates();
    this.repository.postData(APIv1.participants, this.createParticipantForm.value).subscribe({
      next: () => {
        this.router.navigate(['participants']);
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
