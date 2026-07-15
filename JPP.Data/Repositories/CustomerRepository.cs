using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Data.Entities;
using JPP.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JPP.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext.AppDbContext _context;

        public CustomerRepository(AppDbContext.AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IdentityNoExistsAsync(string identityNo)
        {
            if (string.IsNullOrWhiteSpace(identityNo)) return false;

            return await _context.Customers
                .AnyAsync(c => c.IdentityNo == identityNo.Trim());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return await _context.Customers
                .AnyAsync(c => c.EmailAddress == email.Trim());
        }

        public async Task<int> CreateCustomerAsync(BIZCustomer customer)
        {
            _context.Customers.Add(customer);

            // Perintah ini yang akan menghasilkan eksekusi Insert ke SQL
            await _context.SaveChangesAsync();

            // Entity Framework otomatis mengisi property ID setelah SaveChanges
            return customer.ID;
        }
    }
}
