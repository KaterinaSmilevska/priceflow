import { Component, Input } from '@angular/core';
import { SecurityLiquidity } from '../SecurityLiquidity';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-liquidity-table',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './liquidity-table.component.html',
  styleUrl: './liquidity-table.component.css',
})
export class LiquidityTableComponent {
  @Input() title!: string;
  @Input() data: SecurityLiquidity[] | undefined = [];
}
