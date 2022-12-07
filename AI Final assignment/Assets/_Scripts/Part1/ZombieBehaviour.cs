using System;
using UnityEngine;

public class ZombieBehaviour : MonoBehaviour
{
    FieldOfView fov;
    enum States { Aimless, Chasing, Attacking }
    States states;

    [SerializeField] Transform areaToWander;
    Vector3 target;

    [SerializeField] float idleSpeed, chasingSpeed, rotSpeed, force, radiusToAvoid, minDistanceToAvoid;

    [SerializeField] LayerMask obstacleLayer;

    [SerializeField] float maxHealth;
    float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        fov = GetComponent<FieldOfView>();
        states = States.Aimless;
        target = ChooseRandomPoint();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        switch (states)
        {
            case States.Aimless:
                Aimless();
                break;
            case States.Chasing:
                Chasing();
                break;
            case States.Attacking:
                Attacking();
                break;
        }
    }

    private void Aimless()
    {
        if (fov.canSeeTarget)
        {
            if (Vector3.Distance(transform.position, fov.target.position) <= fov.attackRadius) states = States.Attacking;
            else states = States.Chasing;
            return;
        }

        if (Vector3.Distance(transform.position, target) <= 5) target = ChooseRandomPoint();
        else
        {
            Vector3 dir = target - transform.position;
            dir.Normalize();

            // See if the agent must avoid anything in front of it
            AvoidObstacles(ref dir);

            //rotate towards the target pos
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

            //Move
            transform.Translate(transform.forward * idleSpeed * Time.deltaTime, Space.World);
        }
    }

    private void Chasing()
    {
        if (Vector3.Distance(transform.position, fov.target.position) <= fov.attackRadius) { states = States.Attacking; return; }
        if (!fov.canSeeTarget) { states = States.Aimless; return; }

        target = fov.target.position;

        Vector3 dir = target - transform.position;
        dir.Normalize();

        // See if the agent must avoid anything in front of it
        AvoidObstacles(ref dir);

        //rotate towards the target pos
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

        //Move
        transform.Translate(transform.forward * chasingSpeed * Time.deltaTime, Space.World);
    }
    private void Attacking()
    {
        if (!fov.canSeeTarget) { states = States.Aimless; return; }
        if (Vector3.Distance(transform.position, fov.target.position) > fov.attackRadius) { states = States.Chasing; return; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(areaToWander.position, new Vector3(areaToWander.GetComponent<MeshCollider>().bounds.size.x, areaToWander.GetComponent<MeshCollider>().bounds.size.y, 1));
        Gizmos.DrawWireCube(areaToWander.position, areaToWander.GetComponent<MeshCollider>().bounds.size);
        Gizmos.DrawLine(transform.position, target);
    }

    Vector3 ChooseRandomPoint()
    {
        MeshCollider area = areaToWander.GetComponent<MeshCollider>();

        float x = UnityEngine.Random.Range(0, area.bounds.extents.x);
        float z = UnityEngine.Random.Range(0, area.bounds.extents.z);

        int randX = UnityEngine.Random.Range(0, 2);
        int randZ = UnityEngine.Random.Range(0, 2);

        if (randX == 0) x *= -1;
        if (randZ == 0) z *= -1;

        return new Vector3(area.bounds.center.x + x, transform.position.y, area.bounds.center.x + z);
    }

    public void AvoidObstacles(ref Vector3 dir)
    {
        //Check if the agent can "see" obstacles in front of itself
        if (Physics.BoxCast(transform.position, transform.localScale * radiusToAvoid * 0.5f, transform.forward, out RaycastHit hit, Quaternion.identity, minDistanceToAvoid, obstacleLayer))
        {
            // Get the normal vector of the hit point
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0f;

            dir = Vector3.Lerp(transform.forward, transform.forward + hitNormal * force, Time.deltaTime * rotSpeed);
        }
    }

    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
