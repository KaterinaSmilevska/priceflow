import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { Notification } from '../notifications/Notification';
import { NotificationsService } from './notifications.service';
import { NotificationsDropDownComponent } from './notifications-dropdown.component';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, NotificationsDropDownComponent],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.css',
})
export class NotificationsComponent implements OnInit {
  notifications: Notification[] = [];
  unreadCount: number = 0;
  showDropdown = false;

  constructor(private notificationsService: NotificationsService, private elementRef: ElementRef) { }

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
