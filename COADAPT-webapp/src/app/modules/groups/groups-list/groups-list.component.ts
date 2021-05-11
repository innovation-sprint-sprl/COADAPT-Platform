import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { GroupList } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-groups-list',
  templateUrl: './groups-list.component.html',
  styleUrls: ['./groups-list.component.scss']
})
export class GroupsListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  public displayedColumns: string[] = ['name', 'shortname', 'organization', 'study',  'participants', 'actions'];
  public dataSource: MatTableDataSource<GroupList>;

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title, private snackBar: MatSnackBar) { }

  public getAllGroups(): void {
    this.repository.getData(APIv1.groups).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as GroupList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
    });
  }

  public editGroup(id): void {
    this.router.navigate(['groups/edit/' + id]);
  }

  public deleteGroup(group: GroupList) {
    if (!confirm(`Delete ${group.name}?`)) return;
    this.repository.delete(`${APIv1.groups}/${group.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Groups | COADAPT');
    this.getAllGroups();
  }
}
