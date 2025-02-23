using FUNewsManagementSystem.DAL.Models;

namespace FUNewsManagementSystem.DAL.Interfaces
{
    public interface ISystemAccountRepository
    {
        Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
        Task<SystemAccount> GetAccountByIdAsync(short id);
        Task<SystemAccount> GetAccountByEmailAsync(string email);
        Task AddAccountAsync(SystemAccount account);
        Task UpdateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(short id);
    }
}
