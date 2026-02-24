import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PortfolioAnalytics } from "../portfolios/portfolios.service";

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

export interface PortfolioTableView {
  date: string;
  hvCode: string;
  type: 'Купување' | 'Продавање' | 'Дивиденден принос';

  sharesQuantity?: number;
  sharesUnitPrice?: number;

  amount: number;
  commission?: number;
  cashFlow: number;

  isReal: boolean;

  transactionId?: number;
}

export interface PortfolioValue {
  hvid: number;
  hvCode: string;
  totalQuantity: number;
  lastPrice: number;
  currentValue: number;
  isReal: boolean;
}

@Injectable({
  providedIn: 'root'
})

export class TransactionsService {
  private apiUrl = 'api/portfolios';
  private securitiesUrl = 'api/securities';

  constructor(private http: HttpClient) { }

  getByPortfolioId(portfolioId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(
      `${this.apiUrl}/${portfolioId}/transactions`,
      { withCredentials: true }
    );
  }

  add(portfolioId: number, payload: any) {
    return this.http.post<Transaction>(
      `${this.apiUrl}/${portfolioId}/transactions`,
      payload,
      { withCredentials: true }
    );
  }

  update(portfolioId: number, transactionId: number, payload: any){
    return this.http.put<Transaction>(
      `${this.apiUrl}/${portfolioId}/transactions/${transactionId}`,
      payload,
      { withCredentials: true }
    );
  }

  delete(portfolioId: number, transactionId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${portfolioId}/transactions/${transactionId}`,
      { withCredentials: true }
    );
  }

  getAnalytics(portfolioId: number, isReal: boolean): Observable<PortfolioAnalytics> {
    return this.http.get<PortfolioAnalytics>(`${this.apiUrl}/${portfolioId}/transactions/analytics`, { params: { isReal }, withCredentials: true });
  }

  getOwnedShares(portfolioId: number, code: string, isReal: boolean) {
    return this.http.get<number>(`${this.apiUrl}/${portfolioId}/transactions/owned-shares`,
      { params: { code, isReal }, withCredentials: true });
  }

  getOwnedSharesAtDate(portfolioId: number, code: string, isReal: boolean, date: string) {
    return this.http.get<number>(`${this.apiUrl}/${portfolioId}/transactions/owned-shares-date`,
      { params: { code, isReal, date }, withCredentials: true });
  }

  getTotalShares(code: string) {
    return this.http.get<number>(`${this.securitiesUrl}/${code}/total-shares`,
      { withCredentials: true }
    );
  }

  getPortfolioValue(portfolioId: number, isReal: boolean): Observable<PortfolioValue[]> {
    return this.http.get<PortfolioValue[]>(`${this.apiUrl}/${portfolioId}/transactions/value`,
      { params: { isReal }, withCredentials: true });
  }
}
