using FUNewsManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DAL.Repositories
{
    public class AuthServiceRepository
    {
        private readonly FunewsManagementContext _context;

        public AuthServiceRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<SystemAccount> GetAccountByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        }
    }
}
