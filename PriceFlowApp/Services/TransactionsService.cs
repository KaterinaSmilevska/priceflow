using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;

namespace PriceFlowApp.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;
        private readonly IPortfolioReturnsService _portfolioReturnsService;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;

        public TransactionsService(ITransactionsRepository transactionsRepository, ISecuritiesRepository securitiesRepository,
            IPortfolioReturnsService portfolioReturnsService, IDailyTurnoverRepository dailyTurnoverRepository)
        {
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
            _portfolioReturnsService = portfolioReturnsService;
            _dailyTurnoverRepository = dailyTurnoverRepository;
        }

        public IEnumerable<Transaction> FindByPortfolioId(int portfolioid)
        {
            IEnumerable<Transakcii> transactions = _transactionsRepository.GetByPortfolioId(portfolioid);

            return transactions
                .Select(MapToTransaction)
                .ToList();
        }

        public int FindOwnedShares(int portfolioId, string securityCode, bool isReal)
        {
            HartiiOdVrednost security = GetSecurityByCode(securityCode);

            return _transactionsRepository
                .GetOwnedShares(portfolioId, security.Id, isReal);
        }

        public int FindOwnedSharesAtDate(int portfolioId, string securityCode, bool isReal, DateOnly date, int? transactionIdToExclude)
        {
            HartiiOdVrednost security = GetSecurityByCode(securityCode);

            return _transactionsRepository
                .GetOwnedSharesAtDate(portfolioId, security.Id, isReal, date, transactionIdToExclude);
        }

        public Transaction Add(int portfolioId, Transaction transaction)
        {
            HartiiOdVrednost security = GetSecurityByCode(transaction.HVCode);

            ValidationHelper.ValidateRequiredField(transaction.TypeTransaction, "Type", "TYPE_VALIDATION_REQUIRED");

            ValidateShares(portfolioId, security, transaction);

            var entity = new Transakcii
            {
                PortfolioId = portfolioId,
                Hvid = security.Id,
                TipTransakcija = transaction.TypeTransaction,
                KolicinaAkcii = transaction.SharesQuantity,
                Iznos = CalculateTransactionAmount(transaction),
                EdinecnaCenaAkcija = transaction.SharesUnitPrice,
                BerzanskaProvizija = transaction.StockExchangeCommission,
                BrokerskaProvizija = transaction.BrokerageCommission,
                Cdhvprovizija = transaction.CDHVCommission,
                Realna = transaction.IsReal,
                Datum = transaction.Date
            };

            Transakcii addedTransaction = _transactionsRepository.Add(entity);

            return MapToTransaction(addedTransaction);
        }

        public Transaction Update(int portfolioId, int id, Transaction transaction)
        {
            ValidationHelper.ValidateRequiredField(transaction.TypeTransaction, "Type", "TYPE_VALIDATION_REQUIRED");

            Transakcii existingTransaction = GetTransactionById(id);

            if (existingTransaction.PortfolioId != portfolioId)
                throw new NotFoundException("TRANSACTION_NOT_FOUND", "Transaction not found.");

            HartiiOdVrednost security = GetSecurityByCode(transaction.HVCode);
            ValidateShares(portfolioId, security, transaction, id);

            existingTransaction.Hvid = security.Id;
            existingTransaction.KolicinaAkcii = transaction.SharesQuantity;
            existingTransaction.EdinecnaCenaAkcija = transaction.SharesUnitPrice;
            existingTransaction.TipTransakcija = transaction.TypeTransaction;
            existingTransaction.Realna = transaction.IsReal;
            existingTransaction.BerzanskaProvizija = transaction.StockExchangeCommission;
            existingTransaction.BrokerskaProvizija = transaction.BrokerageCommission;
            existingTransaction.Cdhvprovizija = transaction.CDHVCommission;
            existingTransaction.Datum = transaction.Date;
            existingTransaction.Iznos = CalculateTransactionAmount(transaction);

            Transakcii updatedTransaction = _transactionsRepository.Update(existingTransaction);

            return MapToTransaction(updatedTransaction);
        }

        public Transaction Delete(int id)
        {
            Transakcii existingTransaction = GetTransactionById(id);

            Transakcii deletedTransaction = _transactionsRepository.Delete(existingTransaction);

            return MapToTransaction(deletedTransaction);
        }

        public PortfolioAnalytics GetAnalytics(int portfolioId, bool isReal)
        {
            List<Transakcii> transactions = _transactionsRepository.GetByPortfolioId(portfolioId).ToList();

            transactions = transactions
                .Where(t => t.Realna == isReal)
                .ToList();

            IEnumerable<Transakcii?> sellTransactions = transactions.FindAll(t => t.TipTransakcija == "Продавање");

            IEnumerable<Transakcii?> buyTransactions = transactions.FindAll(t => t.TipTransakcija == "Купување");

            PortfolioReturnsSummary summary = _portfolioReturnsService.CalculateSummary(portfolioId);

            decimal totalRevenue = sellTransactions.Sum(t => t.Iznos);
            decimal totalExpenses = buyTransactions.Sum(t => t.Iznos + CalculateCommission(t)) + 
                sellTransactions.Sum(t => CalculateCommission(t));

            decimal taxes = 0;
            
            if(isReal)
            {
                totalRevenue += summary.TotalDividends;
                taxes = summary.TotalTaxes;
            }

            return new PortfolioAnalytics
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                Balance = totalRevenue - totalExpenses,
                Taxes = taxes,
                IsReal = isReal
            };
        }

        public IEnumerable<OwnedSecuritiesPriceTrend> FindPriceTrend(int userId, PriceTrendPeriod? period, PriceTrendResolution? resolution)
        {
            List<int> ownedSecuritiesIds = _transactionsRepository.GetOwnedSecuritiesIds(userId);

            period ??= PriceTrendPeriod.Monthly;
            resolution ??= DetermineResolution(ownedSecuritiesIds.Count, period.Value);

            IEnumerable<DnevenPromet?> dailyPrices = _dailyTurnoverRepository
                .GetBySecuritiesIds(ownedSecuritiesIds, period, resolution);

            return dailyPrices.Select(dp => new OwnedSecuritiesPriceTrend
            {
                Date = dp.Datum,
                SecurityId = dp.Hvid,
                SecurityCode = dp.Hv.Kod,
                Price = dp.CenaPoslednaTransakcija!.Value
            }).ToList();
        }

        public IEnumerable<SecurityPriceTrendReport> GetSecuritiesPriceTrendReport(int userId, PriceTrendPeriod? period, PriceTrendResolution? resolution, string? securityCode)
        {
            List<int> ownedSecuritiesIds = _transactionsRepository.GetOwnedSecuritiesIds(userId);

            int displayedSecurities = string.IsNullOrWhiteSpace(securityCode)
                ? ownedSecuritiesIds.Count : 1;

            period ??= PriceTrendPeriod.Monthly;
            resolution ??= DetermineResolution(displayedSecurities, period.Value);

            int periodsBack = period == PriceTrendPeriod.Monthly ? 1 : 12;

            IEnumerable<DnevenPromet?> dailyPrices = _dailyTurnoverRepository.GetBySecuritiesIds(ownedSecuritiesIds, period, resolution);

            if (!string.IsNullOrWhiteSpace(securityCode))
            {
                dailyPrices = dailyPrices.Where(x => x.Hv.Kod == securityCode);
            }

            var reports = dailyPrices
                .GroupBy(x => new
                {
                    x.Hvid,
                    x.Hv.Kod
                })
                .Select(g =>
                {
                    var ordered = g.Where(x => x.CenaPoslednaTransakcija.HasValue)
                    .OrderBy(x => x.Datum)
                    .ToList();

                    if (!ordered.Any())
                        return null;

                    decimal startPrice = ordered.First().CenaPoslednaTransakcija!.Value;
                    decimal endPrice = ordered.Last().CenaPoslednaTransakcija!.Value;

                    decimal lowestPrice = ordered.Min(x => x.CenaPoslednaTransakcija!.Value);
                    decimal highestPrice = ordered.Max(x => x.CenaPoslednaTransakcija!.Value);
                    
                    decimal averagePrice = ordered.Average(x => x.CenaPoslednaTransakcija!.Value);

                    decimal change = endPrice - startPrice;
                    decimal changePercent = startPrice == 0 ? 0 : Math.Round(change / startPrice * 100, 2);

                    string trend =
                    change > 0 ? "Increasing" :
                    change < 0 ? "Decreasing" :
                    "Stable";

                    return new SecurityPriceTrendReport
                    {
                        SecurityCode = g.Key.Kod,
                        Period = period.ToString(),
                        StartDate = ordered.First().Datum,
                        EndDate = ordered.Last().Datum,
                        NumberOfMeasurements = ordered.Count(),
                        StartPrice = Math.Round(startPrice, 2),
                        EndPrice = Math.Round(endPrice, 2),
                        LowestPrice = Math.Round(lowestPrice, 2),
                        HighestPrice = Math.Round(highestPrice, 2),
                        AveragePrice = Math.Round(averagePrice, 2),
                        PriceChange = change,
                        PriceChangePercent = changePercent,
                        Trend = trend
                    };
                })
                .Where(r => r != null)
                .Select(r => r!)
                .ToList();

            return reports;
        }

        private decimal CalculateTransactionAmount(Transaction transaction)
        {
            return transaction.SharesQuantity * transaction.SharesUnitPrice;
        }

        private decimal CalculateCommission(Transakcii transaction)
        {
            decimal amount = transaction.KolicinaAkcii * transaction.EdinecnaCenaAkcija;

            decimal feesPercent = transaction.BrokerskaProvizija
                + transaction.BerzanskaProvizija
                + transaction.Cdhvprovizija;
            decimal fees = amount * feesPercent / 100;

            return Math.Round(fees, 2);
        }

        private void ValidateShares(int portfolioId, HartiiOdVrednost security,
           Transaction transaction, int? transactionIdToExclude = null)
        {
            if (transaction.SharesQuantity <= 0)
                throw new InvalidOperationException("Transaction quantity must be greater than 0.");

            List<Transakcii> allTransactions = _transactionsRepository.GetByPortfolioId(portfolioId).ToList();

            allTransactions = allTransactions
                .Where(t => t.Hvid == security.Id && t.Realna == transaction.IsReal)
                .ToList();

            if (transactionIdToExclude.HasValue)
            {
                allTransactions = allTransactions
                    .Where(t => t.Id != transactionIdToExclude.Value)
                    .ToList();
            }

            allTransactions.Add(new Transakcii
            {
                Hvid = security.Id,
                TipTransakcija = transaction.TypeTransaction,
                KolicinaAkcii = transaction.SharesQuantity,
                Datum = transaction.Date,
                Realna = transaction.IsReal
            });

            var ordered = allTransactions
                .OrderBy(t => t.Datum)
                .ThenBy(t => t.TipTransakcija == "Купување" ? 0 : 1)
                .ToList();

            if (transaction.TypeTransaction == "Продавање")
            {
                int ownedAtDate = _transactionsRepository.GetOwnedSharesAtDate(portfolioId, security.Id,
                    transaction.IsReal, transaction.Date, transactionIdToExclude);

                if (transaction.SharesQuantity > ownedAtDate)
                {
                    throw new BusinessRuleException("SELL_MORE_THAN_OWNED",
                        $"On {transaction.Date.ToString("yyyy-MM-dd")} you own only {ownedAtDate} shares of '{security.Kod}'");
                }
            }

            int totalBought = ordered
                .Where(t => t.TipTransakcija == "Купување")
                .Sum(t => t.KolicinaAkcii);

            int totalSold = ordered
                .Where(t => t.TipTransakcija == "Продавање")
                .Sum(t => t.KolicinaAkcii);

            int owned = totalBought - totalSold;
        }

        private PriceTrendResolution DetermineResolution(int numberOfSecurities, PriceTrendPeriod period)
        {
            if(period == PriceTrendPeriod.Monthly)
            {
                if (numberOfSecurities <= 3)
                {
                    return PriceTrendResolution.Day;
                }
                else
                {
                    return PriceTrendResolution.Week;
                }
            }
            else
            {
                if (numberOfSecurities <= 5)
                {
                    return PriceTrendResolution.Month;
                }
                else
                {
                    return PriceTrendResolution.Quarter;
                }
            }
        }

        private HartiiOdVrednost GetSecurityByCode(string securityCode)
        {
            var security = _securitiesRepository.GetByCode(securityCode);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private Transakcii GetTransactionById(int transactionId)
        {
            var transaction = _transactionsRepository.GetById(transactionId);
            if (transaction == null)
                throw new NotFoundException("TRANSACTION_NOT_FOUND", "Transaction not found.");

            return transaction;
        }

        private Transaction MapToTransaction(Transakcii transaction)
        {
            return new Transaction
            {
                Id = transaction.Id,
                HVId = transaction.Hvid,
                HVCode = transaction.Hv.Kod,
                SharesQuantity = transaction.KolicinaAkcii,
                SharesUnitPrice = transaction.EdinecnaCenaAkcija,
                Amount = transaction.Iznos,
                TypeTransaction = transaction.TipTransakcija,
                IsReal = transaction.Realna,
                StockExchangeCommission = transaction.BerzanskaProvizija,
                BrokerageCommission = transaction.BrokerskaProvizija,
                CDHVCommission = transaction.Cdhvprovizija,
                Date = transaction.Datum
            };
        }
    }
}
