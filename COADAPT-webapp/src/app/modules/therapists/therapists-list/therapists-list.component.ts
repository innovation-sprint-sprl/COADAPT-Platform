import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { COADAPTUser } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-therapists-list',
  templateUrl: './therapists-list.component.html',
  styleUrls: ['./therapists-list.component.scss']
})
export class TherapistsListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  public displayedColumns: string[] = ['userName', 'createdOn', 'participants', 'actions'];
  public dataSource: MatTableDataSource<COADAPTUser>;

  constructor(private router: Router, private repository: RepositoryService, private snackBar: MatSnackBar, private titleService: Title) {}

  public getAllTherapists(): void {
    this.repository.getData(APIv1.therapists).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as COADAPTUser[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public editTherapist(id): void {
    this.router.navigate(['therapists/edit/' + id]);
  }

  public deleteTherapist(user: COADAPTUser): void {
    if (!confirm(`Delete ${user.userName}?`)) return;
    this.repository.delete(`${APIv1.therapists}/${user.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Therapists | COADAPT');
    this.getAllTherapists();
  }
}
