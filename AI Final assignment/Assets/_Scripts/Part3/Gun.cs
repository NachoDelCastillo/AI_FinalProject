using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : Item
{
    [SerializeField] Vector2 radiusMinMax, shootRateMinMax, clipSizeMinMax, minDamageMinMax;
    [SerializeField] LayerMask zombieMask;
    [SerializeField] GameObject bullet;
    Transform shootingPoint;

    bool shooting, pickedUp;
    float shootCD, radius, shootRate, clipSize, currentBullets, minDamage;
    // Start is called before the first frame update

    TMP_Text ammoText;

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

        List<ZombieBehaviour> allZombies = ZombieBehaviour.allZombies;

        Transform nearestZombie = null;
        float nearestDistance = Mathf.Infinity;
        for (int i = 0; i < allZombies.Count; i++)
        {
            Transform thisZombie = allZombies[i].transform;
            float thisDistance = Vector3.Distance(thisZombie.position, transform.position);

            if (thisDistance < nearestDistance)
            {
                nearestZombie = thisZombie;
                nearestDistance = thisDistance;
            }
        }

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

        shooting = true;
        currentBullets--;
        UpdateAmmoText();

        // Instantiate bullet
        GameObject clon = Instantiate(bullet);
        clon.transform.position = shootingPoint.position;
        clon.transform.forward = (target.position - transform.position).normalized;
        clon.GetComponent<Bullet>().startDistance = Vector3.Distance(transform.position, target.position);
        clon.GetComponent<Bullet>().damage = minDamage;
    }

    // Returns false when the ammo is full and can not have more bullets, in this case, dont pick up the ammo
    public bool GetAmmo(float bullets)
    {
        if (currentBullets == clipSize) return false;

        currentBullets += bullets;
        if (currentBullets > clipSize) currentBullets = clipSize;

        UpdateAmmoText();

        return true;
    }

    protected override void PickUp(Transform _human)
    {
        HumanBehaviour humanScript = _human.GetComponent<HumanBehaviour>();
        if (humanScript.haveGun) return;
        else humanScript.haveGun = true;

        base.PickUp(_human);

        //if (_human.childCount > 1) Destroy(_human.GetChild(2).gameObject);

        pickedUp = true;
        shootingPoint = _human.GetChild(0);
        transform.parent = _human;
        transform.localPosition = Vector3.zero;


        // Activate the ammo text
        ammoText = humanScript.ammoText;
        UpdateAmmoText();
    }

    void UpdateAmmoText()
    {
        ammoText.text = currentBullets + "/" + clipSize;

        if (currentBullets == 0) ammoText.color = Color.red;
        else ammoText.color = Color.black;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
