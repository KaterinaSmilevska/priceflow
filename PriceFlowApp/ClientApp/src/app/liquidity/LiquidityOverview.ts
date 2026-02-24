import { SecurityLiquidity } from "./SecurityLiquidity";

export interface LiquidityOverview {
  mostByTradedQuantity: SecurityLiquidity[];
  leastByTradedQuantity: SecurityLiquidity[];
  mostByTradingDays: SecurityLiquidity[];
  leastByTradingDays: SecurityLiquidity[];
}
