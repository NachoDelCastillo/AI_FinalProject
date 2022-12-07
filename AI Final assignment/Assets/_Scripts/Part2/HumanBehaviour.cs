using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanBehaviour : MonoBehaviour
{
    [SerializeField] Transform startingLine;
    [SerializeField] MeshCollider area;

    [SerializeField] LayerMask ground;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public bool selected;

    [SerializeField] Material noSel, sel;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        selected = false;

        float x = Random.Range(0, area.bounds.extents.x - 5);
        if (Random.Range(0, 2) == 0) x *= -1;
        transform.position = new Vector3(area.bounds.center.x + x, transform.position.y, startingLine.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!selected) {meshRenderer.material = noSel; return; }

        meshRenderer.material = sel;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hitInfo , Mathf.Infinity, ground))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }
}
