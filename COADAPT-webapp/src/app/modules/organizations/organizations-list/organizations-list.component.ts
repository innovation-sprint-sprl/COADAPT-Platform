import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { Router } from '@angular/router';
import { RepositoryService } from '../../../services';

import { OrganizationList } from '../../../models';
import { APIv1 } from '../../../constants';

@Component({
  selector: 'app-organizations-list',
  templateUrl: './organizations-list.component.html',
  styleUrls: ['./organizations-list.component.scss'],
})
export class OrganizationsListComponent implements OnInit {
  // MatTable
  @ViewChild(MatPaginator) paginator: MatPaginator;
  public displayedColumns: string[] = [
    'name',
    'shortname',
    'subAdministrator',
    'studies',
    'participants',
    'actions',
  ];
  public dataSource: MatTableDataSource<OrganizationList>;

  constructor(private router: Router, private repository: RepositoryService, private titleService: Title, private snackBar: MatSnackBar) { }

  public getAllOrganizations(): void {
    this.repository.getData(APIv1.organizations).subscribe((res) => {
      this.dataSource = new MatTableDataSource(res as OrganizationList[]);
      this.paginator.pageSize = 10;
      this.dataSource.paginator = this.paginator;
    });
  }

  public editOrganization(id): void {
    this.router.navigate(['organizations/edit/' + id]);
  }

  public deleteOrganization(organization: OrganizationList) {
    if (!confirm(`Delete ${organization.name}?`)) return;
    this.repository.delete(`${APIv1.organizations}/${organization.id}`).subscribe(
      (result) => { },
      (error) => {
        this.snackBar.open(error, 'Dismiss', {
          duration: 2000,
        });
      }, () => { return location.reload(); }
    );
  }

  ngOnInit(): void {
    this.titleService.setTitle('Organizations | COADAPT');
    this.getAllOrganizations();
  }
}
