import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

export interface Security {
  id: number;
  code: string;
}

export interface PriceTrend {
  date: string;
  price: number;
}

export interface SectorDistribution {
  sectorName: string;
  marketCap: number;
}

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

  getSecurities(): Observable<Security[]> {
    return this.http.get<Security[]>(`${this.baseUrl}/securities`);
  }

  getLatestDate(): Observable<string> {
    return this.http.get<string>(`${this.baseUrl}/latest-date`);
  }
}


