import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { SubAdministratorList } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-sub-administrators-list',
  templateUrl: './sub-administrators-list.component.html',
  styleUrls: ['./sub-administrators-list.component.scss']
})
export class SubAdministratorsListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  public displayedColumns: string[] = ['userName', 'createdOn', 'organization', 'participants', 'actions'];
  public dataSource: MatTableDataSource<SubAdministratorList>;

  constructor(private router: Router, private repository: RepositoryService, private snackBar: MatSnackBar, private titleService: Title) {}

  public getAllSubAdmins(): void {
    this.repository.getData(APIv1.subAdministrators).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as SubAdministratorList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
    });
  }

  public editSubAdmin(id): void {
    this.router.navigate(['sub-administrators/edit/' + id]);
  }

  public deleteSubAdmin(user: SubAdministratorList): void {
    if (!confirm(`Delete ${user.userName}?`)) return;
    this.repository.delete(`${APIv1.subAdministrators}/${user.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Sub-administrators | COADAPT');
    this.getAllSubAdmins();
  }
}
