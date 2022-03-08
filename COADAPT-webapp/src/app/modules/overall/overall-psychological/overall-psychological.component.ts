import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { RepositoryService } from './../../../services';
import { APIv1 } from '../../../constants';
import { ParticipantEdit, PsychologicalReport } from '../../../models';

import { ChartDataSets, ChartOptions, ChartType  } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet } from 'ng2-charts';

@Component({
  selector: 'app-overall-psychological',
  templateUrl: './overall-psychological.component.html',
  styleUrls: ['./overall-psychological.component.scss']
})
export class OverallPsychologicalComponent implements OnInit {

  @ViewChildren(BaseChartDirective) charts: QueryList<BaseChartDirective>;
  
  public careerSatisfactionChartData: ChartDataSets[] = [
    { data: [10, 20, 50, 50, 50, 50, 50, 50, 50, 50], label: 'Female', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50, 50, 50], label: 'Male', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50, 50, 50], label: 'Unknown', stack: 'a' }
  ];

  public careerSatisfactionChartLabels: Label[] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

  public careerSatisfactionChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Career Satisfaction'
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value + ' %';
          }
        }
      }]
    },
    tooltips: {
      enabled: true,
      mode: 'single',
      callbacks: {
        label: function(tooltipItem, data) {
          var allData = data['datasets'][tooltipItem['datasetIndex']]['label'] + ': ' + data['datasets'][tooltipItem['datasetIndex']]['data'][tooltipItem['index']] + '%';
          return allData;
        }
      }
    }
  };

  public careerSatisfactionChartColors: Color[] = [
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

  public careerSatisfactionChartLegend = true;
  public careerSatisfactionChartPlugins = [];
  public careerSatisfactionChartType = 'bar';

  public depressionChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Depression'
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value + ' %';
          }
        }
      }]
    },
    tooltips: {
      enabled: true,
      mode: 'single',
      callbacks: {
        label: function(tooltipItem, data) {
          var allData = data['datasets'][tooltipItem['datasetIndex']]['label'] + ': ' + data['datasets'][tooltipItem['datasetIndex']]['data'][tooltipItem['index']] + '%';
          return allData;
        }
      }
    }
  };

  public depressionChartColors: Color[] = [
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

  public depressionChartLabels: Label[] = ['0-10', '10-20', '20-30', '30-40', '40-50', '50-60', '60-70', '70-80', '80-90', '90-100'];

  public depressionChartType: ChartType = 'bar';
  public depressionChartLegend = true;
  public depressionChartPlugins = [];
  
  public depressionChartData: ChartDataSets[] = [
    { data: [10, 20, 50, 10, 20, 50, 10, 20, 50, 20], label: 'Female', stack: 'a' },
    { data: [20, 30, 50, 10, 20, 50, 10, 20, 50, 20], label: 'Male', stack: 'a' },
    { data: [30, 40, 30, 10, 20, 50, 10, 20, 50, 20], label: 'Unknown', stack: 'a' }
  ];

  public sleepProblemChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Sleep problem'
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value + ' %';
          }
        }
      }]
    },
    tooltips: {
      enabled: true,
      mode: 'single',
      callbacks: {
        label: function(tooltipItem, data) {
          var allData = data['datasets'][tooltipItem['datasetIndex']]['label'] + ': ' + data['datasets'][tooltipItem['datasetIndex']]['data'][tooltipItem['index']] + '%';
          return allData;
        }
      }
    }
  };

  public sleepProblemChartColors: Color[] = [
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

  public sleepProblemChartLabels: Label[] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

  public sleepProblemChartType: ChartType = 'bar';
  public sleepProblemChartLegend = true;
  public sleepProblemChartPlugins = [];
  
  public sleepProblemChartData: ChartDataSets[] = [
    { data: [10, 10, 10, 10, 10, 10, 10, 10, 10, 10], label: 'Female', stack: 'a' },
    { data: [10, 10, 10, 10, 10, 10, 10, 10, 10, 10], label: 'Male', stack: 'a' },
    { data: [10, 10, 10, 10, 10, 10, 10, 10, 10, 10], label: 'Unknown', stack: 'a' }
  ];

  public psychologicalHealthChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Psychological health'
    },
    scales: {
      yAxes: [{
        ticks: {
          callback: (value) => {
            return value + ' %';
          }
        }
      }]
    },
    tooltips: {
      enabled: true,
      mode: 'single',
      callbacks: {
        label: function(tooltipItem, data) {
          var allData = data['datasets'][tooltipItem['datasetIndex']]['label'] + ': ' + data['datasets'][tooltipItem['datasetIndex']]['data'][tooltipItem['index']] + '%';
          return allData;
        }
      }
    }
  };

  public psychologicalHealthChartColors: Color[] = [
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

  public psychologicalHealthChartLabels: Label[] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

  public psychologicalHealthChartLegend = true;
  public psychologicalHealthChartPlugins = [];
  public psychologicalHealthChartType = 'bar';

  public psychologicalHealthChartData: ChartDataSets[] = [
    { data: [10, 20, 50, 50, 50, 50, 50, 50, 50, 50], label: 'Female', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50, 50, 50], label: 'Male', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50, 50, 50], label: 'Unknown', stack: 'a' }
  ];

  public careerSatisfactionGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];
  public depressionGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];
  public sleepProblemGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];
  public psychologicalHealthGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];

  stats = [];

  constructor(private repository: RepositoryService, private titleService: Title) { }

  ngOnInit() {
    this.titleService.setTitle('Overall Psychological | COADAPT');
    this.setCharts();
  }

  public setCharts(): void {
    this.repository.getData(`${APIv1.psychologicalReports}`).subscribe((res) => {
      this.stats = res as PsychologicalReport[];

      //Career Satisfaction Chart
      var totalCount = 0;
      var careerSatisfactionGroup1Count = 0;
      var careerSatisfactionGroup2Count = 0;
      var careerSatisfactionGroup3Count = 0;
      var careerSatisfactionGroup4Count = 0;
      var careerSatisfactionGroup5Count = 0;
      var careerSatisfactionGroup6Count = 0;
      var careerSatisfactionGroup7Count = 0;
      var careerSatisfactionGroup8Count = 0;
      var careerSatisfactionGroup9Count = 0;
      var careerSatisfactionGroup10Count = 0;
      this.stats.forEach(element => {
        if (element.careerSatisfaction === 0){
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[0][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[0][1]++;
          }
          else {
            this.careerSatisfactionGroups[0][2]++;
          }
          careerSatisfactionGroup1Count++;
        }
        else if (element.careerSatisfaction === 1){
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[1][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[1][1]++;
          }
          else {
            this.careerSatisfactionGroups[1][2]++;
          }
          careerSatisfactionGroup2Count++;
        }
        else if (element.careerSatisfaction === 2){
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[2][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[2][1]++;
          }
          else {
            this.careerSatisfactionGroups[2][2]++;
          }
          careerSatisfactionGroup3Count++;
        }
        else if (element.careerSatisfaction === 3){
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[3][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[3][1]++;
          }
          else {
            this.careerSatisfactionGroups[3][2]++;
          }
          careerSatisfactionGroup4Count++;
        }
        else if (element.careerSatisfaction === 4){
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[4][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[4][1]++;
          }
          else {
            this.careerSatisfactionGroups[4][2]++;
          }
          careerSatisfactionGroup5Count++;
        }
        else if (element.careerSatisfaction === 5) {
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[5][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[5][1]++;
          }
          else {
            this.careerSatisfactionGroups[5][2]++;
          }
          careerSatisfactionGroup6Count++;
        }
        else if (element.careerSatisfaction === 6) {
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[6][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[6][1]++;
          }
          else {
            this.careerSatisfactionGroups[6][2]++;
          }
          careerSatisfactionGroup7Count++;
        }
        else if (element.careerSatisfaction === 7) {
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[7][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[7][1]++;
          }
          else {
            this.careerSatisfactionGroups[7][2]++;
          }
          careerSatisfactionGroup8Count++;
        }
        else if (element.careerSatisfaction === 8) {
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[8][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[8][1]++;
          }
          else {
            this.careerSatisfactionGroups[8][2]++;
          }
          careerSatisfactionGroup9Count++;
        }
        else if (element.careerSatisfaction === 9) {
          if (element.participant.gender === 'F'){
            this.careerSatisfactionGroups[9][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.careerSatisfactionGroups[9][1]++;
          }
          else {
            this.careerSatisfactionGroups[9][2]++;
          }
          careerSatisfactionGroup10Count++;
        }
        totalCount++;
      });
      var careerSatisfactionGroup1Coefficient = careerSatisfactionGroup1Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['0'] = Math.round((this.careerSatisfactionGroups[0][i]/careerSatisfactionGroup1Count) * 100 * careerSatisfactionGroup1Coefficient);
      }
      var careerSatisfactionGroup2Coefficient = careerSatisfactionGroup2Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['1'] = Math.round((this.careerSatisfactionGroups[1][i]/careerSatisfactionGroup2Count) * 100 * careerSatisfactionGroup2Coefficient);
      }
      var careerSatisfactionGroup3Coefficient = careerSatisfactionGroup3Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['2'] = Math.round((this.careerSatisfactionGroups[2][i]/careerSatisfactionGroup3Count) * 100 * careerSatisfactionGroup3Coefficient);
      }
      var careerSatisfactionGroup4Coefficient = careerSatisfactionGroup4Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['3'] = Math.round((this.careerSatisfactionGroups[3][i]/careerSatisfactionGroup4Count) * 100 * careerSatisfactionGroup4Coefficient);
      }
      var careerSatisfactionGroup5Coefficient = careerSatisfactionGroup5Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['4'] = Math.round((this.careerSatisfactionGroups[4][i]/careerSatisfactionGroup5Count) * 100 * careerSatisfactionGroup5Coefficient);
      }
      var careerSatisfactionGroup6Coefficient = careerSatisfactionGroup6Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['5'] = Math.round((this.careerSatisfactionGroups[5][i]/careerSatisfactionGroup6Count) * 100 * careerSatisfactionGroup6Coefficient);
      }
      var careerSatisfactionGroup7Coefficient = careerSatisfactionGroup7Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['6'] = Math.round((this.careerSatisfactionGroups[6][i]/careerSatisfactionGroup7Count) * 100 * careerSatisfactionGroup7Coefficient);
      }
      var careerSatisfactionGroup8Coefficient = careerSatisfactionGroup8Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['7'] = Math.round((this.careerSatisfactionGroups[7][i]/careerSatisfactionGroup8Count) * 100 * careerSatisfactionGroup8Coefficient);
      }
      var careerSatisfactionGroup9Coefficient = careerSatisfactionGroup9Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['8'] = Math.round((this.careerSatisfactionGroups[8][i]/careerSatisfactionGroup9Count) * 100 * careerSatisfactionGroup9Coefficient);
      }
      var careerSatisfactionGroup10Coefficient = careerSatisfactionGroup10Count / totalCount;
      for (var i = 0; i < this.careerSatisfactionChartData.length; i++) {
        this.careerSatisfactionChartData[i]['data']['9'] = Math.round((this.careerSatisfactionGroups[9][i]/careerSatisfactionGroup10Count) * 100 * careerSatisfactionGroup10Coefficient);
      }

      //Depression Chart
      totalCount = 0;
      var depressionGroup1Count = 0;
      var depressionGroup2Count = 0;
      var depressionGroup3Count = 0;
      var depressionGroup4Count = 0;
      var depressionGroup5Count = 0;
      var depressionGroup6Count = 0;
      var depressionGroup7Count = 0;
      var depressionGroup8Count = 0;
      var depressionGroup9Count = 0;
      var depressionGroup10Count = 0;
      this.stats.forEach(element => {
        if (element.depression <= 10){
          if (element.participant.gender === 'F'){
            this.depressionGroups[0][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[0][1]++;
          }
          else {
            this.depressionGroups[0][2]++;
          }
          depressionGroup1Count++;
        }
        else if (element.depression <= 20){
          if (element.participant.gender === 'F'){
            this.depressionGroups[1][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[1][1]++;
          }
          else {
            this.depressionGroups[1][2]++;
          }
          depressionGroup2Count++;
        }
        else if (element.depression <= 30){
          if (element.participant.gender === 'F'){
            this.depressionGroups[2][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[2][1]++;
          }
          else {
            this.depressionGroups[2][2]++;
          }
          depressionGroup3Count++;
        }
        else if (element.depression <= 40){
          if (element.participant.gender === 'F'){
            this.depressionGroups[3][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[3][1]++;
          }
          else {
            this.depressionGroups[3][2]++;
          }
          depressionGroup4Count++;
        }
        else if (element.depression <= 50){
          if (element.participant.gender === 'F'){
            this.depressionGroups[4][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[4][1]++;
          }
          else {
            this.depressionGroups[4][2]++;
          }
          depressionGroup5Count++;
        }
        else if (element.depression <= 60){
          if (element.participant.gender === 'F'){
            this.depressionGroups[5][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[5][1]++;
          }
          else {
            this.depressionGroups[5][2]++;
          }
          depressionGroup6Count++;
        }
        else if (element.depression <= 70){
          if (element.participant.gender === 'F'){
            this.depressionGroups[6][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[6][1]++;
          }
          else {
            this.depressionGroups[6][2]++;
          }
          depressionGroup7Count++;
        }
        else if (element.depression <= 80){
          if (element.participant.gender === 'F'){
            this.depressionGroups[7][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[7][1]++;
          }
          else {
            this.depressionGroups[7][2]++;
          }
          depressionGroup8Count++;
        }
        else if (element.depression <= 90){
          if (element.participant.gender === 'F'){
            this.depressionGroups[8][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[8][1]++;
          }
          else {
            this.depressionGroups[8][2]++;
          }
          depressionGroup9Count++;
        }
        else if (element.depression <= 100){
          if (element.participant.gender === 'F'){
            this.depressionGroups[9][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.depressionGroups[9][1]++;
          }
          else {
            this.depressionGroups[9][2]++;
          }
          depressionGroup10Count++;
        }
        totalCount++;
      });
      var depressionGroup1Coefficient = depressionGroup1Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['0'] = Math.round((this.depressionGroups[0][i]/depressionGroup1Count) * 100 * depressionGroup1Coefficient);
      }
      var depressionGroup2Coefficient = depressionGroup2Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['1'] = Math.round((this.depressionGroups[1][i]/depressionGroup2Count) * 100 * depressionGroup2Coefficient);
      }
      var depressionGroup3Coefficient = depressionGroup3Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['2'] = Math.round((this.depressionGroups[2][i]/depressionGroup3Count) * 100 * depressionGroup3Coefficient);
      }
      var depressionGroup4Coefficient = depressionGroup4Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['3'] = Math.round((this.depressionGroups[3][i]/depressionGroup4Count) * 100 * depressionGroup4Coefficient);
      }
      var depressionGroup5Coefficient = depressionGroup5Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['4'] = Math.round((this.depressionGroups[4][i]/depressionGroup5Count) * 100 * depressionGroup5Coefficient);
      }
      var depressionGroup6Coefficient = depressionGroup6Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['5'] = Math.round((this.depressionGroups[5][i]/depressionGroup6Count) * 100 * depressionGroup6Coefficient);
      }
      var depressionGroup7Coefficient = depressionGroup7Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['6'] = Math.round((this.depressionGroups[6][i]/depressionGroup7Count) * 100 * depressionGroup7Coefficient);
      }
      var depressionGroup8Coefficient = depressionGroup8Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['7'] = Math.round((this.depressionGroups[7][i]/depressionGroup8Count) * 100 * depressionGroup8Coefficient);
      }
      var depressionGroup9Coefficient = depressionGroup9Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['8'] = Math.round((this.depressionGroups[8][i]/depressionGroup9Count) * 100 * depressionGroup9Coefficient);
      }
      var depressionGroup10Coefficient = depressionGroup10Count / totalCount;
      for (var i = 0; i < this.depressionChartData.length; i++) {
        this.depressionChartData[i]['data']['9'] = Math.round((this.depressionGroups[9][i]/depressionGroup10Count) * 100 * depressionGroup10Coefficient);
      }

      //Sleep problem Chart (based on their most recent participation)
      totalCount = 0;
      var sleepProblemGroup1Count = 0;
      var sleepProblemGroup2Count = 0;
      var sleepProblemGroup3Count = 0;
      var sleepProblemGroup4Count = 0;
      var sleepProblemGroup5Count = 0;
      var sleepProblemGroup6Count = 0;
      var sleepProblemGroup7Count = 0;
      var sleepProblemGroup8Count = 0;
      var sleepProblemGroup9Count = 0;
      var sleepProblemGroup10Count = 0;
      this.stats.forEach(element => {
        if (element.sleepProblem === 0){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[0][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[0][1]++;
          }
          else {
            this.sleepProblemGroups[0][2]++;
          }
          sleepProblemGroup1Count++;
        }
        else if (element.sleepProblem === 1){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[1][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[1][1]++;
          }
          else {
            this.sleepProblemGroups[1][2]++;
          }
          sleepProblemGroup2Count++;
        }
        else if (element.sleepProblem === 2){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[2][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[2][1]++;
          }
          else {
            this.sleepProblemGroups[2][2]++;
          }
          sleepProblemGroup3Count++;
        }
        else if (element.sleepProblem === 3){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[3][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[3][1]++;
          }
          else {
            this.sleepProblemGroups[3][2]++;
          }
          sleepProblemGroup4Count++;
        }
        else if (element.sleepProblem === 4){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[4][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[4][1]++;
          }
          else {
            this.sleepProblemGroups[4][2]++;
          }
          sleepProblemGroup5Count++;
        }
        else if (element.sleepProblem === 5){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[5][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[5][1]++;
          }
          else {
            this.sleepProblemGroups[5][2]++;
          }
          sleepProblemGroup6Count++;
        }
        else if (element.sleepProblem === 6){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[6][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[6][1]++;
          }
          else {
            this.sleepProblemGroups[6][2]++;
          }
          sleepProblemGroup7Count++;
        }
        else if (element.sleepProblem === 7){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[7][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[7][1]++;
          }
          else {
            this.sleepProblemGroups[7][2]++;
          }
          sleepProblemGroup8Count++;
        }
        else if (element.sleepProblem === 8){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[8][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[8][1]++;
          }
          else {
            this.sleepProblemGroups[8][2]++;
          }
          sleepProblemGroup9Count++;
        }
        else if (element.sleepProblem === 9){
          if (element.participant.gender === 'F'){
            this.sleepProblemGroups[9][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.sleepProblemGroups[9][1]++;
          }
          else {
            this.sleepProblemGroups[9][2]++;
          }
          sleepProblemGroup10Count++;
        }
        totalCount++;
      });
      var sleepProblemGroup1Coefficient = sleepProblemGroup1Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['0'] = Math.round((this.sleepProblemGroups[0][i]/sleepProblemGroup1Count) * 100 * sleepProblemGroup1Coefficient);
      }
      var sleepProblemGroup2Coefficient = sleepProblemGroup2Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['1'] = Math.round((this.sleepProblemGroups[1][i]/sleepProblemGroup2Count) * 100 * sleepProblemGroup2Coefficient);
      }
      var sleepProblemGroup3Coefficient = sleepProblemGroup3Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['2'] = Math.round((this.sleepProblemGroups[2][i]/sleepProblemGroup3Count) * 100 * sleepProblemGroup3Coefficient);
      }
      var sleepProblemGroup4Coefficient = sleepProblemGroup4Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['3'] = Math.round((this.sleepProblemGroups[3][i]/sleepProblemGroup4Count) * 100 * sleepProblemGroup4Coefficient);
      }
      var sleepProblemGroup5Coefficient = sleepProblemGroup5Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['4'] = Math.round((this.sleepProblemGroups[4][i]/sleepProblemGroup5Count) * 100 * sleepProblemGroup5Coefficient);
      }
      var sleepProblemGroup6Coefficient = sleepProblemGroup6Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['5'] = Math.round((this.sleepProblemGroups[5][i]/sleepProblemGroup6Count) * 100 * sleepProblemGroup6Coefficient);
      }
      var sleepProblemGroup7Coefficient = sleepProblemGroup7Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['6'] = Math.round((this.sleepProblemGroups[6][i]/sleepProblemGroup7Count) * 100 * sleepProblemGroup7Coefficient);
      }
      var sleepProblemGroup8Coefficient = sleepProblemGroup8Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['7'] = Math.round((this.sleepProblemGroups[7][i]/sleepProblemGroup8Count) * 100 * sleepProblemGroup8Coefficient);
      }
      var sleepProblemGroup9Coefficient = sleepProblemGroup9Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['8'] = Math.round((this.sleepProblemGroups[8][i]/sleepProblemGroup9Count) * 100 * sleepProblemGroup9Coefficient);
      }
      var sleepProblemGroup10Coefficient = sleepProblemGroup10Count / totalCount;
      for (var i = 0; i < this.sleepProblemChartData.length; i++) {
        this.sleepProblemChartData[i]['data']['9'] = Math.round((this.sleepProblemGroups[9][i]/sleepProblemGroup10Count) * 100 * sleepProblemGroup10Coefficient);
      }

      //Psychological health Chart
      var totalCount = 0;
      var psychologicalHealthGroup1Count = 0;
      var psychologicalHealthGroup2Count = 0;
      var psychologicalHealthGroup3Count = 0;
      var psychologicalHealthGroup4Count = 0;
      var psychologicalHealthGroup5Count = 0;
      var psychologicalHealthGroup6Count = 0;
      var psychologicalHealthGroup7Count = 0;
      var psychologicalHealthGroup8Count = 0;
      var psychologicalHealthGroup9Count = 0;
      var psychologicalHealthGroup10Count = 0;
      this.stats.forEach(element => {
        if (element.psychologicalHealth === 0){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[0][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[0][1]++;
          }
          else {
            this.psychologicalHealthGroups[0][2]++;
          }
          psychologicalHealthGroup1Count++;
        }
        else if (element.psychologicalHealth === 1){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[1][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[1][1]++;
          }
          else {
            this.psychologicalHealthGroups[1][2]++;
          }
          psychologicalHealthGroup2Count++;
        }
        else if (element.psychologicalHealth === 2){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[2][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[2][1]++;
          }
          else {
            this.psychologicalHealthGroups[2][2]++;
          }
          psychologicalHealthGroup3Count++;
        }
        else if (element.psychologicalHealth === 3){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[3][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[3][1]++;
          }
          else {
            this.psychologicalHealthGroups[3][2]++;
          }
          psychologicalHealthGroup4Count++;
        }
        else if (element.psychologicalHealth === 4){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[4][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[4][1]++;
          }
          else {
            this.psychologicalHealthGroups[4][2]++;
          }
          psychologicalHealthGroup5Count++;
        }
        else if (element.psychologicalHealth === 5){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[5][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[5][1]++;
          }
          else {
            this.psychologicalHealthGroups[5][2]++;
          }
          psychologicalHealthGroup6Count++;
        }
        else if (element.psychologicalHealth === 6){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[6][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[6][1]++;
          }
          else {
            this.psychologicalHealthGroups[6][2]++;
          }
          psychologicalHealthGroup7Count++;
        }
        else if (element.psychologicalHealth === 7){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[7][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[7][1]++;
          }
          else {
            this.psychologicalHealthGroups[7][2]++;
          }
          psychologicalHealthGroup8Count++;
        }
        else if (element.psychologicalHealth === 8){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[8][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[8][1]++;
          }
          else {
            this.psychologicalHealthGroups[8][2]++;
          }
          psychologicalHealthGroup9Count++;
        }
        else if (element.psychologicalHealth === 9){
          if (element.participant.gender === 'F'){
            this.psychologicalHealthGroups[9][0]++;
          }
          else if (element.participant.gender === 'M'){
            this.psychologicalHealthGroups[9][1]++;
          }
          else {
            this.psychologicalHealthGroups[9][2]++;
          }
          psychologicalHealthGroup10Count++;
        }
        totalCount++;
      });
      var psychologicalHealthGroup1Coefficient = psychologicalHealthGroup1Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['0'] = Math.round((this.psychologicalHealthGroups[0][i]/psychologicalHealthGroup1Count) * 100 * psychologicalHealthGroup1Coefficient);
      }
      var psychologicalHealthGroup2Coefficient = psychologicalHealthGroup2Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['1'] = Math.round((this.psychologicalHealthGroups[1][i]/psychologicalHealthGroup2Count) * 100 * psychologicalHealthGroup2Coefficient);
      }
      var psychologicalHealthGroup3Coefficient = psychologicalHealthGroup3Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['2'] = Math.round((this.psychologicalHealthGroups[2][i]/psychologicalHealthGroup3Count) * 100 * psychologicalHealthGroup3Coefficient);
      }
      var psychologicalHealthGroup4Coefficient = psychologicalHealthGroup4Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['3'] = Math.round((this.psychologicalHealthGroups[3][i]/psychologicalHealthGroup4Count) * 100 * psychologicalHealthGroup4Coefficient);
      }
      var psychologicalHealthGroup5Coefficient = psychologicalHealthGroup5Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['4'] = Math.round((this.psychologicalHealthGroups[4][i]/psychologicalHealthGroup5Count) * 100 * psychologicalHealthGroup5Coefficient);
      }
      var psychologicalHealthGroup6Coefficient = psychologicalHealthGroup6Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['5'] = Math.round((this.psychologicalHealthGroups[5][i]/psychologicalHealthGroup6Count) * 100 * psychologicalHealthGroup6Coefficient);
      }
      var psychologicalHealthGroup7Coefficient = psychologicalHealthGroup7Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['6'] = Math.round((this.psychologicalHealthGroups[6][i]/psychologicalHealthGroup7Count) * 100 * psychologicalHealthGroup7Coefficient);
      }
      var psychologicalHealthGroup8Coefficient = psychologicalHealthGroup8Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['7'] = Math.round((this.psychologicalHealthGroups[7][i]/psychologicalHealthGroup8Count) * 100 * psychologicalHealthGroup8Coefficient);
      }
      var psychologicalHealthGroup9Coefficient = psychologicalHealthGroup9Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['8'] = Math.round((this.psychologicalHealthGroups[8][i]/psychologicalHealthGroup9Count) * 100 * psychologicalHealthGroup9Coefficient);
      }
      var psychologicalHealthGroup10Coefficient = psychologicalHealthGroup10Count / totalCount;
      for (var i = 0; i < this.psychologicalHealthChartData.length; i++) {
        this.psychologicalHealthChartData[i]['data']['9'] = Math.round((this.psychologicalHealthGroups[9][i]/psychologicalHealthGroup10Count) * 100 * psychologicalHealthGroup10Coefficient);
      }

      //update and finalize the charts
      this.charts.forEach((child) => {
        child.chart.update()
      });
    });
  }

}
