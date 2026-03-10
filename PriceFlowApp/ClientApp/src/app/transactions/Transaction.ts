export interface Transaction {
  id: number;
  hvCode: string;
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
