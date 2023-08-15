using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatronBehavior : BTAgent
{
    public GameObject[] art;
    public GameObject frontDoor;
    public GameObject home;

    [Range(0, 1000)]
    public int boredom = 0;

    public override void Start()
    {
        base.Start();
        RSelector selectObject = new RSelector("Select Art To View");

        for (int i = 0; i < art.Length; i++)
        {
            String n = art[i].name;
            Leaf goToArt = new Leaf("Go To: " + n, GoToArt, i);
            selectObject.AddChild(goToArt);
        }

        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToHome = new Leaf("Go Home", GoToHome);
        Leaf isBored = new Leaf("Is Bored", IsBored);

        Sequence viewArt = new Sequence("View Art");
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);
        viewArt.AddChild(selectObject);
        viewArt.AddChild(goToHome);

        Selector bePatron = new Selector("Be An Art Patron");
        bePatron.AddChild(viewArt);

        tree.AddChild(bePatron);

        StartCoroutine(IncreaseBoredom());
    }

    private IEnumerator IncreaseBoredom()
    {
        while (true)
        {
            boredom = Mathf.Clamp(boredom + 20, 0, 1000);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 5));
        }
    }

    private Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(frontDoor);
        return status;
    }

    private Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf) return Node.Status.FAILURE;

        Node.Status status = GoToLocation(art[i].transform.position);

        if (status == Node.Status.SUCCESS)
        {
            boredom = Mathf.Clamp(boredom - 500, 0, 1000);
        }

        return status;
    }

    private Node.Status GoToHome()
    {
        Node.Status v = GoToLocation(home.transform.position);

        return v;
    }

    private Node.Status IsBored()
    {
        if (boredom < 100)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }
}
