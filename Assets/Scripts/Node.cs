using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status { SUCCESS, RUNNING, FAILURE }
    public Status status;
    public List<Node> children = new List<Node>();
    public int currentChild = 0;
    public string _name;
    public int _sortOrder;

    public Node() { }

    public Node(string n, int order)
    {
        _name = n;
        _sortOrder = order;
    }

    public void Reset()
    {
        foreach (Node node in children)
        {
            node.Reset();
        }

        currentChild = 0;
    }

    public virtual Status Process()
    {
        return children[currentChild].Process();
    }

    public void AddChild(Node node)
    {
        children.Add(node);
    }
}
