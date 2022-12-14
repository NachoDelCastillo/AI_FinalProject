using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotation : MonoBehaviour
{
    // This is only used in the state indicator of the zombies
    void Update()
    { transform.rotation = Quaternion.Euler(0, 0, 0); }
}
