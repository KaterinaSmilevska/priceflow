export interface PortfolioTableView {
  date: string;
  securityCode: string;
  type: 'Купување' | 'Продавање' | 'Дивиденден принос';

  sharesQuantity?: number;
  sharesUnitPrice?: number;

  amount: number;
  commission?: number;
  cashFlow: number;

  isReal: boolean;

  transactionId?: number;
}
