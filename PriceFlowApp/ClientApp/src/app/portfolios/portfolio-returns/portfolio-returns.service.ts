import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PortfolioReturns } from "./PortfolioReturns";
import { PortfolioReturnsSummary } from "./PortfolioReturnsSummary";


@Injectable({ providedIn: 'root' })
export class PortfolioReturnsService {
  private apiUrl = 'api/portfolio-returns';

  constructor(private http: HttpClient) { }

  getByPortfolioId(portfolioId: number): Observable<PortfolioReturns[]> {
    return this.http.get<PortfolioReturns[]>(`${this.apiUrl}/${portfolioId}`);
  }

  add(model: PortfolioReturns): Observable<PortfolioReturns> {
    return this.http.post<PortfolioReturns>(this.apiUrl, model);
  }

  getSummary(portfolioId: number): Observable<PortfolioReturnsSummary> {
    return this.http.get<PortfolioReturnsSummary>(`${this.apiUrl}/summary/${portfolioId}`);
  }
}
