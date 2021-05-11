import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';

import { first } from 'rxjs/operators';

import { RepositoryService, PasswordValidator } from '../../../services'
import { APIv1 } from '../../../constants';
import { COADAPTUser, Site, Group, ParticipantEdit } from './../../../models';

@Component({
  selector: 'app-participants-edit',
  templateUrl: './participants-edit.component.html',
  styleUrls: ['./participants-edit.component.scss']
})
export class ParticipantsEditComponent implements OnInit {
  public error = '';
  public loading = false;

  private route: ActivatedRouteSnapshot;
  public therapists: COADAPTUser[];
  public sites: Site[];
  public newGroups: Group[] = [];
  public groupsByParticipation: Group[][] = [];
  public selectedSites : number[] = [];
  public selectedGroups : number[] = [];
  public selectedTherapist?: number;

  public code;
  public participant;
  public participations: number;
  public participationStatuses: boolean[] = [];
  public isFormDisplayed: boolean = false;

  public updateParticipantForm = new FormGroup({
    code: new FormControl('', [Validators.required]),
    userName: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, PasswordValidator.strong]),
    gender: new FormControl('', []),
    dateOfBirth: new FormControl('', []),
    dateOfFirstJob: new FormControl('', []),
    therapistId: new FormControl('', []),
  });

  public updateStudyParticipantForm = new FormGroup({
    studyParticipantForms: new FormArray([])
  });

  public addStudyParticipantForm = new FormGroup({
    siteId: new FormControl('', []),
    groupId: new FormControl({value: '', disabled: true}),
    education: new FormControl('', []),
    region: new FormControl('', []),
    maritalStatus: new FormControl('', []),
    dateOfCurrentJob: new FormControl('', []),
    jobType: new FormControl('', []),
    startDate: new FormControl(new Date()),
    endDate: new FormControl('', [])
  });

  public finalUpdateForm = new FormGroup({
    userName: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, PasswordValidator.strong]),
    gender: new FormControl('', []),
    dateOfBirth: new FormControl('', []),
    dateOfFirstJob: new FormControl('', []),
    therapistId: new FormControl('', []),
    siteId: new FormControl('', []),
    groupId: new FormControl('', []),
    education: new FormControl('', []),
    region: new FormControl('', []),
    maritalStatus: new FormControl('', []),
    dateOfCurrentJob: new FormControl('', []),
    jobType: new FormControl('', []),
    startDate: new FormControl('', []),
    endDate: new FormControl('', [])
  });

  public getAllForms() : FormArray {
    return this.updateStudyParticipantForm.get("studyParticipantForms") as FormArray;
  }

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private repository: RepositoryService, private titleService: Title) {
    this.route = activatedRoute.snapshot;
  }

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

  public getGroupsOfSameStudyForCreationForm(studyId): void {
    this.repository.getData(APIv1.groups + '/study/' + studyId).subscribe((res) => {
      this.newGroups = (res as Group[]);
    });
  }

  public getGroupsOfSameStudy(studyId): void {
    this.repository.getData(APIv1.groups + '/study/' + studyId).subscribe((res) => {
      this.groupsByParticipation.push(res as Group[]);
    });
  }

  public getParticipant(code): void {
    let participant: ParticipantEdit;
    this.repository.getData(`${APIv1.participants}/${code}`).subscribe((res) => {
      participant = res as ParticipantEdit;
      this.participations = participant.studyParticipants.length;
      this.participant = participant;
      this.selectedTherapist = participant.therapistId;
      this.updateParticipantForm = new FormGroup({
        code: new FormControl(`${participant.code}`, Validators.required),
        userName: new FormControl(`${participant.user.userName}`, Validators.required),
        password: new FormControl(``, [Validators.required, PasswordValidator.strong]),
        gender: new FormControl(`${participant.gender}`, Validators.required),
        dateOfBirth: new FormControl(`${participant.dateOfBirth}`, Validators.required),
        dateOfFirstJob: new FormControl(`${participant.dateOfFirstJob}`, Validators.required),
        therapistId: new FormControl(`${participant.therapistId}`, Validators.required)
      });
      this.updateParticipantForm.get('code').disable();

      for (var i = 0; i < this.participations; i++) {
        this.getAllForms().push(new FormGroup({
          siteId: new FormControl(`${participant.studyParticipants[i]?.siteId}`, Validators.required),
          groupId: new FormControl(`${participant.studyParticipants[i]?.groupId}`, Validators.required),
          education: new FormControl(`${participant.studyParticipants[i]?.education}`, []),
          region: new FormControl(`${participant.studyParticipants[i]?.regionOfResidence}`, []),
          maritalStatus: new FormControl(`${participant.studyParticipants[i]?.maritalStatus}`, []),
          dateOfCurrentJob: new FormControl(`${participant.studyParticipants[i]?.dateOfCurrentJob}`, []),
          jobType: new FormControl(`${participant.studyParticipants[i]?.jobType}`, []),
          startDate: new FormControl(`${participant.studyParticipants[i]?.startDate}`, []),
          endDate: new FormControl(`${participant.studyParticipants[i]?.endDate}`, [])
        }));
      
        this.selectedSites[i] = participant.studyParticipants[i]?.siteId;
        this.getAllForms().controls[i].get('siteId').setValue(this.selectedSites[i]);
        this.onSiteSelect(this.selectedSites[i]);

        this.selectedGroups[i] = participant.studyParticipants[i]?.groupId;
        this.getAllForms().controls[i].get('groupId').setValue(this.selectedGroups[i]);
      
        this.participationStatuses[i] = participant.studyParticipants[i]?.abandoned;
        this.getAllForms().controls[i].get('siteId').disable();
        this.getAllForms().controls[i].get('groupId').disable();
      }

      setTimeout(() => {
        this.checkAbandonedParticipations();
      });
    });
  }

  public checkAbandonedParticipations(): void {
    for (var i = 0; i < this.participations; i++) {
      if (this.participant.studyParticipants[i]?.abandoned) {
        this.getAllForms().controls[i].get('education').disable();
        this.getAllForms().controls[i].get('region').disable();
        this.getAllForms().controls[i].get('maritalStatus').disable();
        this.getAllForms().controls[i].get('dateOfCurrentJob').disable();
        this.getAllForms().controls[i].get('jobType').disable();
        this.getAllForms().controls[i].get('startDate').disable();
        this.getAllForms().controls[i].get('endDate').disable();
      }
    }
  }

  //date value that arrives from the backend is already in ISOString format. Therefore, this should return true.
  //if the value was edited by the user, it will be in UTC, therefore this will return false and will be formatted accordingly
  public isIsoDate(date) {
    if (typeof date === 'string' || date instanceof String) return true;
    var d = new Date(date); 
    return d.toISOString()===date;
  }

  public formatParticipantFormDate(dateField): void {
    const date = this.updateParticipantForm.value[dateField];
    if (!date || date.length === 0 || this.isIsoDate(date)) {
      return;
    }
    const offset = date.getTimezoneOffset();
    if (offset < 0) {
      date.setHours(12,0,0);
    }
    this.updateParticipantForm.value[dateField] = date.toISOString().substring(0,10);
  }

  public formatStudyParticipantFormDate(dateField, activeForm): void {
    const date = this.getAllForms().controls[activeForm].get(dateField).value;
    if (!date || date.length === 0 || this.isIsoDate(date)) {
      return;
    }
    const offset = date.getTimezoneOffset();
    if (offset < 0) {
      date.setHours(12,0,0);
    }
    this.getAllForms().controls[activeForm].get(dateField).setValue(date.toISOString().substring(0,10));
  }

  public formatCreationFormDate(dateField): void {
    const date = this.addStudyParticipantForm.value[dateField];
    if (!date || date.length === 0 || this.isIsoDate(date)) {
      return;
    }
    const offset = date.getTimezoneOffset();
    if (offset < 0) {
      date.setHours(12,0,0);
    }
    this.addStudyParticipantForm.value[dateField] = date.toISOString().substring(0,10);
  }

  public formatCreationFormDates(): void {
    Object.keys(this.addStudyParticipantForm.controls).forEach(key => {
      if (key.toUpperCase().includes("DATE")) {
        this.formatCreationFormDate(key);
      }
    });
  }

  public formatParticipantFormDates(): void {
    Object.keys(this.updateParticipantForm.controls).forEach(key => {
      if (key.toUpperCase().includes("DATE")) {
        this.formatParticipantFormDate(key);
      }
    });
  }

  public formatStudyParticipantFormDates(activeForm): void {
    Object.keys(this.getAllForms().at(activeForm)['controls']).forEach(key => {
      if (key.toUpperCase().includes("DATE")) {
        this.formatStudyParticipantFormDate(key, activeForm);
      }
    });
  }

  ngOnInit(): void {
    this.titleService.setTitle('Edit Participant | COADAPT');
    this.code = this.route.paramMap.get('code');
    this.getAllTherapists();
    this.getAllSites();
    this.getParticipant(this.code);
  }

  onCreationFormSiteSelect(selectedSiteId): void {
    let studyId;
    this.sites.forEach(site => {
      if (site.id === selectedSiteId) {
        studyId = site.studyId;
      }
    });
    this.getGroupsOfSameStudyForCreationForm(studyId);
    if (this.addStudyParticipantForm.controls['groupId'].disabled) {
      this.addStudyParticipantForm.controls['groupId'].enable();
    }
  }

  onSiteSelect(selectedSiteId): void {
    let studyId;
    this.sites.forEach(site => {
      if (site.id === selectedSiteId) {
        studyId = site.studyId;
      }
    });
    this.getGroupsOfSameStudy(studyId);
  }

  onAbandon(activeForm): void {
    this.loading = true;
    if (!confirm(`Are you sure you want to abandon this participation?`)) return;
    this.repository.update(`${APIv1.participants}/${this.code}/endFromSiteGroup/${this.getAllForms().controls[activeForm].get('siteId').value}/${this.getAllForms().controls[activeForm].get('groupId').value}`, this.updateParticipantForm.value).subscribe({
      next: () => {
        location.reload();
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

  onToggleForm(): void {
    this.isFormDisplayed = !this.isFormDisplayed;
  }
  /*
    buildCreateObject and buildUpdateObject check for values equal to the string 'null', 
    because some participants and studyparticipants will return 'null' for a few of their fields from the backend.
    If they are equal, these functions convert them to null values(not 'null').
    This overcomes the issue of the update request failing for such participants/studyparticipants
  */
  public buildCreateObject(): void{
    this.finalUpdateForm.value['userName'] = this.updateParticipantForm.value['userName'];
    this.finalUpdateForm.value['password'] = this.updateParticipantForm.value['password'];
    this.finalUpdateForm.value['gender'] = this.updateParticipantForm.value['gender'];
    this.finalUpdateForm.value['dateOfBirth'] = (this.updateParticipantForm.value['dateOfBirth'] === 'null') ? null : this.updateParticipantForm.value['dateOfBirth'];
    this.finalUpdateForm.value['dateOfFirstJob'] = (this.updateParticipantForm.value['dateOfFirstJob'] === 'null') ? null : this.updateParticipantForm.value['dateOfFirstJob'];
    this.finalUpdateForm.value['therapistId'] = (this.updateParticipantForm.value['therapistId'] === 'null') ? null : this.updateParticipantForm.value['therapistId'];
    this.finalUpdateForm.value['siteId'] = this.addStudyParticipantForm.value['siteId'];
    this.finalUpdateForm.value['groupId'] = this.addStudyParticipantForm.value['groupId'];
    this.finalUpdateForm.value['education'] = this.addStudyParticipantForm.value['education'];
    this.finalUpdateForm.value['region'] = this.addStudyParticipantForm.value['region'];
    this.finalUpdateForm.value['maritalStatus'] = this.addStudyParticipantForm.value['maritalStatus'];
    this.finalUpdateForm.value['dateOfCurrentJob'] = this.addStudyParticipantForm.value['dateOfCurrentJob'];
    this.finalUpdateForm.value['jobType'] = this.addStudyParticipantForm.value['jobType'];
    this.finalUpdateForm.value['startDate'] = this.addStudyParticipantForm.value['startDate'];
    this.finalUpdateForm.value['endDate'] = this.addStudyParticipantForm.value['endDate'];
  }

  onJoinStudy(): void {
    this.loading = true;
    this.formatParticipantFormDates();
    this.formatCreationFormDates();
    this.buildCreateObject();
    this.repository.update(`${APIv1.participants}/${this.code}`, this.finalUpdateForm.value).subscribe({
      next: () => {
        location.reload();
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

  public buildUpdateObject(activeForm): void{
    this.finalUpdateForm.value['userName'] = this.updateParticipantForm.value['userName'];
    this.finalUpdateForm.value['password'] = this.updateParticipantForm.value['password'];
    this.finalUpdateForm.value['gender'] = this.updateParticipantForm.value['gender'];
    this.finalUpdateForm.value['dateOfBirth'] = (this.updateParticipantForm.value['dateOfBirth'] === 'null') ? null : this.updateParticipantForm.value['dateOfBirth'];
    this.finalUpdateForm.value['dateOfFirstJob'] = (this.updateParticipantForm.value['dateOfFirstJob'] === 'null') ? null : this.updateParticipantForm.value['dateOfFirstJob'];
    this.finalUpdateForm.value['therapistId'] = (this.updateParticipantForm.value['therapistId'] === 'null') ? null : this.updateParticipantForm.value['therapistId'];
    this.finalUpdateForm.value['siteId'] = this.getAllForms().controls[activeForm].get('siteId').value;
    this.finalUpdateForm.value['groupId'] = this.getAllForms().controls[activeForm].get('groupId').value;
    this.finalUpdateForm.value['education'] = this.getAllForms().controls[activeForm].get('education').value;
    this.finalUpdateForm.value['region'] = this.getAllForms().controls[activeForm].get('region').value;
    this.finalUpdateForm.value['maritalStatus'] = this.getAllForms().controls[activeForm].get('maritalStatus').value;
    this.finalUpdateForm.value['dateOfCurrentJob'] = (this.getAllForms().controls[activeForm].get('dateOfCurrentJob').value === 'null') ? null : this.getAllForms().controls[activeForm].get('dateOfCurrentJob').value;
    this.finalUpdateForm.value['jobType'] = this.getAllForms().controls[activeForm].get('jobType').value;
    this.finalUpdateForm.value['startDate'] = (this.getAllForms().controls[activeForm].get('startDate').value === 'null') ? null : this.getAllForms().controls[activeForm].get('startDate').value;
    this.finalUpdateForm.value['endDate'] = (this.getAllForms().controls[activeForm].get('endDate').value === 'null') ? null : this.getAllForms().controls[activeForm].get('endDate').value;
  }

  onSubmit(activeForm): void {
    this.loading = true;
    this.formatParticipantFormDates();
    this.formatStudyParticipantFormDates(activeForm);
    this.buildUpdateObject(activeForm);
    this.repository.update(`${APIv1.participants}/${this.code}`, this.finalUpdateForm.value).subscribe({
      next: () => {
        location.reload();
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      },
    });
  }

}
