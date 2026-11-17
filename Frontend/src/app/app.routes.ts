import { Routes } from '@angular/router';

import { Transactions } from './features/transactions/transactions/transactions';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
  },
  {
    path: 'welcome',
    loadComponent: () => import('./features/auth/welcome/welcome').then((m) => m.Welcome),
  },
  {
    path: 'transactions',
    component: Transactions,
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
