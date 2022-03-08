import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { MatSort } from '@angular/material/sort';

import { Router } from '@angular/router';
import { RepositoryService } from '../../../services';

import { ParticipantList } from '../../../models';
import { APIv1 } from '../../../constants';

@Component({
  selector: 'app-individual-psychological',
  templateUrl: './individual-psychological.component.html',
  styleUrls: ['./individual-psychological.component.scss']
})
export class IndividualPsychologicalComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  public displayedColumns: string[] = ['code', 'therapist', 'organizations', 'studies', 'sites', 'groups', 'psychologicalReports'];
  public dataSource: MatTableDataSource<ParticipantList>;

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title) {}

  public getAllParticipants(): void {
    this.repository.getData(APIv1.participants + '/reports-only').subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as ParticipantList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public getParticipantReports(code): void {
    this.router.navigate(['individual/psychological/', code]);
  }

  ngOnInit(): void {
    this.titleService.setTitle('Individual Psychological | COADAPT');
    this.getAllParticipants();
  }

}
