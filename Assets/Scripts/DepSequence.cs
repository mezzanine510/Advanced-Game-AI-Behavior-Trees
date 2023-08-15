using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node
{
    BehaviorTree _dependencyTree;
    NavMeshAgent _agent;

    public DepSequence(string name, BehaviorTree dependencyTree, NavMeshAgent agent)
    {
        _name = name;
        _dependencyTree = dependencyTree;
        _agent = agent;
    }

    public override Status Process()
    {
        // Status dependencyTreeStatus = _dependencyTree.Process();

        // if (dependencyTreeStatus == Status.FAILURE)
        if (_dependencyTree.Process() == Status.FAILURE)
        {
            _agent.ResetPath();

            foreach (Node node in children)
            {
                node.Reset();
            }

            return Status.FAILURE;
        }

        if (_dependencyTree.Process() == Status.RUNNING)
        {
            return Status.RUNNING;
        }

        Status childStatus = children[currentChild].Process();

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.FAILURE) return childStatus; // why return childStatus here, not Status.FAILURE?

        currentChild++;

        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }
}
