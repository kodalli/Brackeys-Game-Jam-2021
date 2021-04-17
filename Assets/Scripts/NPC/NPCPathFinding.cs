using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPathFinding : MonoBehaviour
{
    public static NPCPathFinding Instance;
    [SerializeField] Transform target;
    NavMeshAgent agent;
    void Start(){
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    public void GoToLocation() {
        agent.SetDestination(target.position);
    }
}
