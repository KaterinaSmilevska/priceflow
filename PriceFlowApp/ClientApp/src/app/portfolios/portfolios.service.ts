import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Portfolio {
  id: number;
  name: string;
  description: string | null;
}

export interface CreatePortfolio {
  name: string;
  description: string | null;
}

export interface UpdatePortfolio {
  name: string;
  description: string | null;
}

export interface PortfolioAnalytics {
  totalRevenue: number;
  totalExpenses: number;
  balance: number;
  taxes: number;
}

@Injectable({ providedIn: 'root' })
export class PortfoliosService {
  private apiUrl = 'api/portfolios';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Portfolio[]> {
    return this.http.get<Portfolio[]>(this.apiUrl, { withCredentials: true });
  }

  getById(id: number): Observable<Portfolio> {
    return this.http.get<Portfolio>(`${this.apiUrl}/${id}`, {
      withCredentials: true
    });
  }

  createPortfolio(portfolio: CreatePortfolio): Observable<Portfolio> {
    return this.http.post<Portfolio>(this.apiUrl, portfolio, { withCredentials: true });
  }

  updatePortfolio(id: number, portfolio: UpdatePortfolio): Observable<Portfolio> {
    return this.http.put<Portfolio>(`${this.apiUrl}/${id}`, portfolio, { withCredentials: true });
  }

  deletePortfolio(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { withCredentials: true });
  }
}
