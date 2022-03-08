import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

/* Material for Login */
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

/* Default Module */
import { DefaultModule } from './layouts/default/default.module';

/* Helpers */
import { JwtInterceptor, ErrorInterceptor } from './helpers';

/* Components */
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
// Error Pages
import { ForbiddenComponent, InternalServerComponent, NotFoundComponent } from './error-pages';
import { IndividualPsychologicalReportsComponent } from './modules/individual/individual-psychological-reports/individual-psychological-reports.component';
import { IndividualPhysiologicalMetricsComponent } from './modules/individual/individual-physiological-metrics/individual-physiological-metrics.component';
import { ChartsModule } from 'ng2-charts';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NotFoundComponent,
    InternalServerComponent,
    ForbiddenComponent,
    IndividualPsychologicalReportsComponent,
    IndividualPhysiologicalMetricsComponent,
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    ChartsModule,
    FormsModule,
    HttpClientModule,
    DefaultModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
