export interface PriceChangeNotification {
  id: number;
  securityId: number;
  changePercent: number;
  tradingDate: string;
  message: string;
  isRead: boolean;
}
