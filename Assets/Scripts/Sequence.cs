using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        Status childStatus = children[currentChild].Process();

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.FAILURE)
        {
            currentChild = 0;
            
            foreach (Node node in children)
            {
                node.Reset();
            }

            return Status.FAILURE;
            // return childStatus; // why return childStatus here, not Status.FAILURE?
        }

        currentChild++;

        if (currentChild >= children.Count) // if you're done looping through children return SUCCESS
        {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING; // otherwise return RUNNING because there are children left to process
    }
}
