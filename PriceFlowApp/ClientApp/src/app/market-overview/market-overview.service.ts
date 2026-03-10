import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LiquidityOverview } from '../liquidity/LiquidityOverview';
import { MarketOverview } from './MarketOverview';
import { SecurityPerformance } from './SecurityPerformance';

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

  getLiquidity(months: number, onlyOwned = true) {
    return this.http.get<LiquidityOverview>(`${this.apiUrl}/liquidity`,
      {
        params: { months, onlyOwned },
        withCredentials: true
      });
  }
}
