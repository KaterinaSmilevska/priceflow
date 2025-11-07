import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MarketOverview {
  totalMarketCap: number;
  averageDailyVolume: number;
  topGainer: string;
  topGainerChange: number;
  topLoser: string;
  topLoserChange: number;
  totalSecurities: number;
}

export interface SecurityPerformance {
  code: string;
  changePercent: number;
  volume: number;
}

@Injectable({
  providedIn: 'root'
})

export class MarketOverviewService {
  private apiUrl = 'api/marketoverview';

  constructor(private http: HttpClient) { }

  getOverview(): Observable<MarketOverview> {
    return this.http.get<MarketOverview>(this.apiUrl);
  }

  getTopGainers(count: number): Observable<SecurityPerformance[]> {
    return this.http.get<SecurityPerformance[]>(`${this.apiUrl}/top-gainers?count=${count}`);
  }

  getTopLosers(count: number): Observable<SecurityPerformance[]> {
    return this.http.get<SecurityPerformance[]>(`${this.apiUrl}/top-losers?count=${count}`);
  }

  getMostTraded(count: number): Observable<SecurityPerformance[]> {
    return this.http.get<SecurityPerformance[]>(`${this.apiUrl}/most-traded?count=${count}`);
  }
}
