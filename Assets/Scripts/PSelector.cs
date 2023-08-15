using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSelector : Node
{
    Node[] nodeArray;
    private bool sorted = false;

    public PSelector(string name)
    {
        _name = name;
    }

    public override Status Process()
    {
        if (!sorted)
        {
            SortNodes();
            sorted = true;
            Debug.Log(_name + " has been <color=white>resorted</color>");
        }

        Status childStatus = children[currentChild].Process();

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.SUCCESS)
        {
            // children[currentChild].sortOrder = 1;

            currentChild = 0;
            sorted = false;
            return Status.SUCCESS;
        }
        // else
        // {
        //     // children[currentChild].sortOrder = 10;
        // }

        currentChild++;

        if (currentChild >= children.Count)
        {
            currentChild = 0;
            sorted = false;
            return Status.FAILURE;
        }

        return Status.RUNNING;
    }

    void SortNodes()
    {
        nodeArray = children.ToArray();
        Sort(nodeArray, 0, children.Count - 1);
        children = new List<Node>(nodeArray);
    }

    int Partition(Node[] array, int low, int high)
    {
        Node pivot = array[high];

        int lowIndex = (low - 1);

        //2. Reorder the collection.
        for (int j = low; j < high; j++)
        {
            if (array[j]._sortOrder <= pivot._sortOrder)
            {
                lowIndex++;

                Node temp = array[lowIndex];
                array[lowIndex] = array[j];
                array[j] = temp;
            }
        }

        Node temp1 = array[lowIndex + 1];
        array[lowIndex + 1] = array[high];
        array[high] = temp1;

        return lowIndex + 1;
    }

    void Sort(Node[] array, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(array, low, high);
            Sort(array, low, partitionIndex - 1);
            Sort(array, partitionIndex + 1, high);
        }
    }
}
