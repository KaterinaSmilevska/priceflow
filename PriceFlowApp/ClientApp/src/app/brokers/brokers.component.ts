import { Component, OnInit } from '@angular/core';
import { BrokersService } from './brokers.service';
import { CommonModule } from '@angular/common';
import { LoginService } from '../auth/login/login.service';
import { TranslateModule } from '@ngx-translate/core';
import { Broker } from './Broker';

@Component({
  selector: 'app-brokers',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './brokers.component.html',
  styleUrls: ['./brokers.component.css']
})
export class BrokersComponent implements OnInit {
  brokers: Broker[] = [];
  errorMessage: string | null = null;
  loading = false;

  constructor(private brokersService: BrokersService, private loginService: LoginService) { }

  ngOnInit(): void {
    this.loadBrokers();
  }

  loadBrokers() {
    this.loading = true;
    this.errorMessage = null;

    this.brokersService.getAll().subscribe({
      next: data => {
        this.brokers = data,
          this.loading = false;
      },
      error: err => {
        this.errorMessage = 'LOADING_DATA_ERROR';
        this.loading = false;
      }
    });
  }
}
