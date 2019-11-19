using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BankOfDotNet.API.Models
{
    public class BankContext : DbContext
    {
        public BankContext([NotNull] DbContextOptions<BankContext> options) 
            : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }

    }
}
