import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { PriceChangeNotification } from "./PriceChangeNotification";


@Injectable({
  providedIn: 'root'
})
export class PriceChangeNotificationsService {
  private apiUrl = "api/price-change-notification";

  private unreadCount = new BehaviorSubject<number>(0);
  unreadCount$ = this.unreadCount.asObservable();

  constructor(private http: HttpClient) { }

  getNotifications(): Observable<PriceChangeNotification[]> {
    return this.http.get<PriceChangeNotification[]>(this.apiUrl);
  }

  getUnreadNotificationCount(): void {
    this.http.get<number>(`${this.apiUrl}/unread-count`)
      .subscribe(count => {
        this.unreadCount.next(count);
      });
  }

  markNotificationAsRead(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/read`, {});
  }
}
