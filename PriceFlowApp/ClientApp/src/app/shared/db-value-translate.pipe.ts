import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: 'dbTranslate',
  standalone: true
})
export class DbValueTranslatePipe implements PipeTransform {
  transform(value: string | null | undefined): string {

    switch (value) {
      case 'Купување':
        return 'TRANSACTIONS.BUY';

      case 'Продавање':
        return 'TRANSACTIONS.SELL';

      case 'Дивиденден принос':
        return 'PORTFOLIOS.DIVIDEND';

      case 'Акции':
        return 'SHARES';

      case 'Обврзници':
        return 'BONDS';

      case 'Администратор':
        return 'ADMIN.TITLE';

      case 'Инвеститор':
        return 'INVESTOR';

      case 'Аналитичар':
        return 'ANALYST';

      case 'Обичен корисник':
        return 'USERS.USER';

      default:
        return value ?? '';
    }
  }
}
