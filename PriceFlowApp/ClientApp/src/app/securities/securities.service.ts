import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Security } from './Security';
import { CreateSecurity } from './CreateSecurity';
import { Issuer } from './Issuer';
import { SecurityDailyPrices } from './SecurityDailyPrices';
import { TypeSecurity } from './TypeSecurity';
import { UpdateSecurity } from './UpdateSecurity';

@Injectable({
  providedIn: 'root'
})

export class SecuritiesService {
  private apiUrl = "api/securities";
  private typesUrl = "api/type-security";
  private issuersUrl = "api/issuers";

  constructor(private http: HttpClient) { }

  getById(id: number): Observable<Security> {
    return this.http.get<Security>(`${this.apiUrl}/${id}`);
  }

  getAll(): Observable<Security[]> {
    return this.http.get<Security[]>(this.apiUrl);
  }

  getSecurityCode(id: number): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/code/${id}`);
  }

  add(security: CreateSecurity): Observable<Security> {
    return this.http.post<Security>(this.apiUrl, security);
  }

  update(id: number, security: UpdateSecurity): Observable<Security> {
    return this.http.put<Security>(`${this.apiUrl}/${id}`, security);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getTypes(): Observable<TypeSecurity[]> {
    return this.http.get<TypeSecurity[]>(this.typesUrl);
  }

  getIssuers(): Observable<Issuer[]> {
    return this.http.get<Issuer[]>(this.issuersUrl);
  }

  getLatestPrices(securityCode: string, date: string): Observable<SecurityDailyPrices> {
    return this.http.get<SecurityDailyPrices>(`${this.apiUrl}/prices/`,
      { params: { securityCode, date }, withCredentials: true });
  }

  searchByCode(searchTerm: string): Observable<Security[]> {
    return this.http.get<Security[]>(`${this.apiUrl}/search`,
      { params: { searchTerm } });
  }
}
