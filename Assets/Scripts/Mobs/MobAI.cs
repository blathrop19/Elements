﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]

public class MobAI : MonoBehaviour
{
	public GameObject[] waypoints;
	public GameObject target;

	private int currWaypoint = -1;
	private NavMeshAgent navMeshAgent;
	private Animator animator;
	private VelocityReporter velocityReporter;
	private Vector3 prevVelocity;


	public float animationSpeed = 1f;
	public float rootMovementSpeed = 1f;
	public float rootTurnSpeed = 1f;
    public float fallSpeed = 1f;

	public AIState aiState;

	private void setNextWaypoint(){ 
		try {
			currWaypoint = (currWaypoint + 1) % waypoints.Length;
			navMeshAgent.SetDestination(waypoints[currWaypoint].transform.position);
		} catch {
			Debug.Log ( "Next Waypoint cannot be set due to array indexing issue or array is of length 0 " );
		}
	}

	void Start(){
		setNextWaypoint ();
		navMeshAgent = GetComponent<NavMeshAgent>();
		animator= GetComponent<Animator>();
		aiState = AIState.Patrol;
		velocityReporter = target.GetComponent<VelocityReporter>();
	}

	void Update(){

		if ( Input.GetKey(KeyCode.G) ) {
			if (aiState == AIState.Patrol) {
				aiState = AIState.InterceptTarget;
			}
		}

		if (aiState == AIState.Patrol) {
			if (navMeshAgent.remainingDistance < .5 && !navMeshAgent.pathPending) {
				setNextWaypoint ();
			}
			animator.SetFloat ("vely", navMeshAgent.velocity.magnitude / navMeshAgent.speed);
			animator.SetFloat ("velx", ( prevVelocity.x - navMeshAgent.velocity.x ) / navMeshAgent.speed);
		} else if (aiState == AIState.InterceptTarget) {
			try {
				navMeshAgent.SetDestination(target.transform.position + velocityReporter.velocity);
			} catch {
				Debug.Log ( "Next Waypoint cannot be set due to array indexing issue or array is of length 0 " );
			}
			animator.SetFloat ("vely", navMeshAgent.velocity.magnitude / navMeshAgent.speed);
		} else if (aiState == AIState.Wait) {
			animator.SetFloat ("vely", 0);
		}
		prevVelocity = navMeshAgent.velocity;
	}
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "target"){
			aiState = AIState.Patrol;
		}
	}
}


public enum AIState
{
	Patrol,
	InterceptTarget,
	Wait
};

