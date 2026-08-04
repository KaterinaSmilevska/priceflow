import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PriceTrend } from "./PriceTrend";
import { Security } from "./Security";
import { PortfolioIncome } from "./PortfolioIncome";
import { SectorDistribution } from "./SectorDistribution";
import { SecurityAllocation } from "./SecurityAllocation";

@Injectable({
  providedIn: 'root'
})

export class ChartService {
  private baseUrl = 'api/chart';

  constructor(private http: HttpClient) { }

  public formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  getPriceTrend(securityId: number, startDate: string, endDate: string): Observable<PriceTrend[]> {
    return this.http.get<PriceTrend[]>(`${this.baseUrl}/price-trend`, {
      params: { securityId, startDate, endDate }
    });
  }

  getSectorDistribution(date: string): Observable<SectorDistribution[]> {
    return this.http.get<SectorDistribution[]>(`${this.baseUrl}/sector-distribution`, {
      params: { date }
    });
  }

  getPortfolioIncome(portfolioId: number, isReal:  boolean): Observable<PortfolioIncome[]> {
    return this.http.get<PortfolioIncome[]>(`${this.baseUrl}/portfolio-income`, {
      params: { portfolioId, isReal }
    });
  }

  getSecurityAllocation(portfolioId: number, isReal: boolean): Observable<SecurityAllocation[]> {
    return this.http.get<SecurityAllocation[]>(`${this.baseUrl}/portfolio-security-allocation`, {
      params: { portfolioId, isReal }
    });
  }

  getSecurities(): Observable<Security[]> {
    return this.http.get<Security[]>(`${this.baseUrl}/securities`);
  }

  getLatestDate(): Observable<string> {
    return this.http.get<string>(`${this.baseUrl}/latest-date`);
  }

  public generateColors(count: number): string[] {
    return Array.from({ length: count }, (_, i) =>
      `hsl(${(i * 360) / count}, 65%, 55%)`
    );
  }
}


