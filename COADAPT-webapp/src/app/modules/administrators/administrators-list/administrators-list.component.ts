import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { COADAPTUser } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-administrators-list',
  templateUrl: './administrators-list.component.html',
  styleUrls: ['./administrators-list.component.scss']
})
export class AdministratorsListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  public displayedColumns: string[] = ['userName', 'createdOn', 'actions'];
  public dataSource: MatTableDataSource<COADAPTUser>;

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title, private snackBar: MatSnackBar) { }

  public getAllAdmins(): void {
    this.repository.getData(APIv1.administrators).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as COADAPTUser[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public editAdmin(id): void {
    this.router.navigate(['administrators/edit/' + id]);
  }

  public deleteAdmin(user: COADAPTUser): void {
    if (!confirm(`Delete ${user.userName}?`)) return;
    this.repository.delete(`${APIv1.administrators}/${user.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Administrators | COADAPT');
    this.getAllAdmins();
  }

}
