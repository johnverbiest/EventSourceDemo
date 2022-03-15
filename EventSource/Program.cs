using System;
using Autofac;
using Eventsource.BusinessLogic.Commands.CreateAccount;
using JohnVerbiest.CQRS.Commands;


namespace EventSource;

internal class Program
{
    private static ICommandQueue _commandQueue;

    private static void Main(string[] args)
    {
        // Setup DI
        var builder = new ContainerBuilder();
        builder.RegisterAssemblyTypes(typeof(CreateAccountCommand).Assembly).AsImplementedInterfaces().SingleInstance();
        builder.RegisterAssemblyTypes(typeof(CommandQueue).Assembly).AsImplementedInterfaces().SingleInstance();
        builder.RegisterAssemblyTypes(typeof(Program).Assembly).AsImplementedInterfaces().SingleInstance();

        var container = builder.Build();
        _commandQueue = container.Resolve<ICommandQueue>();

        // This is the event store
        do
        {
            Console.Clear();
            Console.WriteLine("########################################################");
            Console.WriteLine("## Awesome Bank! Welcome                              ##");
            Console.WriteLine("########################################################");
            Console.WriteLine("");
            Console.WriteLine("What do you want to do?");
            Console.WriteLine(" 1) Login as Random Dude");
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
            }
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
}