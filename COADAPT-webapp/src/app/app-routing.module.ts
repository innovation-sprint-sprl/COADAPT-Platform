import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from './helpers';
import { Role } from './models';

/* Components */
import { DefaultComponent } from './layouts';
import { LoginComponent } from './login/login.component';

// Error Pages
import { ForbiddenComponent, InternalServerComponent, NotFoundComponent } from './error-pages';

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
  AdministratorsListComponent,
  AdministratorsCreateComponent,
  AdministratorsEditComponent,
  SubAdministratorsListComponent,
  SubAdministratorsCreateComponent,
  SubAdministratorsEditComponent,
  SupervisorsListComponent,
  SupervisorsCreateComponent,
  SupervisorsEditComponent,
  TherapistsListComponent,
  TherapistsCreateComponent,
  TherapistsEditComponent,
  ParticipantsListComponent,
  ParticipantsCreateComponent,
  ParticipantsEditComponent,
  IndividualPhysiologicalComponent,
  IndividualPsychologicalComponent,
  OverallDemographicsComponent,
  OverallPhysiologicalComponent,
  OverallPsychologicalComponent,
} from './modules';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: '',
    component: DefaultComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: DashboardComponent,
      },
      {
        path: 'organizations',
        component: OrganizationsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'organizations/create',
        component: OrganizationsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'organizations/edit/:id',
        component: OrganizationsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'studies',
        component: StudiesListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'studies/create',
        component: StudiesCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'studies/edit/:id',
        component: StudiesEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'sites',
        component: SitesListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'sites/create',
        component: SitesCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'sites/edit/:id',
        component: SitesEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'groups',
        component: GroupsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'groups/create',
        component: GroupsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'groups/edit/:id',
        component: GroupsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'administrators',
        component: AdministratorsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'administrators/create',
        component: AdministratorsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'administrators/edit/:id',
        component: AdministratorsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'sub-administrators',
        component: SubAdministratorsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'sub-administrators/create',
        component: SubAdministratorsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'sub-administrators/edit/:id',
        component: SubAdministratorsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator] }
      },
      {
        path: 'supervisors',
        component: SupervisorsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator] }
      },
      {
        path: 'supervisors/create',
        component: SupervisorsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator] }
      },
      {
        path: 'supervisors/edit/:id',
        component: SupervisorsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator] }
      },
      {
        path: 'therapists',
        component: TherapistsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'therapists/create',
        component: TherapistsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'therapists/edit/:id',
        component: TherapistsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor] }
      },
      {
        path: 'participants',
        component: ParticipantsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'participants/create',
        component: ParticipantsCreateComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'participants/edit/:code',
        component: ParticipantsEditComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'overall/demographics',
        component: OverallDemographicsComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'overall/physiological',
        component: OverallPhysiologicalComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'overall/psychological',
        component: OverallPsychologicalComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Administrator, Role.SubAdministrator, Role.Supervisor, Role.Therapist] }
      },
      {
        path: 'individual/physiological',
        component: IndividualPhysiologicalComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Participant] }
      },
      {
        path: 'individual/psychological',
        component: IndividualPsychologicalComponent,
        canActivate: [AuthGuard],
        data: { roles: [Role.Participant] }
      },
    ],
  },
  {
    path: '403',
    component: ForbiddenComponent,
  },
  {
    path: '500',
    component: InternalServerComponent,
  },
  {
    path: '404',
    component: NotFoundComponent,
  },
  {
    path: '**',
    redirectTo: '/404',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
