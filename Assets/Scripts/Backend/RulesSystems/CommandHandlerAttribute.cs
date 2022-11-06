using System;

[AttributeUsage(AttributeTargets.Method)] 
public class CommandHandlerAttribute : Attribute {
    public GameCommandType commandType;

    public CommandHandlerAttribute(GameCommandType gct) {
        commandType = gct;
    }
}