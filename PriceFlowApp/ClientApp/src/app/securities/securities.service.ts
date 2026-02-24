import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Security {
  id: number;
  isin: string;
  code: string;
  typeSecurityName: string;
  issuerName: string;
  totalNumShares: number;
}

export interface CreateSecurity {
  isin: string,
  code: string,
  typeSecurityId: number,
  issuerId: number,
  totalNumShares: number
}

export interface TypeSecurity {
  id: number;
  name: string;
}

export interface Issuer {
  id: number;
  name: string;
}

export interface SecurityDailyPrices {
  securityCode: string;
  minPrice: number | null;
  maxPrice: number | null;
  averagePrice: number | null;
}

@Injectable({
  providedIn: 'root'
})

export class SecuritiesService {
  private apiUrl = "api/securities";
  private typesUrl = "api/type-security";
  private issuersUrl = "api/issuers";

  constructor(private http: HttpClient) { }

  getAll(): Observable<Security[]> {
    return this.http.get<Security[]>(this.apiUrl);
  }

  getById(id: number): Observable<Security> {
    return this.http.get<Security>(`${this.apiUrl}/${id}`);
  }

  getSecurityCode(id: number): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/code/${id}`);
  }

  addSecurity(security: CreateSecurity): Observable<Security> {
    return this.http.post<Security>(this.apiUrl, security);
  }

  getTypes(): Observable<TypeSecurity[]> {
    return this.http.get<TypeSecurity[]>(this.typesUrl);
  }

  getIssuers(): Observable<Issuer[]> {
    return this.http.get<Issuer[]>(this.issuersUrl);
  }

  deleteSecurity(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  updateSecurity(id: number, security: CreateSecurity): Observable<Security> {
    return this.http.put<Security>(`${this.apiUrl}/${id}`, security);
  }

  getLatestPrices(securityCode: string, date: string): Observable<SecurityDailyPrices> {
    return this.http.get<SecurityDailyPrices>(`${this.apiUrl}/prices/`,
      { params: { securityCode, date }, withCredentials: true });
  }
}
