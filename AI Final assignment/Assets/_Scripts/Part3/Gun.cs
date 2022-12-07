using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Vector2 radiusMinMax, shootRateMinMax, clipSizeMinMax, minDamageMinMax;
    [SerializeField] LayerMask zombieMask;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootingPoint;

    bool shooting;
    float shootCD, radius, shootRate, clipSize, currentBullets, minDamage;
    // Start is called before the first frame update
    void Start()
    {
        shooting = false;

        radius = Random.Range(radiusMinMax.x, radiusMinMax.y);
        shootRate = Random.Range(shootRateMinMax.x, shootRateMinMax.y);
        clipSize = Random.Range(clipSizeMinMax.x, clipSizeMinMax.y);
        minDamage = Random.Range(minDamageMinMax.x, minDamageMinMax.y);

        currentBullets = clipSize;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] nearbyZombies = Physics.OverlapSphere(transform.position, radius, zombieMask);
        if (nearbyZombies.Length <= 0) return;

        Transform nearestZombie = null;
        for (int i = 0; i < nearbyZombies.Length; i++)
        {
            if (i == 0) nearestZombie = nearbyZombies[i].transform;
            else if(Vector3.Distance(transform.position, nearbyZombies[i].transform.position) < Vector3.Distance(transform.position, nearestZombie.position)) nearestZombie = nearbyZombies[i].transform;
        }

        if (!shooting) Shoot(nearestZombie);
        else
        {
            shootCD += Time.deltaTime;
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

        shooting = true;
        currentBullets--;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
