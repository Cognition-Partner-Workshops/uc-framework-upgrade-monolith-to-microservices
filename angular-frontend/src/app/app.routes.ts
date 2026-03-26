import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LossReportListComponent } from './components/loss-reports/loss-report-list/loss-report-list.component';
import { LossReportCreateComponent } from './components/loss-reports/loss-report-create/loss-report-create.component';
import { LossReportEditComponent } from './components/loss-reports/loss-report-edit/loss-report-edit.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'loss-reports', component: LossReportListComponent },
  { path: 'loss-reports/create', component: LossReportCreateComponent },
  { path: 'loss-reports/edit/:id', component: LossReportEditComponent },
  { path: '**', redirectTo: '' }
];
