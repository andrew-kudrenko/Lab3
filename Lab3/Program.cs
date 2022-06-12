using System.Collections;

namespace Lab3
{
    using ExchangingTuple = Tuple<string, double>;
    
    class Program
    {
        public static void Main()
        {
            Bank bank = new("Andy's Bank");

            CreateAccountDto accountData = new()
            {
                InterestRate = 0.2,
                Balance = 70,
                Currency = "US",
                Owner = "Andrew Kudrenko"
            };
            Account account = bank.RegisterAccount(accountData);

            account += 400;

            bank.SetAccount(account.Id);

            double asEuro = bank.ExchangeCurrency("EUR");
            double asDollar = bank.ExchangeCurrency("US");
            double asRoubles = bank.ExchangeCurrency("RUB");
            
            Console.WriteLine(account);
            Console.WriteLine($"Exchanged. EUR: {asEuro}, US: {asDollar}, RUB: {asRoubles}");
        }
    }

    class Bank
    {
        public string Name { get; }
        
        private readonly List<Account> _accounts = new();
        private readonly CurrencyExchanger _exchanger = new();
        private Account? _account;

        public Bank(string name)
        {
            this.Name = name;
        }
        
        public double ExchangeCurrency(string to)
        {
            this.CheckForAccount();

            double balance = this._account!.Balance;
            string currency = this._account!.Currency;

            return this._exchanger.Exchange(new ExchangingTuple(currency, balance), to);
        }

        public void SetAccount(string id)
        {
            Account? account = this._accounts.Find(a => a.Id == id);

            if (account == null)
            {
                throw new Exception("Such account is not found");
            }

            this._account = account;
        }

        public Account RegisterAccount(CreateAccountDto? from)
        {
            string id = DateTime.Now.ToString();
            Account account = new Account(id, from);
            
            this._accounts.Add(account);

            return account;
        }

        private void CheckForAccount()
        {
            if (this._account == null)
            {
                throw new Exception("Account hasn't been selected");
            }
        }
    }

    struct Currency
    {
        public readonly string Name;
        public readonly double RateToDollar;

        public Currency(string? name, double? rateToDollar)
        {
            this.Name = name ?? "";
            this.RateToDollar = rateToDollar ?? 1;
        }
    }
    

    class CurrencyExchanger
    {
        private readonly List<Currency> _currencies =
        new List<Currency>{
            new Currency("US", 1),
            new Currency("EUR", 1.15),
            new Currency("RUB", 0.023),
        };

        public double Exchange(ExchangingTuple from, string to)
        {
            double asDollars = this.ConvertCurrency(from);
            return this.ConvertCurrency(new Tuple<string, double>(to, asDollars));
        }

        private double ConvertCurrency(ExchangingTuple from)
        {
            return this.GetRateToDollar(from.Item1) * from.Item2;
        }

        private double GetRateToDollar(string name)
        {
            Currency? found = this._currencies.Find(c => c.Name == name);

            if (found == null)
            {
                throw new Exception($"Currency `{name}` have not be found");
            }

            return found.Value.RateToDollar;
        }
    }

    struct CreateAccountDto
    {
        public string Owner;
        public double InterestRate;
        public double Balance;
        public string Currency;
    }
        
    class Account
    {
        public string Owner { get; private set; }
        public string Id { get; }
        public double InterestRate { get; }
        public double Balance { get; private set; }
        
        public string Currency { get; }

        public Account(string id, CreateAccountDto? from)
        {
            this.Id = id;
            this.InterestRate = from?.InterestRate ?? 0;
            this.Owner = from?.Owner ?? "";
            this.Balance = from?.Balance ?? 0;
            this.Currency = from?.Currency ?? "US";
        }

        public void ChargeInterest()
        {
            this.Balance += this.Balance * this.InterestRate;
        }
        
        public string GetInfo()
        {
            return $"ID: {Id}. Owner is {Owner}. Current balance equals {Balance}{Currency}";
        }

        public void ChangeOwner(string owner)
        {
            if (owner.Length == 0)
            {
                throw new Exception("Incorrect owner name");
            }

            this.Owner = owner;
        }

        public override string ToString() => this.GetInfo();
    }
}