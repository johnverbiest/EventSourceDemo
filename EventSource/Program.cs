using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Azure.Data.Tables;
using Eventsource.BusinessLogic.Commands.CreateAccount;
using Eventsource.BusinessLogic.Commands.DepositFunds;
using Eventsource.BusinessLogic.Commands.TransferFunds;
using Eventsource.BusinessLogic.Commands.WithdrawFunds;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer;
using Eventsource.Datalayer.ReadOnlyDb;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Queries;


namespace EventSource;

internal class Program
{
    private static ICommandQueue _commandQueue;
    private static IQueryHandler<AllActiveAccountsQuery, AllActiveAccountsQuery.Result> _accountsQueryHandler;
    private static IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result> _accountBalanceQueryHandler;
    private static IQueryHandler<AccountHistoryQuery, AccountHistoryQuery.Result> _accountHistoryQueryHandler;

    private static void Main(string[] args)
    {
        // Setup DI
        var builder = new ContainerBuilder();
        builder.RegisterAssemblyTypes(typeof(CreateAccountCommand).Assembly).AsImplementedInterfaces().SingleInstance();
        builder.RegisterAssemblyTypes(typeof(CommandQueue).Assembly).AsImplementedInterfaces().SingleInstance();
        builder.RegisterAssemblyTypes(typeof(EventStore).Assembly).AsImplementedInterfaces().SingleInstance();
        builder.RegisterAssemblyTypes(typeof(Program).Assembly).AsImplementedInterfaces().SingleInstance();

        var container = builder.Build();
        _commandQueue = container.Resolve<ICommandQueue>();
        _accountsQueryHandler =
            container.Resolve<IQueryHandler<AllActiveAccountsQuery, AllActiveAccountsQuery.Result>>();
        _accountBalanceQueryHandler =
            container.Resolve<IQueryHandler<AccountBalanceQuery, AccountBalanceQuery.Result>>();
        _accountHistoryQueryHandler =
            container.Resolve<IQueryHandler<AccountHistoryQuery, AccountHistoryQuery.Result>>();

        // This is the event store
        do
        {
            Console.WriteLine("########################################################");
            Console.WriteLine("## Awesome Bank! Welcome                              ##");
            Console.WriteLine("########################################################");
            Console.WriteLine("");
            Console.WriteLine("What do you want to do?");
            Console.WriteLine(" 1) Login as Random Dude");
            Console.WriteLine(" 2) Login as Bank Meeple");
            Console.WriteLine(" 0) Exit program");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" LoadEm) Put a lot of data in the store");
            Console.WriteLine(" Rebuild) Rebuild the read-only db");
            Console.Write("Please enter your command:");
            var command = Console.ReadLine();
            switch (command)
            {
                case "0":
                    return;
                case "1":
                    RunAsRandomDude();
                    break;
                case "2":
                    RunAsBankMeeple();
                    break;
                case "LoadEm":
                    LoadEm();
                    break;
                case "Rebuild":
                    Rebuild(container.Resolve<IReadOnlyModelHandler[]>());
                    break;
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        } while (true);
    }

    private static void Rebuild(IReadOnlyModelHandler[] rebuilders)
    {
        foreach (var readOnlyModelHandler in rebuilders)
        {
            readOnlyModelHandler.Rebuild().Wait();
        }
    }

    private static void LoadEm()
    {
        var accounts = 10;
        var transactions = 100;
        var random = new Random(DateTime.UtcNow.Millisecond);

        for (var i = 1; i <= accounts + 1; i++) 
        {
            var name = $"LoadEm Account {i}";
            _commandQueue.QueueForExecution(new CreateAccountCommand() { Name = name }).Wait();
            var allaccounts = _accountsQueryHandler.Handle(new AllActiveAccountsQuery()).Result.Accounts;
            var account = allaccounts.OrderByDescending(x => x.AccountNumber).First(x => x.Name == name);
        }

        var theFinalAccounts = _accountsQueryHandler.Handle(new AllActiveAccountsQuery()).Result.Accounts;
        foreach (var account in theFinalAccounts)
        {
            for (int j = 1; j <= transactions; j++)
            {
                var transactionType = random.Next(1000) % 3;
                switch (transactionType)
                {
                    case 0:
                        Console.Write($"Account {account.AccountNumber}: Deposit");
                        var depositWatch = Stopwatch.StartNew();
                        _commandQueue.QueueForExecution(new DepositFundsCommand()
                        {
                            AccountNumber = account.AccountNumber,
                            Amount = random.Next(0, 1000)
                        }).Wait();
                        Console.WriteLine($" done in {depositWatch.ElapsedMilliseconds}ms");
                        break;
                    case 1:
                        Console.Write($"Account {account.AccountNumber}: Withdraw");
                        var withdrawWatch = Stopwatch.StartNew();
                        _commandQueue.QueueForExecution(new WithdrawFundsCommand()
                        {
                            AccountNumber = account.AccountNumber,
                            Amount = random.Next(0, 1000)
                        });
                        Console.WriteLine($" done in {withdrawWatch.ElapsedMilliseconds}ms");
                        break;
                    case 2:
                        Console.Write($"Account {account.AccountNumber}: Transfer");
                        var transferWatch = Stopwatch.StartNew();
                        _commandQueue.QueueForExecution(new TransferFundsCommand()
                        {
                            AccountNumber = account.AccountNumber,
                            DestinationAccountNumber = theFinalAccounts[random.Next(0, theFinalAccounts.Length)].AccountNumber,
                            Amount = random.Next(0, 1000)
                        });
                        Console.WriteLine($" done in {transferWatch.ElapsedMilliseconds}ms");
                        break;
                }
            }
        }
    }


