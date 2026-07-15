import { Injectable, Pipe, PipeTransform } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
@Pipe({
  name: 'sectorsTranslate',
  standalone: true
})
export class SectorsTranslatePipe implements PipeTransform {
  transform(value: string | null | undefined): string {

    switch (value) {
      case 'Банкарство':
        return 'SECTORS.BANKING'

      case 'Услуги':
        return 'SECTORS.SERVICES'

      case 'Трговија':
        return 'SECTORS.TRADING'

      case 'Индустрија':
        return 'SECTORS.INDUSTRY'

      case 'Градежништво':
        return 'SECTORS.CONSTRUCTION'

      case 'Угостителство':
        return 'SECTORS.HOSPITALITY'

      default:
        return value ?? '';
    }
  }
}
