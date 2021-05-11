import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { SupervisorList } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-supervisors-list',
  templateUrl: './supervisors-list.component.html',
  styleUrls: ['./supervisors-list.component.scss']
})
export class SupervisorsListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  public displayedColumns: string[] = ['userName', 'createdOn', 'organizations', 'studies', 'actions'];
  public dataSource: MatTableDataSource<SupervisorList>;

  constructor(private router: Router, private repository: RepositoryService, private snackBar: MatSnackBar, private titleService: Title) { }

  public getAllSupervisors(): void {
    this.repository.getData(APIv1.supervisors).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as SupervisorList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
    });
  }

  public editSupervisor(id): void {
    this.router.navigate(['supervisors/edit/' + id]);
  }

  public deleteSupervisor(user: SupervisorList): void {
    if (!confirm(`Delete ${user.userName}?`)) return;
    this.repository.delete(`${APIv1.supervisors}/${user.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Supervisors | COADAPT');
    this.getAllSupervisors();
  }
}
