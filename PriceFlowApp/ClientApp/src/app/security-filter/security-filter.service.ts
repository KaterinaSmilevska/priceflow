import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Sector } from "./Sector";
import { FilteredSecurity } from "./FilteredSecurity";
import { Security } from "../securities/securities.service";

@Injectable({
  providedIn: 'root'
})
export class SecurityFilterService {
  private apiUrl = 'api/security-filter';

  constructor(private http: HttpClient) { }

  getMostProfitableSecuritiesByDividendYield(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/most-profitable/by-dividend-yield`);
  }

  getMostProfitableSecuritiesByDividendPerShare(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/most-profitable/by-dividend-per-share`);
  }

  getSecuritiesWithBiggestPriceOscillations(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/price-oscillation/biggest`);
  }

  getSecuritiesWithSmallestPriceOscillations(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/price-oscillation/smallest`);
  }

  getLeastLiquidSecuritiesByTradedQuantity(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/liquidity/least-by-traded-quantity`);
  }

  getMostLiquidSecuritiesByTradedQuantity(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/liquidity/most-by-traded-quantity`);
  }

  getLeastLiquidSecuritiesByNumTradingDays(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/liquidity/least-by-trading-days`);
  }

  getMostLiquidSecuritiesByNumTradingDays(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/liquidity/most-by-trading-days`);
  }

  getMostProfitableSectorsByDividendYield(): Observable<Sector[]> {
    return this.http.get<Sector[]>(`${this.apiUrl}/sectors/most-profitable/by-dividend-yield`);
  }

  getMostProfitableSectorsByProfit(): Observable<Sector[]> {
    return this.http.get<Sector[]>(`${this.apiUrl}/sectors/most-profitable/by-profit`);
  }

  getSecuritiesValuation(): Observable<FilteredSecurity[]> {
    return this.http.get<FilteredSecurity[]>(`${this.apiUrl}/securities/valuation`);
  }
}
