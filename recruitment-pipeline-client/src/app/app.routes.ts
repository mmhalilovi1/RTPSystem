import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { PositionListComponent } from './features/positions/position-list.component';
import { PositionFormComponent } from './features/positions/position-form.component';
import { PositionDetailComponent } from './features/positions/position-detail.component';
import { CandidateProfileComponent } from './features/candidates/candidate-profile.component';
/*import { MyApplicationsComponent } from './features/applications/my-applications.component';
import { ApplicationDetailComponent } from './features/applications/application-detail.component';*/
import { AdminRecruitersComponent } from './features/admin/admin-recruiters.component';
import { roleGuard } from './core/guards/role.guard';
import { authGuard } from './core/guards/auth.guard';
import { excludeRolesGuard } from './core/guards/exclude-roles.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'positions', component: PositionListComponent },
  {
    path: 'positions/new',
    component: PositionFormComponent,
    canActivate: [roleGuard(['Admin', 'Recruiter'])]
  },
  { path: 'positions/:id', component: PositionDetailComponent },
  {
    path: 'my-profile',
    component: CandidateProfileComponent,
    canActivate: [excludeRolesGuard(['Admin', 'Recruiter'])]
  },
  /*{
    path: 'my-applications',
    //component: MyApplicationsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'applications/:id',
    //component: ApplicationDetailComponent,
    canActivate: [authGuard]
  },*/
  {
    path: 'admin/recruiters',
    component: AdminRecruitersComponent,
    canActivate: [roleGuard(['Admin'])]
  },
  { path: '', redirectTo: '/positions', pathMatch: 'full' },
  { path: '**', redirectTo: '/positions' }
];
