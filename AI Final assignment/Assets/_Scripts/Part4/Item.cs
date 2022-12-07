using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{   
    [SerializeField] protected float pickUpRadius;
    [HideInInspector] public int column;

    [HideInInspector] public SpawnManager spawnManager;
    SphereCollider sphereCollider;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();

        sphereCollider.isTrigger = true;
        sphereCollider.radius = pickUpRadius;
    }

    protected void OnTriggerEnter(Collider other)
    {
        HumanBehaviour human = other.GetComponent<HumanBehaviour>();
        if (human == null) return;

        PickUp(human.transform);
        sphereCollider.enabled = false;
    }

    protected virtual void PickUp(Transform _human)
    {
        spawnManager.PickUpItem(column);
    }
}
