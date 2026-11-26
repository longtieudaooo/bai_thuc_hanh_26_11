namespace BankAtmMdiApp.DTO
{
    public class AtmStation
    {
        public string StationID { get; set; }
        public string Address { get; set; }
        public decimal CashBalance { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
    }
}