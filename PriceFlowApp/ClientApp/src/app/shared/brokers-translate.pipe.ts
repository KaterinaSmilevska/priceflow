import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: 'brokersTranslate',
  standalone: true
})
export class BrokersTranslatePipe implements PipeTransform {
  transform(value: string | null | undefined): string {

    switch (value) {
      case 'Комерцијална банка АД Скопје':
        return 'BROKERS.TRANSLATE_NAMES.KOMERCIJALNA_BANKA'

      case 'Стопанска банка АД Скопје':
        return 'BROKERS.TRANSLATE_NAMES.STOPANSKA_BANKA'

      case 'Илирика Инвестментс АД Скопје':
        return 'BROKERS.TRANSLATE_NAMES.ILIRIKA_INVESTMENTS'

      case 'Фершпед брокер АД Скопје':
        return 'BROKERS.TRANSLATE_NAMES.FERSHPED_BROKER'

      case 'Еурохаус АД Скопје':
        return 'BROKERS.TRANSLATE_NAMES.EUROHAUS'

      default:
        return value ?? '';
    }
  }
}
