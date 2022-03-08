import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatPaginator } from '@angular/material/paginator';

import { DashboardService } from '../../dashboard.service';
import { Router } from '@angular/router';
import { RepositoryService } from 'src/app/services';
import { MatTableDataSource } from '@angular/material/table';
import { ParticipantList } from 'src/app/models';
import { MatSort } from '@angular/material/sort';
import { APIv1 } from 'src/app/constants';

@Component({
  selector: 'app-individual-physiological',
  templateUrl: './individual-physiological.component.html',
  styleUrls: ['./individual-physiological.component.scss']
})
export class IndividualPhysiologicalComponent implements OnInit {

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  public displayedColumns: string[] = ['code', 'therapist', 'organizations', 'studies', 'sites', 'groups', 'phychologicalMetrics'];
  public dataSource: MatTableDataSource<ParticipantList>;

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title) { }

  ngOnInit() {
    this.titleService.setTitle('Individual Physiological | COADAPT');
    this.repository.getData(APIv1.participants + '/metrics-only').subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as ParticipantList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getParticipantMetrics(code): void {
    this.router.navigate(['individual/physiological/', code]);
  }

}
