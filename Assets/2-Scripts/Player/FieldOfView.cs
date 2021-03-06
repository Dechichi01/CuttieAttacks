﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {
	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
	public List<Transform> visibleTargets = new List<Transform>();

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	void Start()
	{
		//StartCoroutine(FindTargetsWithDelay(.2f));
	}

    void Update()
    {
        FindVisibleTargets();
    }

	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			 yield return new WaitForSeconds(delay);
			 FindVisibleTargets();
		}
	}

	void FindVisibleTargets()
	{
		visibleTargets.Clear();
		//Any targets in our FOV?
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			//Any target in our viewAngle
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle/2)
			{
				float dstToTarget = Vector3.Distance(transform.position,target.position);
				//Is there any obstacle blocking the view?
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					visibleTargets.Add(target);
				}
			}
		}
	}
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
			angleInDegrees += transform.eulerAngles.y;
		return new Vector3(Mathf.Sin(angleInDegrees*Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
	}
}
