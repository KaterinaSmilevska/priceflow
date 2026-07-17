import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Observable, map } from 'rxjs';
import { LoginService } from '../auth/login/login.service';
import { PriceChangeNotificationsService } from '../notifications/price-change/price-change-notifications.service';
import { PriceChangeNotification } from '../notifications/price-change/PriceChangeNotification';
import { PriceChangeNotificationsComponent } from '../notifications/price-change/price-change-notifications.component';
import { ThemeService } from '../theme.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, PriceChangeNotificationsComponent, TranslateModule],
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css', '../../styles.css']
})
export class NavMenuComponent implements OnInit {
  successMessage: string | null = null;
  isLoggedIn = false;
  isExpanded = false;
  isAdmin$: Observable<boolean>
  private previousLoginState = false;

  isEnglish = false;

  notifications: PriceChangeNotification[] = [];
  unreadCount = 0;
  showDropdown = false;

  constructor(public loginService: LoginService, private notificationsService: PriceChangeNotificationsService,
    public themeService: ThemeService, public translateService: TranslateService) {
    this.isAdmin$ = this.loginService.getUserRoles().pipe(
      map(roles => roles.includes('Администратор')));
  }

  ngOnInit(): void {
    this.isEnglish = this.translateService.getCurrentLang() === 'en';

    this.translateService.onLangChange.subscribe(event => {
      this.isEnglish = event.lang === 'en';
    });

     this.loginService.isLoggedIn().subscribe(currentState => {
       if (this.previousLoginState != currentState) {
         if (currentState) {
           this.loadNotifications();
           this.showSuccessMessage(this.translateService.instant('AUTH.LOGIN_SUCCESS_TOOLTIP'));
         } else if (this.previousLoginState) {
           this.showSuccessMessage(this.translateService.instant('AUTH.LOGOUT_SUCCESS_TOOLTIP'));
         }
       }
       this.previousLoginState = currentState;
       this.isLoggedIn = currentState;
     });
     this.notificationsService.unreadCount$
       .subscribe(count => this.unreadCount = count);
  }

  loadNotifications(): void {
    this.notificationsService.getNotifications()
      .subscribe(res => {
        this.notifications = res;
      });
    this.notificationsService.getUnreadNotificationCount();
  }

  markAsRead(notification: PriceChangeNotification): void {
    if (!notification.isRead) {
      this.notificationsService.markNotificationAsRead(notification.id)
        .subscribe(() => {
          notification.isRead = true;

          this.notificationsService.getUnreadNotificationCount();
        });
    }
  }

  isAdmin(): boolean {
    return this.loginService.hasRole('Администратор');
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  openLoginModal() {
    window.location.href = '/login';
  }

  private showSuccessMessage(message: string) {
    this.successMessage = message;
    setTimeout(() => {
      this.successMessage = null;
    }, 3000);
  }

  toggleLanguage() {
    //const current = this.translateService.getCurrentLang();

    const next = this.isEnglish ? 'mk' : 'en';

    this.translateService.use(next);
    localStorage.setItem('lang', next);
  }
}
