export interface Transaction {
  id: number;
  securityCode: string;
  sharesQuantity: number;
  sharesUnitPrice: number;
  amount: number;
  typeTransaction: 'Купување' | 'Продавање';
  isReal: boolean;
  stockExchangeCommission: number;
  brokerageCommission: number;
  cdhvCommission: number;
  date: string;
}
