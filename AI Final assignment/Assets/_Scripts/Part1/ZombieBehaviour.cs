using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieBehaviour : MonoBehaviour
{
    static public List<ZombieBehaviour> allZombies = new List<ZombieBehaviour>();

    public FieldOfView fov;
    public enum States { Aimless, Chasing, Attacking, Flock }
    public States states;

    bool thisIsLeader;
    ZombieBehaviour followThisLeader;

    [SerializeField] Transform areaToWander;
    Vector3 target;

    [SerializeField] float idleSpeed, chasingSpeed, rotSpeed, force, radiusToAvoid, minDistanceToAvoid;

    [SerializeField] LayerMask obstacleLayer;

    [SerializeField] float maxHealth;
    float currentHealth;

    // When this zombie spot a human, alert every zombie near them
    float alarmDistance = 500;

    // VISUAL FEEDBACK
    [SerializeField]
    Image stateIndicator;
    [Serializable]
    struct ImageStateIndicator
    { public States state; public Sprite sprite; }
    [SerializeField]
    ImageStateIndicator[] indicatorSprites;
    [SerializeField]
    GameObject leaderImage;

    void Start()
    {
        fov = GetComponent<FieldOfView>();
        states = States.Aimless;
        target = ChooseRandomPoint();

        currentHealth = maxHealth;

        allZombies.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(gameObject.name + " : Current State = " + states);

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
            case States.Flock:
                FollowLeader();
                break;
        }

        // Find and assign the respective sprite according to the current state
        stateIndicator.sprite = Array.Find(indicatorSprites, stateSprite => stateSprite.state == states).sprite;

        leaderImage.SetActive(thisIsLeader);
    }

    private void Aimless()
    {
        if (target == null) target = ChooseRandomPoint();

        if (fov.canSeeTarget && fov.target != null)
        {
            if (Vector3.Distance(transform.position, fov.target.position) <= fov.attackRadius) states = States.Attacking;
            else states = States.Chasing;

            // This frame, a player was spotted by this zombie
            // It tries to become a leader

            // Check if there is at least
            if (allZombies.Count <= 1) return;
            thisIsLeader = true;

            // It becomes a leader if the human this zombie is following dosent have any other zombie leaders following him
            for (int i = 0; i < allZombies.Count; i++)
            {
                ZombieBehaviour thisZombie = allZombies[i];
                if (thisZombie == this) continue;

                // If both zombies have the same target
                if (thisZombie.target == fov.target.position)
                {
                    // And if this other zombie is a leader
                    if (thisZombie.thisIsLeader)
                        thisIsLeader = false;
                }
            }

            // If this zombie didnte became a leader, stop this process
            if (!thisIsLeader) return;

            // Turn into flock state every zombie that is near
            for (int i = 0; i < allZombies.Count; i++)
            {
                ZombieBehaviour thisZombie = allZombies[i];

                // Not this zombie
                if (thisZombie == this) continue;

                // If this zombie is near enough
                if (Vector3.Distance(thisZombie.transform.position, transform.position) < alarmDistance && 
                    thisZombie.states == States.Aimless)
                {
                    // Turn him into a flock
                    thisZombie.ConvertToFlock(this);
                }
            }

            return;
        }

        if (Vector3.Distance(transform.position, target) <= 5) target = ChooseRandomPoint();
        else
            MoveToCurrentTarget();
    }

    private void FollowLeader()
    {
        // If this zombie is near the human, start chasing him instead
        if (fov.canSeeTarget)
        {
            states = States.Chasing;
            followThisLeader = null; // Delete this reference

            return;
        }

        target = followThisLeader.transform.position;

        MoveToCurrentTarget();
    }

    private void Chasing()
    {
        float distanceToTarget = -1;
        if (fov.target != null)
            distanceToTarget = Vector3.Distance(transform.position, fov.target.position);

        if (fov.target == null || distanceToTarget >= fov.visionRadius + 3)
        {
            // If this zombie dont know where the player is or zome zombie killed the player, start wandering again 
            states = States.Aimless;
            // And if it was a leader, rearrange the flock

            if (thisIsLeader)
                RearrangeFlock();

            return;
        }
        else if (distanceToTarget <= fov.attackRadius) { states = States.Attacking; return; }

        target = fov.target.position;

        MoveToCurrentTarget();
    }

    float attackRecoil = .5f;
    float attackRecoilMax = .5f;
    private void Attacking()
    {
        float distanceToTarget = -1;
        if (fov.target != null)
            distanceToTarget = Vector3.Distance(transform.position, fov.target.position);

        // If the human got away or the human died because of another zombie
        if (fov.target == null || distanceToTarget > fov.attackRadius) { states = States.Chasing; return; }

        attackRecoil -= Time.deltaTime;
        if (attackRecoil <= 0)
        {
            attackRecoil = attackRecoilMax;

            // Destroy the human
            Destroy(fov.target.gameObject);

            // Wander around searching for other humans
            states = States.Aimless;

            // If it is a leader, inform the rest of zombies that the human is dead
            if (thisIsLeader)
                RearrangeFlock();
        }
    }

    private void MoveToCurrentTarget()
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();

        // See if the agent must avoid anything in front of it
        AvoidObstacles(ref dir);

        //rotate towards the target pos
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

        //Move
        transform.Translate(transform.forward * chasingSpeed * Time.deltaTime, Space.World);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaToWander.position, new Vector3(areaToWander.GetComponent<MeshCollider>().bounds.size.x, 5, areaToWander.GetComponent<MeshCollider>().bounds.size.z));
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

    public void ConvertToFlock(ZombieBehaviour flockLeader)
    {
        states = States.Flock;

        followThisLeader = flockLeader;
    }


    // This method is called by the leader of the flock when killed or when he cannot longer spot the player anymore
    void RearrangeFlock()
    {
        thisIsLeader = false;

        // If this was a leader, pass the leader role to the closest zombie to the human that is chasing him.
        ZombieBehaviour closestZombieToHuman = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < allZombies.Count; i++)
        {
            ZombieBehaviour thisZombie = allZombies[i];
            if (thisZombie == this) continue;

            float thisDistance = Vector3.Distance(thisZombie.transform.position, target);

            if (thisDistance < closestDistance)
            {
                closestDistance = thisDistance;
                closestZombieToHuman = thisZombie;
            }
        }

        // There is no flock, leader of nothing
        if (closestZombieToHuman == null) return;

        // If the closest zombie to the human that was being followed is chasing him, make him the leader
        if (closestZombieToHuman.states == States.Chasing && closestZombieToHuman.target == target)
        {
            closestZombieToHuman.thisIsLeader = true;

            // Find all the zombies that are chasing this zombie and make them follow the new leader.
            for (int i = 0; i < allZombies.Count; i++)
            {
                ZombieBehaviour thisZombie = allZombies[i];
                if (thisZombie == this) continue;

                if (thisZombie.followThisLeader == this)
                    thisZombie.followThisLeader = closestZombieToHuman;
            }
        }

        // If no zombie is still following this human, make all the zombies that were following this zombie walk aimlessly
        else
        {
            // Find all the zombies that are chasing this zombie and make them follow the new leader.
            for (int i = 0; i < allZombies.Count; i++)
            {
                if (allZombies[i].followThisLeader == this)
                    allZombies[i].states = States.Aimless;
            }
        }
    }

    private void OnDestroy()
    {
        allZombies.Remove(this);

        if (allZombies.Count == 0) return;

        if (thisIsLeader)
            RearrangeFlock();
    }
}
