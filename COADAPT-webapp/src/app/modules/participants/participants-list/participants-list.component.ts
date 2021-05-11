import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { ParticipantList } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-participants-list',
  templateUrl: './participants-list.component.html',
  styleUrls: ['./participants-list.component.scss']
})
export class ParticipantsListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  public displayedColumns: string[] = ['code', 'createdOn', 'therapist', 'organizations', 'studies', 'sites', 'groups', 'actions'];
  public dataSource: MatTableDataSource<ParticipantList>;

  constructor(private router: Router, private repository: RepositoryService, private snackBar: MatSnackBar, private titleService: Title) {}

  public getAllParticipants(): void {
    this.repository.getData(APIv1.participants).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as ParticipantList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public editParticipant(code): void {
    this.router.navigate(['participants/edit/' + code]);
  }

  public deleteParticipant(user: ParticipantList): void {
    console.log(user);
    if (!confirm(`Delete ${user.code}?`)) return;
    this.repository.delete(`${APIv1.participants}/${user.code}`).subscribe(
      (result) => { },
      (error) => {
        console.log(error);
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Participants | COADAPT');
    this.getAllParticipants();
  }
}
