using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{   
    [SerializeField] protected float pickUpRadius;
    [HideInInspector] public int column;

    [HideInInspector] public SpawnManager spawnManager;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();

        sphereCollider.isTrigger = true;
        sphereCollider.radius = pickUpRadius;
    }

    protected void OnTriggerEnter(Collider other)
    {
        HumanBehaviour human = other.GetComponent<HumanBehaviour>();
        if (human == null) return;

        PickUp(human.transform);
    }

    protected virtual void PickUp(Transform _human)
    {
        spawnManager.PickUpItem(column);
    }
}
