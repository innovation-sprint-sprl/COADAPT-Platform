import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { RepositoryService } from './../../../services';
import { APIv1 } from '../../../constants';
import { ParticipantEdit } from '../../../models';

import { ChartDataSets, ChartOptions, ChartType  } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet } from 'ng2-charts';

@Component({
  selector: 'app-overall-demographics',
  templateUrl: './overall-demographics.component.html',
  styleUrls: ['./overall-demographics.component.scss']
})
export class OverallDemographicsComponent implements OnInit {

  @ViewChildren(BaseChartDirective) charts: QueryList<BaseChartDirective>;
  
  public ageChartData: ChartDataSets[] = [
    { data: [10, 20, 50, 50, 50, 50, 50, 50], label: 'Female', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50], label: 'Male', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50], label: 'Unknown', stack: 'a' }
  ];

  public ageChartLabels: Label[] = ['<30', '30-40', '40-50', '50-55', '55-60', '60-65', '>65', 'Unknown'];

  public ageChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Age'
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

  public ageChartColors: Color[] = [
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

  public ageChartLegend = true;
  public ageChartPlugins = [];
  public ageChartType = 'bar';

  public maritalStatusChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Marital status'
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

  public maritalStatusChartColors: Color[] = [
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

  public maritalStatusChartLabels: Label[] = ['Single', 'Married', 'Unknown'];

  public maritalStatusChartType: ChartType = 'bar';
  public maritalStatusChartLegend = true;
  public maritalStatusChartPlugins = [];
  
  public maritalStatusChartData: ChartDataSets[] = [
    { data: [10, 20, 50], label: 'Female', stack: 'a' },
    { data: [20, 30, 50], label: 'Male', stack: 'a' },
    { data: [30, 40, 30], label: 'Unknown', stack: 'a' }
  ];

  public educationChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Education'
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

  public educationChartColors: Color[] = [
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

  public educationChartLabels: Label[] = ['Primary', 'Lower Secondary', 'Upper Secondary', 
                                  'Post-secondary non-tertiary', 'Short-cycle tertiary', 
                                  'Bachelor\'s', 'Master\'s', 'Doctoral', 'Unknown'];

  public educationChartType: ChartType = 'bar';
  public educationChartLegend = true;
  public educationChartPlugins = [];
  
  public educationChartData: ChartDataSets[] = [
    { data: [10, 10, 10, 10, 10, 10, 10, 10, 20], label: 'Female', stack: 'a' },
    { data: [10, 10, 10, 10, 10, 10, 10, 10, 20], label: 'Male', stack: 'a' },
    { data: [10, 10, 10, 10, 10, 10, 10, 10, 20], label: 'Unknown', stack: 'a' }
  ];

  public yearsAtWorkChartOptions = {
    responsive: true,
    legend: {
      display: true
    },
    title: {
      display: true,
      text: 'Years at work'
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

  public yearsAtWorkChartColors: Color[] = [
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

  public yearsAtWorkChartLabels: Label[] = ['0-10', '10-15', '15-20', '20-25', '25-30', '30-35', '>35', 'Unknown'];

  public yearsAtWorkChartLegend = true;
  public yearsAtWorkChartPlugins = [];
  public yearsAtWorkChartType = 'bar';

  public yearsAtWorkChartData: ChartDataSets[] = [
    { data: [10, 20, 50, 50, 50, 50, 50, 50], label: 'Female', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50], label: 'Male', stack: 'a' },
    { data: [10, 20, 50, 50, 50, 50, 50, 50], label: 'Unknown', stack: 'a' }
  ];

  public ageGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];
  public maritalStatusGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0]];
  public educationGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];
  public yearsAtWorkGroups: number[][] = [[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]];

  stats = [];

  constructor(private repository: RepositoryService, private titleService: Title) { }

  ngOnInit() {
    this.titleService.setTitle('Overall Demographics | COADAPT');
    this.setCharts();
  }

  public setCharts(): void {
    this.repository.getData(`${APIv1.participants}/full`).subscribe((res) => {
      this.stats = res as ParticipantEdit[];

      //Age Chart
      var totalCount = 0;
      var ageGroup1Count = 0;
      var ageGroup2Count = 0;
      var ageGroup3Count = 0;
      var ageGroup4Count = 0;
      var ageGroup5Count = 0;
      var ageGroup6Count = 0;
      var ageGroup7Count = 0;
      var ageGroup8Count = 0;
      this.stats.forEach(element => {
        var yearOfBirth = 0;
        var age = -1;
        var currentYear = new Date().getFullYear();
        if (element.dateOfBirth !== '0001-01-01T00:00:00') {
          yearOfBirth = new Date(element.dateOfBirth).getFullYear();
          age = currentYear - yearOfBirth;
        }
        if (age >= 0 && age <= 30){
          if (element.gender === 'F'){
            this.ageGroups[0][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[0][1]++;
          }
          else {
            this.ageGroups[0][2]++;
          }
          ageGroup1Count++;
        }
        else if (age > 30 && age <= 40){
          if (element.gender === 'F'){
            this.ageGroups[1][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[1][1]++;
          }
          else {
            this.ageGroups[1][2]++;
          }
          ageGroup2Count++;
        }
        else if (age > 40 && age <= 50){
          if (element.gender === 'F'){
            this.ageGroups[2][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[2][1]++;
          }
          else {
            this.ageGroups[2][2]++;
          }
          ageGroup3Count++;
        }
        else if (age > 50 && age <= 55){
          if (element.gender === 'F'){
            this.ageGroups[3][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[3][1]++;
          }
          else {
            this.ageGroups[3][2]++;
          }
          ageGroup4Count++;
        }
        else if (age > 55 && age <= 60){
          if (element.gender === 'F'){
            this.ageGroups[4][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[4][1]++;
          }
          else {
            this.ageGroups[4][2]++;
          }
          ageGroup5Count++;
        }
        else if (age > 60 && age <= 65){
          if (element.gender === 'F'){
            this.ageGroups[5][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[5][1]++;
          }
          else {
            this.ageGroups[5][2]++;
          }
          ageGroup6Count++;
        }
        else if (age > 65){
          if (element.gender === 'F'){
            this.ageGroups[6][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[6][1]++;
          }
          else {
            this.ageGroups[6][2]++;
          }
          ageGroup7Count++;
        }
        else{
          if (element.gender === 'F'){
            this.ageGroups[7][0]++;
          }
          else if (element.gender === 'M'){
            this.ageGroups[7][1]++;
          }
          else {
            this.ageGroups[7][2]++;
          }
          ageGroup8Count++;
        }
        totalCount++;
      });
      var ageGroup1Coefficient = ageGroup1Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['0'] = Math.round((this.ageGroups[0][i]/ageGroup1Count) * 100 * ageGroup1Coefficient);
      }
      var ageGroup2Coefficient = ageGroup2Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['1'] = Math.round((this.ageGroups[1][i]/ageGroup2Count) * 100 * ageGroup2Coefficient);
      }
      var ageGroup3Coefficient = ageGroup3Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['2'] = Math.round((this.ageGroups[2][i]/ageGroup3Count) * 100 * ageGroup3Coefficient);
      }
      var ageGroup4Coefficient = ageGroup4Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['3'] = Math.round((this.ageGroups[3][i]/ageGroup4Count) * 100 * ageGroup4Coefficient);
      }
      var ageGroup5Coefficient = ageGroup5Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['4'] = Math.round((this.ageGroups[4][i]/ageGroup5Count) * 100 * ageGroup5Coefficient);
      }
      var ageGroup6Coefficient = ageGroup6Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['5'] = Math.round((this.ageGroups[5][i]/ageGroup6Count) * 100 * ageGroup6Coefficient);
      }
      var ageGroup7Coefficient = ageGroup7Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['6'] = Math.round((this.ageGroups[6][i]/ageGroup7Count) * 100 * ageGroup7Coefficient);
      }
      var ageGroup8Coefficient = ageGroup8Count / totalCount;
      for (var i = 0; i < this.ageChartData.length; i++) {
        this.ageChartData[i]['data']['7'] = Math.round((this.ageGroups[7][i]/ageGroup8Count) * 100 * ageGroup8Coefficient);
      }

      //Marital Status Chart (based on their most recent participation)
      totalCount = 0;
      var singleCount = 0;
      var marriedCount = 0;
      var unknownCount = 0;
      this.stats.forEach(element => {
        if (element.studyParticipants[0].maritalStatus === 'Single' || element.studyParticipants[0].maritalStatus === 'sposato/a'){
          if (element.gender === 'F'){
            this.maritalStatusGroups[0][0]++;
          }
          else if (element.gender === 'M'){
            this.maritalStatusGroups[0][1]++;
          }
          else {
            this.maritalStatusGroups[0][2]++;
          }
          singleCount++;
        }
        else if (element.studyParticipants[0].maritalStatus === 'Married'){
          if (element.gender === 'F'){
            this.maritalStatusGroups[1][0]++;
          }
          else if (element.gender === 'M'){
            this.maritalStatusGroups[1][1]++;
          }
          else {
            this.maritalStatusGroups[1][2]++;
          }
          marriedCount++;
        } 
        else {
          if (element.gender === 'F'){
            this.maritalStatusGroups[2][0]++;
          }
          else if (element.gender === 'M'){
            this.maritalStatusGroups[2][1]++;
          }
          else {
            this.maritalStatusGroups[2][2]++;
          }
          unknownCount++;
        }  
        totalCount++;
      });
      var singleCoefficient = singleCount / totalCount;
      for (var i = 0; i < this.maritalStatusChartData.length; i++) {
        this.maritalStatusChartData[i]['data']['0'] = Math.round((this.maritalStatusGroups[0][i]/singleCount) * 100 * singleCoefficient);
      }
      var marriedCoefficient = marriedCount / totalCount;
      for (var i = 0; i < this.maritalStatusChartData.length; i++) {
        this.maritalStatusChartData[i]['data']['1'] = Math.round((this.maritalStatusGroups[1][i]/marriedCount) * 100 * marriedCoefficient);
      }
      var unknownCoefficient = unknownCount / totalCount;
      for (var i = 0; i < this.maritalStatusChartData.length; i++) {
        this.maritalStatusChartData[i]['data']['2'] = Math.round((this.maritalStatusGroups[2][i]/unknownCount) * 100 * unknownCoefficient);
      }

      //Education Chart (based on their most recent participation)
      totalCount = 0;
      var primaryCount = 0;
      var lowerSecondaryCount = 0;
      var upperSecondaryCount = 0;
      var nonTertiaryCount = 0;
      var shortCycleTertiaryCount = 0;
      var bachelorsCount = 0;
      var mastersCount = 0;
      var doctoralCount = 0;
      var unknownCount = 0;
      this.stats.forEach(element => {
        if (element.studyParticipants[0].education === 'Primary'){
          if (element.gender === 'F'){
            this.educationGroups[0][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[0][1]++;
          }
          else {
            this.educationGroups[0][2]++;
          }
          primaryCount++;
        }
        else if (element.studyParticipants[0].education === 'Lower Secondary'){
          if (element.gender === 'F'){
            this.educationGroups[1][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[1][1]++;
          }
          else {
            this.educationGroups[1][2]++;
          }
          lowerSecondaryCount++;
        }
        else if (element.studyParticipants[0].education === 'Upper Secondary'){
          if (element.gender === 'F'){
            this.educationGroups[2][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[2][1]++;
          }
          else {
            this.educationGroups[2][2]++;
          }
          upperSecondaryCount++;
        }
        else if (element.studyParticipants[0].education === 'Post-secondary non-tertiary'){
          if (element.gender === 'F'){
            this.educationGroups[3][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[3][1]++;
          }
          else {
            this.educationGroups[3][2]++;
          }
          nonTertiaryCount++;
        }
        else if (element.studyParticipants[0].education === 'Short-cycle tertiary'){
          if (element.gender === 'F'){
            this.educationGroups[4][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[4][1]++;
          }
          else {
            this.educationGroups[4][2]++;
          }
          shortCycleTertiaryCount++;
        }
        else if (element.studyParticipants[0].education === 'Bachelor\'s'){
          if (element.gender === 'F'){
            this.educationGroups[5][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[5][1]++;
          }
          else {
            this.educationGroups[5][2]++;
          }
          bachelorsCount++;
        }
        else if (element.studyParticipants[0].education === 'Master\'s'){
          if (element.gender === 'F'){
            this.educationGroups[6][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[6][1]++;
          }
          else {
            this.educationGroups[6][2]++;
          }
          mastersCount++;
        }
        else if (element.studyParticipants[0].education === 'Doctoral'){
          if (element.gender === 'F'){
            this.educationGroups[7][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[7][1]++;
          }
          else {
            this.educationGroups[7][2]++;
          }
          doctoralCount++;
        }
        else {
          if (element.gender === 'F'){
            this.educationGroups[8][0]++;
          }
          else if (element.gender === 'M'){
            this.educationGroups[8][1]++;
          }
          else {
            this.educationGroups[8][2]++;
          }
          unknownCount++;
        }
        totalCount++;
      });
      var primaryCoefficient = primaryCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['0'] = Math.round((this.educationGroups[0][i]/primaryCount) * 100 * primaryCoefficient);
      }
      var lowerSecondaryCoefficient = lowerSecondaryCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['1'] = Math.round((this.educationGroups[1][i]/lowerSecondaryCount) * 100 * lowerSecondaryCoefficient);
      }
      var upperSecondaryCoefficient = upperSecondaryCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['2'] = Math.round((this.educationGroups[2][i]/upperSecondaryCount) * 100 * upperSecondaryCoefficient);
      }
      var nonTertiaryCoefficient = nonTertiaryCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['3'] = Math.round((this.educationGroups[3][i]/nonTertiaryCount) * 100 * nonTertiaryCoefficient);
      }
      var shortCycleTertiaryCoefficient = shortCycleTertiaryCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['4'] = Math.round((this.educationGroups[4][i]/shortCycleTertiaryCount) * 100 * shortCycleTertiaryCoefficient);
      }
      var bachelorsCoefficient = bachelorsCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['5'] = Math.round((this.educationGroups[5][i]/bachelorsCount) * 100 * bachelorsCoefficient);
      }
      var mastersCoefficient = mastersCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['6'] = Math.round((this.educationGroups[6][i]/mastersCount) * 100 * mastersCoefficient);
      }
      var doctoralCoefficient = doctoralCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['7'] = Math.round((this.educationGroups[7][i]/doctoralCount) * 100 * doctoralCoefficient);
      }
      var unknownCoefficient = unknownCount / totalCount;
      for (var i = 0; i < this.educationChartData.length; i++) {
        this.educationChartData[i]['data']['8'] = Math.round((this.educationGroups[8][i]/unknownCount) * 100 * unknownCoefficient);
      }

      //Years at work Chart
      var totalCount = 0;
      var yearsAtWorkGroup1Count = 0;
      var yearsAtWorkGroup2Count = 0;
      var yearsAtWorkGroup3Count = 0;
      var yearsAtWorkGroup4Count = 0;
      var yearsAtWorkGroup5Count = 0;
      var yearsAtWorkGroup6Count = 0;
      var yearsAtWorkGroup7Count = 0;
      var yearsAtWorkGroup8Count = 0;
      this.stats.forEach(element => {

        //if dateOfFirstJob does not exist, take dateOfCurrentJob into consideration
        var chartData = element.dateOfFirstJob;
        if (chartData === '0001-01-01T00:00:00') {
          chartData = element.studyParticipants[0]?.dateOfCurrentJob;
        }
        //if dateOfCurrentJob does not exist as well, place it into the Unknown category
        var currentYear = new Date().getFullYear();
        var firstYearAtWork = 0;
        var yearsAtWork = -1;
        if (chartData !== '0001-01-01T00:00:00') {
          firstYearAtWork = new Date(chartData).getFullYear();
          yearsAtWork = currentYear - firstYearAtWork;
        }
        if (yearsAtWork >= 0 && yearsAtWork <= 10){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[0][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[0][1]++;
          }
          else {
            this.yearsAtWorkGroups[0][2]++;
          }
          yearsAtWorkGroup1Count++;
        }
        else if (yearsAtWork > 10 && yearsAtWork <= 15){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[1][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[1][1]++;
          }
          else {
            this.yearsAtWorkGroups[1][2]++;
          }
          yearsAtWorkGroup2Count++;
        }
        else if (yearsAtWork > 15 && yearsAtWork <= 20){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[2][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[2][1]++;
          }
          else {
            this.yearsAtWorkGroups[2][2]++;
          }
          yearsAtWorkGroup3Count++;
        }
        else if (yearsAtWork > 20 && yearsAtWork <= 25){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[3][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[3][1]++;
          }
          else {
            this.yearsAtWorkGroups[3][2]++;
          }
          yearsAtWorkGroup4Count++;
        }
        else if (yearsAtWork > 25 && yearsAtWork <= 30){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[4][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[4][1]++;
          }
          else {
            this.yearsAtWorkGroups[4][2]++;
          }
          yearsAtWorkGroup5Count++;
        }
        else if (yearsAtWork > 30 && yearsAtWork <= 35){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[5][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[5][1]++;
          }
          else {
            this.yearsAtWorkGroups[5][2]++;
          }
          yearsAtWorkGroup6Count++;
        }
        else if (yearsAtWork > 35){
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[6][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[6][1]++;
          }
          else {
            this.yearsAtWorkGroups[6][2]++;
          }
          yearsAtWorkGroup7Count++;
        }
        else{
          if (element.gender === 'F'){
            this.yearsAtWorkGroups[7][0]++;
          }
          else if (element.gender === 'M'){
            this.yearsAtWorkGroups[7][1]++;
          }
          else {
            this.yearsAtWorkGroups[7][2]++;
          }
          yearsAtWorkGroup8Count++;
        }
        totalCount++;
      });
      var yearsAtWorkGroup1Coefficient = yearsAtWorkGroup1Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['0'] = Math.round((this.yearsAtWorkGroups[0][i]/yearsAtWorkGroup1Count) * 100 * yearsAtWorkGroup1Coefficient);
      }
      var yearsAtWorkGroup2Coefficient = yearsAtWorkGroup2Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['1'] = Math.round((this.yearsAtWorkGroups[1][i]/yearsAtWorkGroup2Count) * 100 * yearsAtWorkGroup2Coefficient);
      }
      var yearsAtWorkGroup3Coefficient = yearsAtWorkGroup3Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['2'] = Math.round((this.yearsAtWorkGroups[2][i]/yearsAtWorkGroup3Count) * 100 * yearsAtWorkGroup3Coefficient);
      }
      var yearsAtWorkGroup4Coefficient = yearsAtWorkGroup4Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['3'] = Math.round((this.yearsAtWorkGroups[3][i]/yearsAtWorkGroup4Count) * 100 * yearsAtWorkGroup4Coefficient);
      }
      var yearsAtWorkGroup5Coefficient = yearsAtWorkGroup5Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['4'] = Math.round((this.yearsAtWorkGroups[4][i]/yearsAtWorkGroup5Count) * 100 * yearsAtWorkGroup5Coefficient);
      }
      var yearsAtWorkGroup6Coefficient = yearsAtWorkGroup6Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['5'] = Math.round((this.yearsAtWorkGroups[5][i]/yearsAtWorkGroup6Count) * 100 * yearsAtWorkGroup6Coefficient);
      }
      var yearsAtWorkGroup7Coefficient = yearsAtWorkGroup7Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['6'] = Math.round((this.yearsAtWorkGroups[6][i]/yearsAtWorkGroup7Count) * 100 * yearsAtWorkGroup7Coefficient);
      }
      var yearsAtWorkGroup8Coefficient = yearsAtWorkGroup8Count / totalCount;
      for (var i = 0; i < this.yearsAtWorkChartData.length; i++) {
        this.yearsAtWorkChartData[i]['data']['7'] = Math.round((this.yearsAtWorkGroups[7][i]/yearsAtWorkGroup8Count) * 100 * yearsAtWorkGroup8Coefficient);
      }

      //update and finalize the charts
      this.charts.forEach((child) => {
        child.chart.update()
      });
    });
  }

}
