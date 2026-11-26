using System;

namespace BankAtmMdiApp.DTO
{
    public class ReconcileResult
    {
        public DateTime ReconcileDate { get; set; }
        public int TotalTransactions { get; set; }
        public int MatchedCount { get; set; }
        public int DiscrepancyCount { get; set; }
        public string Note { get; set; }
    }
}