    private static void RunAsRandomDude()
    {
        do
        {
            Console.WriteLine("Welcome Random Dude! \n What do you want to do today?");
            Console.WriteLine(" 1) Create new account");
            Console.WriteLine(" 0) Logout");
            var command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    return;
                case "1":
                    Console.Write("Please enter your name: ");
                    var name = Console.ReadLine();
                    _commandQueue.QueueForExecution(new CreateAccountCommand() { Name = name }).Wait();
                    break;
            }
        } while (true);
    }


    private static void RunAsBankMeeple()
    {
        do
        {
            Console.WriteLine("Welcome Bank Meeple! \n What client do you want to do stuff for?");

            Console.WriteLine(" 0) Logout");
            Console.WriteLine("------------------");
            var accounts = _accountsQueryHandler.Handle(new AllActiveAccountsQuery()).Result.Accounts;
            foreach (var acc in accounts)
            {
                Console.WriteLine($" {acc.AccountNumber}) {acc.Name}");
            }
            Console.WriteLine("------------------");
            Console.Write("Open account number: ");
            var command = Console.ReadLine();
            if (command == "0") return;

            var account = accounts.Single(x => x.AccountNumber.ToString().Equals(command));
            do
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Please say {account.Name} hello! You are managing account number {account.AccountNumber}");
                Console.WriteLine("What do you want to do?");
                Console.WriteLine(" 0) Select other account");
                Console.WriteLine(" 1) Deposit funds");
                Console.WriteLine(" 2) Show History & Balance");
                Console.WriteLine(" 3) Withdraw funds");
                Console.WriteLine(" 4) Transfer funds");


                command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        account = null;
                        break;
                    case "1":
                        Console.WriteLine("How much has to be deposited?");
                        var depositAmount = decimal.Parse(Console.ReadLine());
                        _commandQueue.QueueForExecution(new DepositFundsCommand()
                        {
                            AccountNumber = account.AccountNumber,
                            Amount = depositAmount
                        }).Wait();
                        break;

                    case "2":
                        var history = _accountHistoryQueryHandler.Handle(new AccountHistoryQuery()
                            { AccountNumber = account.AccountNumber }).Result.Events;
                        foreach (var historyObject in history)
                        {
                            Console.WriteLine($"{historyObject.date.ToLocalTime():G}\t || {historyObject.Description} \t\t|| Balance: {historyObject.Balance}");
                        }


                        // Show current balance
                        var themBalance = _accountBalanceQueryHandler.Handle(new AccountBalanceQuery()
                            { AccountNumber = account.AccountNumber }).Result.Balance;
                        Console.WriteLine($"The account currently holds a balance of {themBalance}");
                        break;

                    case "3":
                        // Show current balance
                        var balance = _accountBalanceQueryHandler.Handle(new AccountBalanceQuery()
                        { AccountNumber = account.AccountNumber }).Result.Balance;
                        Console.WriteLine($"The account currently holds a balance of {balance}");

                        // Request and process withdrawal
                        Console.WriteLine($"How much does {account.Name} wants to withdraw?");
                        var withdrawAmount = decimal.Parse(Console.ReadLine());
                        _commandQueue.QueueForExecution(new WithdrawFundsCommand()
                        {
                            AccountNumber = account.AccountNumber,
                            Amount = withdrawAmount
                        });

                        // Show new balance
                        balance = _accountBalanceQueryHandler.Handle(new AccountBalanceQuery()
                            { AccountNumber = account.AccountNumber }).Result.Balance;
                        Console.WriteLine($"The new balance is {balance}");
                        break;



                    case "4":
                        // Show current balance
                        var curBalance = _accountBalanceQueryHandler.Handle(new AccountBalanceQuery()
                            { AccountNumber = account.AccountNumber }).Result.Balance;
                        Console.WriteLine($"The account currently holds a balance of {curBalance}");

                        // Request amount
                        Console.WriteLine($"How much does {account.Name} wants to tranfer?");
                        var transferAmount = decimal.Parse(Console.ReadLine());

                        // Request transfer target
                        Console.WriteLine("To what account?");
                        foreach (var acc in accounts)
                        {
                            Console.WriteLine($" {acc.AccountNumber}) {acc.Name}");
                        }

                        var input = Console.ReadLine();
                        var transferTarget =
                            accounts.Single(x => x.AccountNumber.ToString().Equals(input));

                        Console.WriteLine($"Tranfering {transferAmount} from {account.Name} to {transferTarget.Name} (#{transferTarget.AccountNumber})");
                        // Do the transfer (or try)
                        _commandQueue.QueueForExecution(new TransferFundsCommand()
                        {
                            AccountNumber = account.AccountNumber,
                            DestinationAccountNumber = transferTarget.AccountNumber,
                            Amount = transferAmount
                        });

                        // Show new balance
                        curBalance = _accountBalanceQueryHandler.Handle(new AccountBalanceQuery()
                            { AccountNumber = account.AccountNumber }).Result.Balance;
                        Console.WriteLine($"The new balance is {curBalance}");
                        break;
                }
            } while (account != null);
        } while (true);
    }
}