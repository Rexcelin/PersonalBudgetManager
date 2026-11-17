export type TransactionType = 'Income' | 'Expense';

export interface Transaction {
  transactionId: number;
  title: string;
  amount: number;
  type: TransactionType;
  category: string;
  transactionDate: string;
  description?: string | null;
}

export interface CreateTransactionRequest {
  title: string;
  amount: number;
  type: TransactionType;
  category: string;
  transactionDate: string;
  description?: string | null;
}

export interface UpdateTransactionRequest {
  title: string;
  amount: number;
  type: TransactionType;
  category: string;
  transactionDate: string;
  description?: string | null;
}
