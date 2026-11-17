export interface BudgetOverview {
  monthlyLimit: number;
  amountSpent: number;
  remainingAmount: number;
  usagePercentage: number;
  isOverBudget: boolean;
}

export interface MonthlySummary {
  month: number;
  year: number;
  monthName: string;
  income: number;
  expenses: number;
}

export interface CategoryExpense {
  category: string;
  amount: number;
}

export interface RecentTransaction {
  transactionId: number;
  title: string;
  amount: number;
  type: string;
  category: string;
  transactionDate: string;
  description: string | null;
}

export interface DashboardSummary {
  totalIncome: number;
  totalExpenses: number;
  balance: number;

  currentMonthIncome: number;
  currentMonthExpenses: number;

  currentMonthBudget: BudgetOverview | null;

  monthlySummaries: MonthlySummary[];
  categoryExpenses: CategoryExpense[];
  recentTransactions: RecentTransaction[];
}
