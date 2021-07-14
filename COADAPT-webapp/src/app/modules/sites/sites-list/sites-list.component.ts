import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from './../../../services';

import { SiteList } from './../../../models';
import { APIv1 } from './../../../constants';

@Component({
  selector: 'app-sites-list',
  templateUrl: './sites-list.component.html',
  styleUrls: ['./sites-list.component.scss']
})
export class SitesListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  public displayedColumns: string[] = ['name', 'shortname', 'organization', 'study', 'participants', 'actions'];
  public dataSource: MatTableDataSource<SiteList>;

  constructor(private router: Router, private repository: RepositoryService, private snackBar: MatSnackBar, private titleService: Title) {}

  public getAllSites(): void {
    this.repository.getData(APIv1.sites).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as SiteList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public editSite(id): void {
    this.router.navigate(['sites/edit/' + id]);
  }

  public deleteSite(site: SiteList) {
    if (!confirm(`Delete ${site.name}?`)) return;
    this.repository.delete(`${APIv1.sites}/${site.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Sites | COADAPT');
    this.getAllSites();
  }
}
