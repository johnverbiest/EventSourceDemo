using System;
using System.Linq;
using Autofac;
using Eventsource.BusinessLogic.Commands.CreateAccount;
using Eventsource.BusinessLogic.Queries.AllActiveAccountsQuery;
using Eventsource.Datalayer;
using JohnVerbiest.CQRS.Commands;
using JohnVerbiest.CQRS.Queries;


namespace EventSource;

internal class Program
{
    private static ICommandQueue _commandQueue;
    private static IQueryHandler<AllActiveAccountsQuery, AllActiveAccountsQuery.Result> _accountsQueryHandler;

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
                    _commandQueue.QueueForExecution(new CreateAccountCommand(){Name = name}).Wait();
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
                

                command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        account = null;
                        break;
                    case "1":

                        break;
                }
            } while (account != null);
        } while (true);
    }
}