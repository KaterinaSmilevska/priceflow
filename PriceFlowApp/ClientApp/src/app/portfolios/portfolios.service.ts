import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PortfolioPerformanceSummary } from './performance-export/PortfolioPerformanceSummary';
import { Portfolio } from './Portfolio';
import { CreatePortfolio } from './CreatePortfolio';
import { UpdatePortfolio } from './UpdatePortfolio';
import { SecuritiesPriceTrend } from './SecuritiesPriceTrend';
import { SecurityPriceTrendReport } from './securities-price-trend/SecurityPriceTrendReport';

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

  add(portfolio: CreatePortfolio): Observable<Portfolio> {
    return this.http.post<Portfolio>(this.apiUrl, portfolio, { withCredentials: true });
  }

  update(id: number, portfolio: UpdatePortfolio): Observable<Portfolio> {
    return this.http.put<Portfolio>(`${this.apiUrl}/${id}`, portfolio, { withCredentials: true });
  }

  delete(id: number): Observable<Portfolio> {
    return this.http.delete<Portfolio>(`${this.apiUrl}/${id}`, { withCredentials: true });
  }

  getSecuritiesPriceTrend(period?: 'Monthly' | 'Yearly', resolution?: 'Day' | 'Week' | 'Month' | 'Quarter') {
    const params: any = {}

    if (period) {
      params.period = period;
    }

    if (resolution) {
      params.resolution = resolution;
    }

    return this.http.get<SecuritiesPriceTrend[]>(
      `${this.apiUrl}/securities-price-trend`,
      { params, withCredentials: true });
  }

  getSecuritiesPriceTrendReport(period?: string, resolution?: string, securityCode?: string) {
    let params: any = {  };

    if (period) {
      params.period = period;
    }

    if (resolution) {
      params.resolution = resolution;
    }

    if (securityCode) {
      params.securityCode = securityCode;
    }

    return this.http.get<SecurityPriceTrendReport[]>(
      `${this.apiUrl}/securities-price-trend-report`,
      { params, withCredentials: true });
  }

  getPerformanceSummary(portfolioId: number, from: string, to: string): Observable<PortfolioPerformanceSummary> {
    return this.http.get<PortfolioPerformanceSummary>(
      `${this.apiUrl}/${portfolioId}/performance-summary`,
      {
        params: {
          from,
          to
        }
      }
    );
  }

  exportPerformanceSummary(portfolioId: number, from: string, to: string, format: string) {
    return this.http.get(
      `${this.apiUrl}/${portfolioId}/performance-summary`,
      {
        params: {
          from,
          to,
          format
        },
        responseType: 'blob'
      }
    );
  }

  generateSecuritiesPriceTrendReport(period?: string, resolution?: string, securityCode?: string) {
    let params: any = { };

    if (securityCode) {
      params.securityCode = securityCode;
    }

    if (period) {
      params.period = period;
    }

    if (resolution) {
      params.resolution = resolution;
    }

    return this.http.get(
      `${this.apiUrl}/securities-price-trend-report/pdf`,
      {
        params: params,
        responseType: 'blob',
        withCredentials: true
      }
    );
  }
}
