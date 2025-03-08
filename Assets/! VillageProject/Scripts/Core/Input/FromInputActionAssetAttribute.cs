using System;

public class FromInputActionAssetAttribute : Attribute
{
    public string ActionName { get; private set; }

    public FromInputActionAssetAttribute(string actionName)
    {
        ActionName = actionName;
    }
}