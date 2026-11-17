import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { RouterLink } from '@angular/router';
import { DashboardSummary } from '../../../core/services/models/dashboard';
import { DashboardService } from '../../../core/services/services/dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  dashboard: DashboardSummary | null = null;

  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    console.log('Dashboard API request started');

    this.dashboardService
      .getDashboardSummary()
      .pipe(
        finalize(() => {
          console.log('Dashboard API request completed');
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (response: DashboardSummary) => {
          console.log('Dashboard API response:', response);

          this.dashboard = response;

          console.log('Dashboard data assigned:', this.dashboard);
        },
        error: (error: HttpErrorResponse) => {
          console.error('Dashboard API error:', error);
          this.handleError(error);
        },
      });
  }

  get currentMonthName(): string {
    return new Date().toLocaleString('en-IN', {
      month: 'long',
      year: 'numeric',
    });
  }

  get monthlyChartMax(): number {
    if (!this.dashboard?.monthlySummaries.length) {
      return 1;
    }

    const highestValue = Math.max(
      ...this.dashboard.monthlySummaries.map((month) => Math.max(month.income, month.expenses)),
    );

    return highestValue > 0 ? highestValue : 1;
  }

  get categoryTotal(): number {
    return (
      this.dashboard?.categoryExpenses.reduce((total, category) => total + category.amount, 0) ?? 0
    );
  }

  getBudgetProgress(): number {
    const percentage = this.dashboard?.currentMonthBudget?.usagePercentage ?? 0;

    return Math.min(Math.max(percentage, 0), 100);
  }

  getCategoryPercentage(amount: number): number {
    if (this.categoryTotal === 0) {
      return 0;
    }

    return (amount / this.categoryTotal) * 100;
  }

  getIncomeBarHeight(amount: number): number {
    if (!this.dashboard) {
      return 0;
    }

    return (amount / this.monthlyChartMax) * 100;
  }

  getExpenseBarHeight(amount: number): number {
    if (!this.dashboard) {
      return 0;
    }

    return (amount / this.monthlyChartMax) * 100;
  }

  trackByTransaction(index: number, transaction: { transactionId: number }): number {
    return transaction.transactionId;
  }

  trackByCategory(index: number, category: { category: string }): string {
    return category.category;
  }

  private handleError(error: HttpErrorResponse): void {
    if (error.status === 401) {
      this.errorMessage = 'Your session has expired. Please log in again.';
      return;
    }

    if (error.status === 403) {
      this.errorMessage = 'You are not authorized to view this dashboard.';
      return;
    }

    if (error.status === 0) {
      this.errorMessage = 'Cannot connect to the backend. Please check whether the API is running.';
      return;
    }

    this.errorMessage =
      error.error?.message ??
      error.error?.title ??
      'Unable to load dashboard information. Please try again.';
  }
}
