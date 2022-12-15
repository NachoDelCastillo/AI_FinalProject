using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Item
{
    [SerializeField] Vector2 numberOfBulletsMinMax;
    int numberOfBullets;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        numberOfBullets = Random.Range((int)numberOfBulletsMinMax.x, (int)numberOfBulletsMinMax.y);
    }
    protected override void PickUp(Transform _human)
    {
        // If it dosent have a gun, dont pick it up
        if (!_human.GetComponent<HumanBehaviour>().haveGun) return;

        _human.GetComponentInChildren<Gun>().GetAmmo(numberOfBullets);

        base.PickUp(_human);
        Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        HumanBehaviour human = other.GetComponent<HumanBehaviour>();
        if (human == null) return;

        PickUp(human.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
