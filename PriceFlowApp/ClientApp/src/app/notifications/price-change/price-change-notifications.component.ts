import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { PriceChangeNotificationsService } from './price-change-notifications.service';
import { PriceChangeNotification } from './PriceChangeNotification';
import { PriceChangeNotificationsDropDownComponent } from './price-change-dropdown/price-change-notifications-dropdown.component';

@Component({
  selector: 'app-price-change-notifications',
  standalone: true,
  imports: [CommonModule, PriceChangeNotificationsDropDownComponent],
  templateUrl: './price-change-notifications.component.html',
  styleUrl: './price-change-notifications.component.css',
})
export class PriceChangeNotificationsComponent implements OnInit {
  notifications: PriceChangeNotification[] = [];
  unreadCount: number = 0;
  showDropdown = false;

  constructor(private notificationsService: PriceChangeNotificationsService, private elementRef: ElementRef) { }

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications() {
    this.notificationsService.getNotifications()
      .subscribe(data => {
        this.notifications = data;
        this.unreadCount = data.filter(n => !n.isRead).length;
      });
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;

    if (this.showDropdown) {
      setTimeout(() => {
        const dropdown = document.querySelector('.notification-dropdown') as HTMLElement;
        if (!dropdown) return;

        const rect = dropdown.getBoundingClientRect();
        const spaceBelow = window.innerHeight - rect.top;

        if (spaceBelow < rect.height) {
          dropdown.classList.add('flipped');
        } else {
          dropdown.classList.remove('flipped');
        }
      });
    }
  }

  markAsRead(id: number) {
    this.notificationsService.markNotificationAsRead(id)
      .subscribe(() => {
        const notif = this.notifications.find(n => n.id === id);
        if (notif) notif.isRead = true;
        this.unreadCount = this.notifications.filter(n => !n.isRead).length;
      });
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (!this.elementRef.nativeElement.contains(event.target))
    {
      this.showDropdown = false;
    }
  }
}
