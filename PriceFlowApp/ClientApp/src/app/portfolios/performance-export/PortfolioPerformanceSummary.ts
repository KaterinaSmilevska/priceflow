export interface PortfolioPerformanceSummary {
  portfolioName: string;
  fromDate: string;
  toDate: string;
  startingValue: number;
  endingValue: number;
  dividends: number;
  commissions: number;
  absoluteReturn: number;
}
