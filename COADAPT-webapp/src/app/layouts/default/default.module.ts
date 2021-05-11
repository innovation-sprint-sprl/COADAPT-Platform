import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Material
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatBadgeModule } from '@angular/material/badge';
import { MatBottomSheetModule } from '@angular/material/bottom-sheet';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatStepperModule } from '@angular/material/stepper';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSliderModule } from '@angular/material/slider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTreeModule } from '@angular/material/tree';

import { SharedModule } from '../../shared/shared.module';
import { DashboardService } from '../../modules/dashboard.service';

/* Components */
import { DefaultComponent } from './default.component';

/* Charts */
import { ChartsModule } from 'ng2-charts';

/* Directives */
import { HasRoleDirective } from '../../directives/has-role.directive';

/* Modules */
import {
  DashboardComponent,
  OrganizationsListComponent,
  OrganizationsCreateComponent,
  OrganizationsEditComponent,
  StudiesListComponent,
  StudiesCreateComponent,
  StudiesEditComponent,
  SitesListComponent,
  SitesCreateComponent,
  SitesEditComponent,
  GroupsListComponent,
  GroupsCreateComponent,
  GroupsEditComponent,
  AdministratorsCreateComponent,
  AdministratorsEditComponent,
  AdministratorsListComponent,
  SubAdministratorsListComponent,
  SubAdministratorsCreateComponent,
  SubAdministratorsEditComponent,
  SupervisorsListComponent,
  SupervisorsCreateComponent,
  SupervisorsEditComponent,
  TherapistsCreateComponent,
  TherapistsEditComponent,
  TherapistsListComponent,
  ParticipantsListComponent,
  ParticipantsCreateComponent,
  ParticipantsEditComponent,
  IndividualPhysiologicalComponent,
  IndividualPsychologicalComponent,
  OverallDemographicsComponent,
  OverallPhysiologicalComponent,
  OverallPsychologicalComponent
} from '../../modules';

@NgModule({
  declarations: [
    DefaultComponent,
    HasRoleDirective,
    DashboardComponent,
    OrganizationsListComponent,
    OrganizationsCreateComponent,
    StudiesListComponent,
    StudiesCreateComponent,
    SitesListComponent,
    SitesCreateComponent,
    GroupsListComponent,
    GroupsCreateComponent,
    SubAdministratorsListComponent,
    SubAdministratorsCreateComponent,
    SupervisorsListComponent,
    SupervisorsCreateComponent,
    TherapistsListComponent,
    TherapistsCreateComponent,
    ParticipantsListComponent,
    ParticipantsCreateComponent,
    IndividualPhysiologicalComponent,
    IndividualPsychologicalComponent,
    OverallDemographicsComponent,
    OverallPhysiologicalComponent,
    OverallPsychologicalComponent,
    AdministratorsCreateComponent,
    AdministratorsListComponent,
    AdministratorsEditComponent,
    TherapistsEditComponent,
    SupervisorsEditComponent,
    SubAdministratorsEditComponent,
    StudiesEditComponent,
    SitesEditComponent,
    ParticipantsEditComponent,
    OrganizationsEditComponent,
    GroupsEditComponent,
  ],
  imports: [
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    RouterModule,
    FlexLayoutModule,
    ReactiveFormsModule,
    SharedModule,
    MatAutocompleteModule,
    MatBadgeModule,
    MatBottomSheetModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatStepperModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatTreeModule,
    ChartsModule
  ],
  providers: [DashboardService],
})
export class DefaultModule {}
