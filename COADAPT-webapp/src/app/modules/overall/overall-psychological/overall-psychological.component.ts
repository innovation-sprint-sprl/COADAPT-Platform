import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { DashboardService } from '../../dashboard.service';
import { PeriodicElement } from '../../../models'

import { RepositoryService } from './../../../services';
import { APIv1 } from '../../../constants';
import { PsychologicalReport } from '../../../models';

import { ChartDataSets, ChartOptions, ChartType  } from 'chart.js';
import { Color, Label } from 'ng2-charts';

const ELEMENT_DATA: PeriodicElement[] = [
  { position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H' },
  { position: 2, name: 'Helium', weight: 4.0026, symbol: 'He' },
  { position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li' },
  { position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be' },
  { position: 5, name: 'Boron', weight: 10.811, symbol: 'B' },
  { position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C' },
  { position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N' },
  { position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O' },
  { position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F' },
  { position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne' },
  { position: 11, name: 'Sodium', weight: 22.9897, symbol: 'Na' },
  { position: 12, name: 'Magnesium', weight: 24.305, symbol: 'Mg' },
  { position: 13, name: 'Aluminum', weight: 26.9815, symbol: 'Al' },
  { position: 14, name: 'Silicon', weight: 28.0855, symbol: 'Si' },
  { position: 15, name: 'Phosphorus', weight: 30.9738, symbol: 'P' },
  { position: 16, name: 'Sulfur', weight: 32.065, symbol: 'S' },
  { position: 17, name: 'Chlorine', weight: 35.453, symbol: 'Cl' },
  { position: 18, name: 'Argon', weight: 39.948, symbol: 'Ar' },
  { position: 19, name: 'Potassium', weight: 39.0983, symbol: 'K' },
  { position: 20, name: 'Calcium', weight: 40.078, symbol: 'Ca' },
];

@Component({
  selector: 'app-overall-psychological',
  templateUrl: './overall-psychological.component.html',
  styleUrls: ['./overall-psychological.component.scss']
})
export class OverallPsychologicalComponent implements OnInit {

  public lineChartData: ChartDataSets[] = [
    { data: [85, 72, 78, 75, 77, 75], label: 'Depression' },
  ];

  public lineChartLabels: Label[] = ['0', '20', '40', '60', '80', '100'];

  public lineChartOptions = {
    responsive: true,
  };

  public lineChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,255,0,0.28)',
    },
  ];

  public lineChartLegend = true;
  public lineChartPlugins = [];
  public lineChartType = 'line';
  public barChartOptions: ChartOptions = {
    responsive: true,
  };
  public barChartLabels: Label[] = ['Excellent sleep', 'Good sleep', 'Average sleep', 'Poor sleep', 'Extremely poor sleep'];
  public barChartType: ChartType = 'bar';
  public barChartLegend = true;
  public barChartPlugins = [];

  public barChartData: ChartDataSets[] = [
    { data: [40, 60, 30, 30, 30], label: '% of participants with sleep issues' }
  ];

  public barGroups: number[] = [0, 0, 0, 0, 0];
  cards = [];
  pieChart = [];

  stats = [];

  displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
  dataSource = new MatTableDataSource<PeriodicElement>(ELEMENT_DATA);

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private dashboardService: DashboardService, private repository: RepositoryService, private titleService: Title) { }

  ngOnInit() {
    this.titleService.setTitle('Overall Psychological | COADAPT');
    this.paginator.pageSize = 10;
    
    this.dataSource.paginator = this.paginator;
    this.setCharts();
  }

  public setCharts(): void {
    this.repository.getData(APIv1.psychologicalReports).subscribe((res) => {
      this.stats = res as PsychologicalReport[];
      this.lineChartData['0']['data'][0] = 100;

      var totalCount = 0;
      this.stats.forEach(element => {
        if (element.sleepProblem < 20){
          this.barGroups[0]++;
        }
        else if (element.sleepProblem < 40){
          this.barGroups[1]++;
        }
        else if (element.sleepProblem < 60){
          this.barGroups[2]++;
        }
        else if (element.sleepProblem < 80){
          this.barGroups[3]++;
        }
        else if (element.sleepProblem < 100){
          this.barGroups[4]++;
        }
        totalCount++;
      });
      for (var i = 0; i < this.barChartData['0']['data'].length; i++) {
        this.barChartData['0']['data'][i] = (this.barGroups[i]/totalCount) * 100;
      }

      this.cards = this.dashboardService.cards();
      this.pieChart = this.dashboardService.pieChart();
    });
  }

}
