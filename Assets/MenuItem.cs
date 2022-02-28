using System;

[Serializable]
public struct MenuItem
{
    public MenuItem(string _name, Action _action)
    {
        name = _name;
        action = _action;
    }
    public string name;
    public Action action;
}