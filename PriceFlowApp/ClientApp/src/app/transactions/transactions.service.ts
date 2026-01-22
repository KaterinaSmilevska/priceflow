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

@Injectable({
  providedIn: 'root'
})

export class TransactionsService {
  private apiUrl = 'api/portfolios';

  constructor(private http: HttpClient) { }

  getByPortfolioId(portfolioId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(
      `${this.apiUrl}/${portfolioId}/transactions`
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
      `${this.apiUrl}/${portfolioId}/transactions/${transactionId}`
    );
  }

  getAnalytics(portfolioId: number): Observable<PortfolioAnalytics> {
    return this.http.get<PortfolioAnalytics>(`${this.apiUrl}/${portfolioId}/transactions/analytics`, { withCredentials: true });
  }
}
