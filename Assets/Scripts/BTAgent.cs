using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BTAgent : MonoBehaviour
{
    public enum ActionState { IDLE, WORKING }

    public BehaviorTree tree;
    public NavMeshAgent navMeshAgent;
    public ActionState state = ActionState.IDLE;
    public Node.Status treeStatus = Node.Status.RUNNING;
    WaitForSeconds waitForSeconds;
    Vector3 rememberedLocation;

    public virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        tree = new BehaviorTree();
        // waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 0.4f));
        waitForSeconds = new WaitForSeconds(0.1f);
        StartCoroutine(Behave());
    }

    IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return waitForSeconds;
        }
    }

    protected Node.Status CanSee(Vector3 target, string tag, float distance, float maxAngle)
    {
        Transform t = transform;
        Vector3 directionToTarget = target - t.position;
        float angle = Vector3.Angle(directionToTarget, t.forward);

        if (angle <= maxAngle && directionToTarget.magnitude <= distance)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(this.transform.position, directionToTarget, out hitInfo))
            {
                if (hitInfo.collider.gameObject.CompareTag(tag))
                {
                    return Node.Status.SUCCESS;
                }
            }
        }

        return Node.Status.FAILURE;
    }

    public Node.Status Flee(Vector3 location, float distance)
    {
        if (state == ActionState.IDLE)
        {
            rememberedLocation = this.transform.position + (transform.position - location).normalized * distance;
        }

        return GoToLocation(rememberedLocation);
    }

    public Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        // Debug.Log("<color=cyan>Distance to target = " + distanceToTarget + "</color>");

        if (state == ActionState.IDLE)
        {
            navMeshAgent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        // else if (Vector3.Distance(navMeshAgent.pathEndPosition, destination) >= 2)
        else if (Vector3.Distance(navMeshAgent.pathEndPosition, destination) >= 2)
        {
            // Debug.Log("Agent is " + distanceToTarget + " away from destination");
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            Debug.Log("<color=green>Agent has arrived at " + destination + "</color>");
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING; // for the first time state switches from IDLE to WORKING
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GoToLocation(door.transform.position);

        if (status == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.GetComponent<NavMeshObstacle>().enabled = false;
                Debug.Log("<color=green>Door is open: " + door + "</color>");
                return Node.Status.SUCCESS;
            }

            Debug.Log("<color=red>Door is locked: " + door + "</color>");
            return Node.Status.FAILURE;
        }
        else
        {
            Debug.Log("<color=yellow>Going to door - status: " + status + "</color>");
            return status;
        }
    }
}
