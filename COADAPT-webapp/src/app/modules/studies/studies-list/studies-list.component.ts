import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { StudyList } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-studies-list',
  templateUrl: './studies-list.component.html',
  styleUrls: ['./studies-list.component.scss']
})
export class StudiesListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  public displayedColumns: string[] = ['name', 'shortname', 'organization', 'supervisor', 'sites', 'groups', 'participants', 'actions'];
  public dataSource: MatTableDataSource<StudyList>;

  constructor(private router: Router, private repository: RepositoryService, private snackBar: MatSnackBar, private titleService: Title) {}

  public getAllStudies(): void {
    this.repository.getData(APIv1.studies).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as StudyList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
    });
  }

  public editStudy(id): void {
    this.router.navigate(['studies/edit/' + id]);
  }

  public deleteStudy(study: StudyList) {
    if (!confirm(`Delete ${study.name}?`)) return;
    this.repository.delete(`${APIv1.studies}/${study.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Studies | COADAPT');
    this.getAllStudies();
  }
}
