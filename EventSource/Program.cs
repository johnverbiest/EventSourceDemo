using System;
using System.Globalization;
using System.Linq;
using Autofac;
using Eventsource.BusinessLogic.Commands.CreateAccount;
using Eventsource.BusinessLogic.Commands.DepositFunds;
using Eventsource.BusinessLogic.Commands.TransferFunds;
using Eventsource.BusinessLogic.Commands.WithdrawFunds;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer;
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
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        } while (true);
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