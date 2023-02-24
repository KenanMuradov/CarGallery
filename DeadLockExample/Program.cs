namespace DeadLockExample;


// Mənbə : dotnettutorials

// Əsas nümunə kimi Account class ını işlədirik
public class Account
{
    public int ID { get; }
    private double balance { get; set; }
    public Account(int id, double balance)
    {
        ID = id;
        this.balance = balance;
    }

    // balance property sinə bir başa access verməmək üçün bu 2 metod işlənir
    public void WithdrawMoney(double amount)=> balance -= amount;

    public void DepositMoney(double amount)=> balance += amount;
    
}


// AccountManager class 2 account arasında pul transferi eləməl üçün class dı
// FundTransfer methodunu işlədəndə bizim account lar blocklanır və onlara çatmaq olmur
public class AccountManager
{
    private Account FromAccount;
    private Account ToAccount;
    private double TransferAmount;
    public AccountManager(Account AccountFrom, Account AccountTo, double AmountTransfer)
    {
        FromAccount = AccountFrom;
        ToAccount = AccountTo;
        TransferAmount = AmountTransfer;
    }

    public void FundTransfer()
    {
        Console.WriteLine($"{Thread.CurrentThread.Name} trying to acquire lock on {FromAccount.ID}");
        lock (FromAccount)
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} acquired lock on {FromAccount.ID}");
            Console.WriteLine($"{Thread.CurrentThread.Name} Doing Some work");
            Thread.Sleep(1000);
            Console.WriteLine($"{Thread.CurrentThread.Name} trying to acquire lock on {ToAccount.ID}");
            lock (ToAccount)
            {
                FromAccount.WithdrawMoney(TransferAmount);
                ToAccount.DepositMoney(TransferAmount);
            }
        }
    }
}

public class Program
{
    static void Main()
    {
        // Main də 2 account yaradırıq və bunları 2 manager lərə veririk 
        // Sadəcə manager 1 üçün 1001 ci account pul çıxarılan, 1002 ci account isə pul köçürüləndir
        // Manager 2 üçün isə tərsinə pul 1002 ci account dan çıxarılıb 1001 ci account a köçürülüe
        Console.WriteLine("Main Thread Started");
        Account Account1001 = new(1001, 5000);
        Account Account1002 = new(1002, 3000);

        AccountManager accountManager1 = new AccountManager(Account1001, Account1002, 5000);
        Thread thread1 = new(accountManager1.FundTransfer)
        {
            Name = "Thread1"
        };

        AccountManager accountManager2 = new AccountManager(Account1002, Account1001, 6000);
        Thread thread2 = new(accountManager2.FundTransfer)
        {
            Name = "Thread2"
        };


        // Thread ləri eyni vaxtda start elədiyimizə görə account1001 və account1002 təxminən eyni anda blocklanır
        // FundTransfer methodunun işinin tamamlaması üçün manager 1 account1002, manager 2 isə account1001 i locklamaq istəyir 
        // Amma hər 2 account artıq bloklanmış olduğuna görə onlar bir birinin lockdan çıxmağını sonsuza qədər gözləyəcək
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Console.WriteLine("Main Thread Completed");
        Console.ReadKey();
    }
}