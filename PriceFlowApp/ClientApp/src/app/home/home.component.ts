import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  marketCards = [
    { title: 'Total Market Cap', value: '1.25B MKD', change: +1.5, icon: '💰' },
    { title: 'Average Daily Volume', value: '280K', change: +3.2, icon: '📊' },
    { title: 'Top Gainer', value: 'MAKEDONIJA +8.2%', change: +8.2, icon: '🚀' },
    { title: 'Top Loser', value: 'EUROMAK -5.6%', change: -5.6, icon: '📉' },
    { title: 'Listed Securities', value: '58', change: 0, icon: '🏛' },
  ];

  constructor(private router: Router) { }

  onGetStarted() {
    this.router.navigate(['/register']);
  }
}
