using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public bool canSeeTarget;

    public float visionRadius, attackRadius;

    [Range(0, 360)]
    public float visionAngle;

    [SerializeField] LayerMask targetMask, obstructionMask;

    private void Update()
    {
        FieldOfViewCheck();
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionRadius, targetMask);
     
        if (rangeChecks.Length != 0)
        {
            Transform _target = rangeChecks[0].transform;
            Vector3 directionToTarget = (_target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeeTarget = true;
                    target = _target;
                }
                else canSeeTarget = false;
            }
            else canSeeTarget = false;
        }
        else if (canSeeTarget) canSeeTarget = false;
    }
}