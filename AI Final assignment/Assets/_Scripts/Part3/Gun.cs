using UnityEngine;

public class Gun : Item
{
    [SerializeField] Vector2 radiusMinMax, shootRateMinMax, clipSizeMinMax, minDamageMinMax;
    [SerializeField] LayerMask zombieMask;
    [SerializeField] GameObject bullet;
    Transform shootingPoint;

    bool shooting, pickedUp;
    float shootCD, radius, shootRate, clipSize, currentBullets, minDamage;
    // Start is called before the first frame update

    ZombieBehaviour[] allZombies;

    protected override void Start()
    {
        base.Start();
        

        pickedUp = false;
        shooting = false;

        radius = Random.Range(radiusMinMax.x, radiusMinMax.y);
        shootRate = Random.Range(shootRateMinMax.x, shootRateMinMax.y);
        clipSize = Random.Range((int)clipSizeMinMax.x, (int)clipSizeMinMax.y);
        minDamage = Random.Range(minDamageMinMax.x, minDamageMinMax.y);

        currentBullets = clipSize;

        allZombies = FindObjectsOfType<ZombieBehaviour>();

        FindObjectOfType<ZombieBehaviour>();
    }

    Transform currentNearestZombie;

    // Update is called once per frame
    void Update()
    {
        // Change gun angle
        if (currentNearestZombie != null)
            transform.forward = (currentNearestZombie.position - transform.position).normalized;

        // Update shooting variable
        if (shooting)
            shootCD += Time.deltaTime;
    }

    // This method is called when a zombie is near
    public void Attack()
    {
        if (!pickedUp) return;

        Transform nearestZombie = null;
        float nearestDistance = Mathf.Infinity;
        for (int i = 0; i < allZombies.Length; i++)
        {
            Transform thisZombie = allZombies[i].transform;
            float thisDistance = Vector3.Distance(thisZombie.position, transform.position);

            if (thisDistance < nearestDistance)
            {
                nearestZombie = thisZombie;
                nearestDistance = thisDistance;
            }
        }

        //Debug.Log("nearestZombie = " + nearestZombie);

        // Change gun angle
        currentNearestZombie = nearestZombie;

        if (!shooting) Shoot(nearestZombie);
        else
        {
            if (shootCD >= shootRate)
            {
                shooting = false;
                shootCD = 0;
            }
        }
    }

    private void Shoot(Transform target)
    {
        if (currentBullets <= 0) return;

        Debug.Log("SHOOT, currentBullets = " + currentBullets);

        shooting = true;
        currentBullets--;

        // Instantiate bullet
        GameObject clon = Instantiate(bullet);
        clon.transform.position = shootingPoint.position;
        clon.transform.forward = (target.position - transform.position).normalized;
        clon.GetComponent<Bullet>().startDistance = Vector3.Distance(transform.position, target.position);
        clon.GetComponent<Bullet>().damage = minDamage;
    }

    public void GetAmmo(float bullets)
    {
        currentBullets += bullets;
        if (currentBullets > clipSize) currentBullets = clipSize;
    }

    protected override void PickUp(Transform _human)
    {
        HumanBehaviour humanScript = _human.GetComponent<HumanBehaviour>();
        if (humanScript.haveGun) return;
        else humanScript.haveGun = true;

        base.PickUp(_human);

        if (_human.childCount > 1) Destroy(_human.GetChild(1).gameObject);

        pickedUp = true;
        shootingPoint = _human.GetChild(0);
        transform.parent = _human;
        transform.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
