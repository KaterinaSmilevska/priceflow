import { Component, OnInit } from '@angular/core';
import { Broker, BrokerService } from './broker.service';

@Component({
  selector: 'app-broker',
  templateUrl: './broker.component.html',
  styleUrls: ['./broker.component.css']
})
export class BrokerComponent implements OnInit {
  kompanija: string = 'Еурохаус АД Скопје';
  broker: Broker | null = null;
  error: string | null = null;

  constructor(private brokerService: BrokerService) { }

  ngOnInit(): void { }

  getBroker(): void {
    this.brokerService.getBroker(this.kompanija).subscribe({
      next: (broker) => {
        this.broker = broker;
        this.error = null;
      },
      error: (err) => {
        this.broker = null;
        this.error = err.error?.message || 'Failed to fetch broker';
      }
    });
  }
}
