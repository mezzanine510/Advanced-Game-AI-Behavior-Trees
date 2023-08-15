using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSelector : Node
{
    private bool shuffled = false;

    public RSelector(string n)
    {
        _name = n;
    }

    public override Status Process()
    {
        if (!shuffled)
        {
            children.Shuffle();
            shuffled = true;
            Debug.Log(_name + " has been <color=pink>shuffled</color>");
        }

        Status childStatus = children[currentChild].Process();

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.SUCCESS)
        {
            currentChild = 0;
            shuffled = false;
            return Status.SUCCESS;
        }

        currentChild++;

        if (currentChild >= children.Count)
        {
            currentChild = 0;
            shuffled = false;
            return Status.FAILURE;
        }

        return Status.RUNNING;
    }

}
