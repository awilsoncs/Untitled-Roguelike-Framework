using System;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : Attribute {
    public string guid {get;}
    public ComponentAttribute(string guid) {
        this.guid = guid;
    }
}