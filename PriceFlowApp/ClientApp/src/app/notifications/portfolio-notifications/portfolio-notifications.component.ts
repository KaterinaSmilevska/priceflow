import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { PortfolioNotificationsService } from './portfolio-notifications.service';
import { PortfolioNotification } from './PortfolioNotification';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-portfolio-notifications',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './portfolio-notifications.component.html',
  styleUrl: './portfolio-notifications.component.css',
})
export class PortfolioNotificationsComponent implements OnInit {
  @Input() portfolioId!: number;

  notification: PortfolioNotification = {
    portfolioId: 0,
    isEnabled: false,
    frequency: 'Weekly'
  };

  loading = true;
  saving = false;
  saved = false;
  error = false;

  constructor(private notificationsService: PortfolioNotificationsService) { }

  ngOnInit(): void {
    this.loadNotification();
  }

  loadNotification() {
    this.notificationsService.getByPortfolioId(this.portfolioId)
      .subscribe({
        next: (res) => {
          this.notification = res;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.error = true;
        }
      });
  }

  save() {
    this.saving = true;
    this.saved = false;
    this.error = false;

    this.notificationsService.update(this.notification)
      .subscribe({
        next: () => {
          this.saving = false;
          this.saved = true;
        },
        error: () => {
          this.saving = false;
          this.error = true;
        }
      });
  }
}
