export interface Budget {
  budgetId: number;
  monthlyLimit: number;
  month: number;
  year: number;
}

export interface CreateBudgetRequest {
  monthlyLimit: number;
  month: number;
  year: number;
}

export interface UpdateBudgetRequest {
  monthlyLimit: number;
}
