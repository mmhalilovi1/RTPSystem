import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { PositionListComponent } from './features/positions/position-list.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'positions', component: PositionListComponent },
  { path: '', redirectTo: '/positions', pathMatch: 'full' },
  { path: '**', redirectTo: '/positions' }
];
