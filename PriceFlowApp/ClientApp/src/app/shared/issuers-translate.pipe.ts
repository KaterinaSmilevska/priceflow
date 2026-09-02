import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: 'issuersTranslate',
  standalone: true
})
export class IssuersTranslatePipe implements PipeTransform {
  transform(value: string | null | undefined): string {

    switch (value) {
      case 'Алкалоид АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.ALKALOID';

      case 'Комерцијална банка АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.KOMERCIJALNA_BANKA';

      case 'Стопанска банка АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.STOPANSKA_BANKA';

      case 'Жито Лукс АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.ZHITO_LUKS';

      case 'ТТК банка АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.TTK_BANKA';

      case 'Гранит АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.GRANIT';

      case 'НЛБ банка АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.NLB_Banka';

      case 'Макпетрол АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.MAKPETROL';

      case 'Реплек АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.REPLEK';


      case 'Македонски Телеком АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.MK_TELEKOM';


      case 'Пелистерка АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.PELISTERKA';


      case 'Македонијатурист АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.MAKEDONIJATURIST';


      case 'Алта банка АД Битола':
        return 'ISSUERS.TRANSLATE_NAMES.ALTA_BANKA';


      case 'Витаминка АД Прилеп':
        return 'ISSUERS.TRANSLATE_NAMES.VITAMINKA';

      case 'Фершпед АД Скопје':
        return 'ISSUERS.TRANSLATE_NAMES.FERSHPED';

      default:
        return value ?? '';
    }
  }
}
