using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NewsletterX.Core.Domain.Model;
using NewsletterX.Core.Domain.Service;

namespace NewsletterX.Infrastructure.DataAccess
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly string _connectionString;
        public SubscriptionRepository()
        {
            _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NewsLetter;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        }
        public async Task<bool> Create(Subscription subscription)
        {
            await using var conn = new SqlConnection(_connectionString);
            const string insert =
                "INSERT INTO Subscr (Id, Email, VerificationCode, IsConfirmed) VALUES (@Id, @Email, @ConfCode, 0)";
            var rowsAffected = await conn.ExecuteAsync(insert, new {Id = subscription.Id, Email = subscription.Email, ConfCode = subscription.VerificationCode});
            return rowsAffected == 1;
        }

        public async Task<Subscription> ReadByEmail(string email)
        {
            await using var conn = new SqlConnection(_connectionString);
            const string lookUp =
                "SELECT * FROM Subscr WHERE Email = @Email";
            var query = await conn.QueryAsync<Subscription>(lookUp, new {Email = email});
            return query.FirstOrDefault(x => x.Email == email);
        }

        public async Task<bool> Update(Subscription subscription)
        {
            await using var conn = new SqlConnection(_connectionString);
            const string update = @"UPDATE Subscr 
                                    SET IsConfirmed = 1  
                                    WHERE VerificationCode = @ConfCode";
            var linesAffected = await conn.ExecuteAsync(update, new { ConfCode = subscription.VerificationCode });
            return linesAffected == 1;
        }
    }
}
