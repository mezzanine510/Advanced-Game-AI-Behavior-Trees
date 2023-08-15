using System;
using System.Collections;
using UnityEngine;

public class RobberBehavior : BTAgent
{
    // public enum ActionState { IDLE, WORKING }
    [Range(0, 1000)] public int money = 400;

    [SerializeField] GameObject diamond;
    [SerializeField] GameObject painting;
    [SerializeField] GameObject van;
    [SerializeField] GameObject backDoor;
    [SerializeField] GameObject frontDoor;
    [SerializeField] GameObject[] art;

    public GameObject cop;
    public GameObject stolenObject;
    Leaf goToBackDoor;
    Leaf goToFrontDoor;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        PSelector openDoor = new PSelector("Open Door");
        RSelector selectObject = new RSelector("Select Object to Steal");

        for (int i = 0; i < art.Length; i++)
        {
            String name = art[i].name;
            Leaf goToArt = new Leaf("Go To: " + name, GoToArt, i);
            selectObject.AddChild(goToArt);
        }

        goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor, 2);
        goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor, 1);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 2);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Leaf hasMoney = new Leaf("Has Money", HasMoney);

        Sequence runAway = new Sequence("Check Can See Cop");
        Leaf canSeeCop = new Leaf("Can See Cop", CanSeeCop);
        Leaf fleeFromCop = new Leaf("Flee From Cop?", FleeFromCop);

        Inverter invertMoney = new Inverter("Invert Money");
        Inverter invertCanSeeCop = new Inverter("Can NOT see cop");

        invertMoney.AddChild(hasMoney);
        invertCanSeeCop.AddChild(canSeeCop);

        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);

        // Sequence s1 = new Sequence("s1");
        // s1.AddChild(invertMoney);

        // Sequence s2 = new Sequence("s2");
        // s2.AddChild(cannotSeeCop);
        // s2.AddChild(openDoor);

        // Sequence s3 = new Sequence("s3");
        // s3.AddChild(cannotSeeCop);
        // s3.AddChild(selectObject);

        // Sequence s4 = new Sequence("s4");
        // s4.AddChild(cannotSeeCop);
        // s4.AddChild(goToVan);

        BehaviorTree stealConditions = new BehaviorTree();
        Sequence conditions = new Sequence("Stealing Conditions");
        conditions.AddChild(invertCanSeeCop);
        conditions.AddChild(invertMoney);
        stealConditions.AddChild(conditions);
        
        DepSequence steal = new DepSequence("Steal Something", stealConditions, navMeshAgent);
        // steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        steal.AddChild(selectObject);
        steal.AddChild(goToVan);

        Selector stealWithFallback = new Selector("Steal With Fallback");
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);

        // steal.AddChild(s1);
        // steal.AddChild(s2);
        // steal.AddChild(s3);
        // steal.AddChild(s4);

        runAway.AddChild(canSeeCop); // parent the Selector to the inverter
        runAway.AddChild(fleeFromCop); // add the flee leaf which runs on success

        Selector beThief = new Selector("Be a thief");
        beThief.AddChild(stealWithFallback);
        beThief.AddChild(runAway);

        tree.AddChild(beThief);
        tree.PrintTree();

        StartCoroutine(DecreaseMoney());
    }

    IEnumerator DecreaseMoney()
    {
        while (true)
        {
            money = Mathf.Clamp(money - 50, 0, 1000);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 5));
        }
    }

    private Node.Status CanSeeCop()
    {
        return CanSee(cop.transform.position, "Cop", 10, 90);
    }

    private Node.Status FleeFromCop()
    {
        return Flee(cop.transform.position, 10);
    }

    private Node.Status HasMoney()
    {
        return money < 500 ? Node.Status.FAILURE : Node.Status.SUCCESS;
    }

    private Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(frontDoor);

        if (status == Node.Status.FAILURE)
        {
            goToFrontDoor._sortOrder = 10;
            return status;
        }
        else
        {
            goToFrontDoor.sortOrder = 1;
        }

        return status;
    }

    private Node.Status GoToBackDoor()
    {
        Node.Status status = GoToDoor(backDoor);

        if (status == Node.Status.FAILURE)
        {
            goToBackDoor._sortOrder = 10;
        }
        else
        {
            goToBackDoor._sortOrder = 1;
        }

        return status;
    }

    private Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf) return Node.Status.FAILURE;

        Node.Status status = GoToLocation(art[i].transform.position);

        if (status == Node.Status.SUCCESS)
        {
            art[i].transform.parent = this.transform;
            stolenObject = art[i];
            return Node.Status.SUCCESS;
        }

        return status;
    }

    private Node.Status GoToVan()
    {
        Node.Status v = GoToLocation(van.transform.position);

        if (v == Node.Status.SUCCESS)
        {
            money += 500;
            stolenObject.transform.parent = null;
            stolenObject.SetActive(false);
            stolenObject = null;
        }

        return v;
    }

    private Node.Status GoToDiamond()
    {
        if (!diamond.activeSelf) return Node.Status.FAILURE;

        Node.Status status = GoToLocation(diamond.transform.position);

        if (status == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.transform;
            stolenObject = diamond;
            return Node.Status.SUCCESS;
        }

        else return status;
    }
}
