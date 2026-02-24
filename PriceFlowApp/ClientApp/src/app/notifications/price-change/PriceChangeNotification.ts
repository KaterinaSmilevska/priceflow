export interface PriceChangeNotification {
  id: number;
  hvId: number;
  changePercent: number;
  tradingDate: string;
  message: string;
  isRead: boolean;
}
