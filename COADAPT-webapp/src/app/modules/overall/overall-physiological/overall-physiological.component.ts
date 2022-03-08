import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ChartDataSets } from 'chart.js';
import { BaseChartDirective, Color, Label } from 'ng2-charts';
import { APIv1 } from 'src/app/constants';
import { ParticipantList } from 'src/app/models';
import { OuraActivity } from 'src/app/models/oura-activity';
import { OuraReadiness } from 'src/app/models/oura-readiness';
import { OuraSleep } from 'src/app/models/oura-sleep';
import { RepositoryService } from 'src/app/services';

import { DashboardService } from '../../dashboard.service';

@Component({
  selector: 'app-overall-physiological',
  templateUrl: './overall-physiological.component.html',
  styleUrls: ['./overall-physiological.component.scss']
})
export class OverallPhysiologicalComponent implements OnInit {

  @ViewChildren(BaseChartDirective) charts: QueryList<BaseChartDirective>;
  
  constructor(private dashboardService: DashboardService, private titleService: Title, private repository: RepositoryService) { }

  /* Steps settings */
  public stepsChartData: ChartDataSets[] = [
    { data: [0, 0, 0, 0, 0, 0, 0], label: 'Steps - Average', stack: 'a' },
  ];

  public stepsChartLabels: Label[] = ['Mon', 'Tue', 'Wen', 'Thu', 'Fr', 'Sut', 'Sun'];

  public stepsChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value;
          }
        }
      }]
    }
  };

  public stepsChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,0,0,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,176,240,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,0,0,0.28)'
    }
  ];

  public stepsChartLegend = true;
  public stepsChartPlugins = [];
  public stepsChartType = 'bar';

  /* Settings for calories */
  public caloriesChartData: ChartDataSets[] = [ { data: [0, 0, 0, 0, 0, 0, 0], label: 'Calories - Average', stack: 'a' } ];
  public caloriesChartLabels: Label[] = ['Mon', 'Tue', 'Wen', 'Thu', 'Fri', 'Sut', 'Sun'];
  public caloriesChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value;
          }
        }
      }]
    }
  };
  public caloriesChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,0,0,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,176,240,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,0,0,0.28)'
    }
  ];
  public caloriesChartLegend = true;
  public caloriesChartPlugins = [];
  public caloriesChartType = 'bar';

  /* Settings for readinesses */
  public readinessesChartData: ChartDataSets[] = [ { data: [0, 0, 0, 0, 0, 0, 0], label: 'Score - Average', stack: 'a' } ];
  public readinessesChartLabels: Label[] = ['Mon', 'Tue', 'Wen', 'Thu', 'Fri', 'Sut', 'Sun'];
  public readinessesChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value;
          }
        }
      }]
    }
  };
  public readinessesChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,0,0,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,176,240,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,0,0,0.28)'
    }
  ];
  public readinessesChartLegend = true;
  public readinessesChartPlugins = [];
  public readinessesChartType = 'bar';

  /* Settings for sleep */
  public sleepChartData: ChartDataSets[] = [ { data: [0, 0, 0, 0, 0, 0, 0], label: 'Sleep - Average', stack: 'a' } ];
  public sleepChartLabels: Label[] = ['Mon', 'Tue', 'Wen', 'Thu', 'Fri', 'Sut', 'Sun'];
  public sleepChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value;
          }
        }
      }]
    }
  };
  public sleepChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,0,0,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,176,240,0.28)'
    },
    {
      borderColor: 'black',
      backgroundColor: 'rgba(0,0,0,0.28)'
    }
  ];
  public sleepChartLegend = true;
  public sleepChartPlugins = [];
  public sleepChartType = 'bar';


  ngOnInit() {
    this.titleService.setTitle('Overall Physiological | COADAPT');

    this.repository.getData(APIv1.participants + '/metrics-only').subscribe((res) => {
        res = res as ParticipantList[];
        let totalParticipants = res.length;

        this.repository.getData(APIv1.ouraActivity).subscribe((res) => {
          let items = res as OuraActivity[];
    
          let steps = [0, 0, 0, 0, 0, 0, 0];
          let calories = [0, 0, 0, 0, 0, 0, 0];
    
          items.forEach(item => {
            let tmpDate = new Date(Date.parse(item.summaryDate));

            steps[tmpDate.getDay()] += item.steps;
            calories[tmpDate.getDay()] += item.caloriesTotal;
          });
    
          for (let i = 0; i < steps.length; i++) {
            steps[i] = +((steps[i] / totalParticipants).toString().substring(0, 3));
          }

          for (let i = 0; i < calories.length; i++) {
            calories[i] = +((calories[i] / totalParticipants).toString().substring(0, 3));
          }
          
          this.stepsChartData[0].data = steps;
          this.caloriesChartData[0].data = calories;
        });

        this.repository.getData(APIv1.ouraReadiness).subscribe((res) => {
          let items = res as OuraReadiness[];
    
          let readiness = [0, 0, 0, 0, 0, 0, 0];
    
          items.forEach(item => {
            let tmpDate = new Date(Date.parse(item.summaryDate));
            readiness[tmpDate.getDay()] += item.score;
          });
    
          for (let i = 0; i < readiness.length; i++) {
            readiness[i] = +((readiness[i] / totalParticipants).toString().substring(0, 3));
          }
          
          this.readinessesChartData[0].data = readiness;
        });

        this.repository.getData(APIv1.ouraSleep).subscribe((res) => {
          let items = res as OuraSleep[];
    
          let sleep = [0, 0, 0, 0, 0, 0, 0];
    
          items.forEach(item => {
            let tmpDate = new Date(Date.parse(item.summaryDate));

            let duration = item.duration / 3600;

            sleep[tmpDate.getDay()] += duration;
          });

          for (let i = 0; i < sleep.length; i++) {
            sleep[i] = +((sleep[i] / totalParticipants).toString().substring(0, 3));
          }
          
          this.sleepChartData[0].data = sleep;
        });
    });
  }

}
