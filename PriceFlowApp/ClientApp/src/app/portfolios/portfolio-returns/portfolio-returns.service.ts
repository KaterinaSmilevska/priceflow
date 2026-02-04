import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

export interface PortfolioReturns {
  date: string;
  netAmount: number;
  tax: number;
  portfolioId: number;
  hvId: number;
}

export interface PortfolioReturnsSummary {
  totalDividends: number;
  totalTaxes: number;
}

@Injectable({ providedIn: 'root' })
export class PortfolioReturnsService {
  private apiUrl = 'api/portfolio-returns';

  constructor(private http: HttpClient) { }

  create(model: PortfolioReturns): Observable<PortfolioReturns> {
    return this.http.post<PortfolioReturns>(this.apiUrl, model);
  }

  getSummary(portfolioId: number): Observable<PortfolioReturnsSummary>  {
    return this.http.get<PortfolioReturnsSummary>(`${this.apiUrl}/summary/${portfolioId}`);
  }

  getByPortfolioId(portfolioId: number): Observable<PortfolioReturns[]> {
    return this.http.get<PortfolioReturns[]>(`${this.apiUrl}/${portfolioId}`);
  }
}
