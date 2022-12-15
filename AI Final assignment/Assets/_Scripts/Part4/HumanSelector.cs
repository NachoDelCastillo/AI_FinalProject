using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSelector : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                // Get the current list of humans and convert it into an array
                List<HumanBehaviour> allHumans = HumanBehaviour.allHumans;
                HumanBehaviour[] humans = new HumanBehaviour[allHumans.Count];
                int j = 0;
                foreach (HumanBehaviour human in allHumans)
                    humans[j++] = human;

                // Get humans near the click
                List<HumanBehaviour> nearHumans = new();
                for (int i = 0; i < humans.Length; i++)
                {
                    if (Vector3.Distance(hitInfo.point, humans[i].transform.position) > 5) continue;

                    nearHumans.Add(humans[i]);
                }

                if (nearHumans.Count == 0) return;

                HumanBehaviour nearestHuman = nearHumans[0];

                if (nearHumans.Count > 1)
                {
                    foreach (var human in nearHumans)
                    {
                        if (Vector3.Distance(hitInfo.point, human.transform.position) < Vector3.Distance(hitInfo.point, nearestHuman.transform.position)) nearestHuman = human;
                    }
                }

                for (int i = 0; i < humans.Length; i++)
                {
                    if (humans[i].selected && humans[i] != nearestHuman) humans[i].agent.isStopped = true;
                    if (humans[i] != nearestHuman) humans[i].selected = false;
                }
                nearestHuman.selected = true;
            }
        }   
    }
}
