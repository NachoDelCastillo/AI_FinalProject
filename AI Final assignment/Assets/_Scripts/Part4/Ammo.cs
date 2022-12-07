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
        base.PickUp(_human);

        if (_human.childCount > 1)
        {
            _human.GetChild(1).GetComponent<Gun>().GetAmmo(numberOfBullets);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
