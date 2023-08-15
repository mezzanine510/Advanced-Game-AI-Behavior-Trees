using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node
{
    public Inverter(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        Status childStatus = children[0].Process();

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.FAILURE) return Status.SUCCESS;
        
        return Status.FAILURE;
    }
}