import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { Notification } from "./Notification";

@Component({
  selector: 'app-notifications-dropdown',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications-dropdown.component.html',
  styleUrl: './notifications-dropdown.component.css',
})

export class NotificationsDropDownComponent {
  @Input() notifications: Notification[] = [];
  @Output() markRead = new EventEmitter<number>();

  onMarkAsRead(id: number) {
    this.markRead.emit(id);
  } 
}
