using WebApplication1.Models;

namespace WebApplication1.services
{
    public static class BalanceCounter
    {
        public static decimal ClacBalance(int Id, ApplicationDbContext db)
        { 
            decimal Balance = 0;    

            List<Transaction> AllTransactions = db.Transactions
                    .Where(
                        transaction => (
                            transaction.AccountFromId == Id ||
                            transaction.AccountToId == Id))
                    .OrderByDescending(t => t.Date)
                    .ToList();

            foreach (var transaction in AllTransactions)
            {
                if (transaction.AccountToId == Id)
                    Balance += transaction.Sum;
                
                else if (transaction.AccountFromId == Id)
                    Balance -= transaction.Sum;
            }

            return Balance;
        }
    }
}
