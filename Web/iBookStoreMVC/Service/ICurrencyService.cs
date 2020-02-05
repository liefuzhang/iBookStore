using System.Threading.Tasks;

namespace iBookStoreMVC.Service
{
    public interface ICurrencyService
    {
        Task<decimal?> GetCurrencyRate(string currency);
    }
}