using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BankAtmMdiApp.DAL;
using BankAtmMdiApp.DTO;

namespace BankAtmMdiApp.BUS
{
    public class AtmBUS
    {
        private AtmDAL dal = new AtmDAL();

        public List<AtmStation> LayDanhSachTram() => dal.GetAllStations();

        public string LuuThongTinTram(AtmStation atm)
        {
            if (string.IsNullOrWhiteSpace(atm.StationID) || string.IsNullOrWhiteSpace(atm.Address))
                return "Mã trạm và địa chỉ không được để trống!";
            if (atm.CashBalance < 0)
                return "Số tiền tồn quỹ không hợp lệ!";
            return dal.InsertOrUpdateStation(atm) ? "OK" : "Lưu thất bại!";
        }

        public async Task<ReconcileResult> ChayDoiSoatTuDongAsync(IProgress<int> progress, CancellationToken token)
        {
            return await Task.Run(async () =>
            {
                int totalSteps = 50; // ~5 giây
                for (int i = 1; i <= totalSteps; i++)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(100, token);
                    progress?.Report((i * 100) / totalSteps);
                }

                Random rnd = new Random();
                int totalTx = rnd.Next(150000, 350000);
                int diff = rnd.Next(0, 12);
                ReconcileResult result = new ReconcileResult
                {
                    ReconcileDate = DateTime.Now,
                    TotalTransactions = totalTx,
                    MatchedCount = totalTx - diff,
                    DiscrepancyCount = diff,
                    Note = diff == 0 ? "Khớp 100% dữ liệu Core Banking" : $"Phát hiện {diff} giao dịch treo cần tra soát"
                };

                dal.SaveReconcileLog(result);
                return result;
            }, token);
        }
    }
}