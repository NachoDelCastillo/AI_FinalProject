using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed;
    [HideInInspector] public float startDistance;
    [HideInInspector] public float damage;

    [SerializeField] AnimationCurve damageCurve;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        HumanBehaviour human = other.GetComponent<HumanBehaviour>();
        if (human != null) return;

        Destroy(gameObject);

        ZombieBehaviour zombie = other.GetComponent<ZombieBehaviour>();
        if (zombie == null) return;

        zombie.GetDamage(damage + damageCurve.Evaluate(startDistance));
    }
}
