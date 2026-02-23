import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PortfolioNotification } from "./PortfolioNotification";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class PortfolioNotificationsService {
  private apiUrl = 'api/portfolio-notification';

  constructor(private http: HttpClient) { }

  getByPortfolioId(portfolioId: number): Observable<PortfolioNotification> {
    return this.http.get<PortfolioNotification>(`${this.apiUrl}/${portfolioId}`);
  }

  update(notification: PortfolioNotification): Observable<PortfolioNotification> {
    return this.http.put<PortfolioNotification>(this.apiUrl, notification);
  }
}
