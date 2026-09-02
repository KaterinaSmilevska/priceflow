import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { PriceChangeNotification } from "../PriceChangeNotification";
import { TranslateModule } from "@ngx-translate/core";

@Component({
  selector: 'app-price-change-notifications-dropdown',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './price-change-notifications-dropdown.component.html',
  styleUrl: './price-change-notifications-dropdown.component.css',
})

export class PriceChangeNotificationsDropDownComponent{
  @Input() notifications: PriceChangeNotification[] = [];
  @Output() markRead = new EventEmitter<number>();

  onMarkAsRead(id: number) {
    this.markRead.emit(id);
  } 
}
