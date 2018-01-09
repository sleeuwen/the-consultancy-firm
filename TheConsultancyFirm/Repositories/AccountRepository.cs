using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(a => a.Email == email);
        }

        public void DeleteDummyUser(string email)
        {
            var user = GetUserByEmail(email);

            if (user == null) return;

            _context.Users.Remove(user);
        }
    }
}
