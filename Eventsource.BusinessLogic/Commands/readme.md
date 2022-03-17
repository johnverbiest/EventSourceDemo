# Commands

Commands are actions to be done. An action is triggered by either a user, or a Policy triggered by an Event.

At the end of a command, events are fired. These signify the state changes to the system because of the execution of the command.

In short, a commandhandler is responsible for:

- (Re)validating the input
- Doing the work (for example sending an email, printing a file, calculate state change, ...)
- Sending events that signifies the state change that occured