# Events

Events are things that have been done (state changes). Events can be triggered by:

- A Command handler
- Some kind of timer
- An external trigger (api / service bus)

An event contains:
- All relevant data to the state change (for example, a NameChangedEvent must contain for example the id of the person and the new name)
- All relevant metadata, for example: ip, username that triggered the action, time, location, wind direction, ... 

## This example

In this example all events are of the type  `IBusinessLogicEvent` and therefor contains a minimum of 3 datapoints:

```c#
    public interface IBusinessLogicEvent: IEvent
    {
        Guid EventId { get;  }
        DateTime EventRaised { get; }
        int AccountNumber { get; }
    }
```

Because of this we can create a backend that knows about the AccountNumber and we can use this to query the relevant events quickly.
