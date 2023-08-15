using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick(); // like a frame for the behavior tree
    public Tick ProcessMethod;

    public delegate Status TickMulti(int index); // like a frame for the behavior tree
    public TickMulti ProcessMethodMulti;
    public int index;
    public int sortOrder = 0;

    public Leaf() { }

    public Leaf(string name, Tick processMethod)
    {
        _name = name;
        ProcessMethod = processMethod;
    }

    public Leaf(string name, TickMulti processMethodMulti, int index)
    {
        _name = name;
        ProcessMethodMulti = processMethodMulti;
        this.index = index;
    }

    public Leaf(string name, Tick processMethod, int sortOrder)
    {
        _name = name;
        ProcessMethod = processMethod;
        _sortOrder = sortOrder;
    }

    public override Status Process()
    {
        Node.Status s;

        if (ProcessMethod != null)
            s = ProcessMethod();
        else if (ProcessMethodMulti != null)
            s = ProcessMethodMulti(index);
        else s = Status.FAILURE;

        Debug.Log(_name + " " + s);
        
        return s;
    }
}
