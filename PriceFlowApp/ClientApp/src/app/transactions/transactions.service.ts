import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PortfolioAnalytics } from "../portfolios/PortfolioAnalytics";
import { PortfolioValue } from "./PortfolioValue";
import { Transaction } from "./Transaction";

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

  delete(portfolioId: number, transactionId: number): Observable<Transaction> {
    return this.http.delete<Transaction>(
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

  getOwnedSharesAtDate(portfolioId: number, code: string, isReal: boolean, date: string, transactionIdToExclude?: number) {
    const params: any = { code, isReal, date };

    if (transactionIdToExclude !== undefined) {
      params.transactionIdToExclude = transactionIdToExclude;
    }
    return this.http.get<number>(`${this.apiUrl}/${portfolioId}/transactions/owned-shares-date`,
      { params, withCredentials: true });
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
