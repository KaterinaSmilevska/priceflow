export interface SecurityPriceTrendReport
{
  securityCode: string;
  period: string;
  startDate: string;
  endDate: string;
  numberOfMeasurements: number;
  startPrice: number;
  endPrice: number;
  lowestPrice: number;
  highestPrice: number;
  averagePrice: number;
  priceChange: number;
  priceChangePercent: number;
  trend: string;
}
