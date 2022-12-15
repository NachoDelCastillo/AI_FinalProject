using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class HumanBehaviour : MonoBehaviour
{
    static public List<HumanBehaviour> allHumans = new List<HumanBehaviour>();

    [SerializeField] Transform startingLine, safeZone;
    [SerializeField] MeshCollider area;

    [SerializeField] LayerMask ground;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public bool selected;

    [SerializeField] Material noSel, sel;
    MeshRenderer meshRenderer;

    [SerializeField] Vector3 target;
    // Start is called before the first frame update

    [SerializeField]
    List<Vector3> waypoints = new List<Vector3>();
    List<GameObject> waypointsVisualFeedback = new List<GameObject>();

    public bool haveGun;

    [SerializeField]
    public TMP_Text ammoText;

    [SerializeField]
    GameObject waypointVisualFeedback;

    // This variables
    [HideInInspector]
    public ZombieBehaviour zombieLeaderChasingThis;

    [SerializeField]
    bool startBeingSelected;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        selected = false;

        float x = Random.Range(0, area.bounds.extents.x - 5);
        if (Random.Range(0, 2) == 0) x *= -1;

        if (startingLine != null)
            transform.position = new Vector3(area.bounds.center.x + x, transform.position.y, startingLine.position.z);

        allHumans.Add(this);

        selected = startBeingSelected;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, safeZone.position) < 6)
            GameManager.GetInstance().HumansWin();
    }

    private void LateUpdate()
    {
        if (!selected)
            meshRenderer.material = noSel;
        else
        {
            meshRenderer.material = sel;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground))
                {

                    Vector3 newPosition = new Vector3(hitInfo.point.x, startingLine.position.y, hitInfo.point.z);
                    waypoints.Add(newPosition);
                    waypointsVisualFeedback.Add(Instantiate(waypointVisualFeedback, newPosition, Quaternion.identity));

                    Debug.Log("NavMesh.SamplePosition(); = " + NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 1000, 0));
                }
            }
        }

        // If there are any waypoints go there, if not, go to safeZone
        if (waypoints.Count >= 1)
            target = waypoints[0];
        else target = safeZone.position;

        agent.SetDestination(target);

        if (Vector3.Distance(transform.position, target) >= 5) agent.isStopped = false;

        // If the human reached a waypoint, delete it from the list
        if (Vector3.Distance(transform.position, target) <= 5 && target != safeZone.position)
        {
            waypoints.RemoveAt(0);
            GameObject oldWaypoint = waypointsVisualFeedback[0];
            waypointsVisualFeedback.RemoveAt(0);
            Destroy(oldWaypoint);
        }
    }

    private void OnDestroy()
    {
        allHumans.Remove(this);

        if (allHumans.Count == 0)
            GameManager.GetInstance().ZombiesWin();

        for (int i = 0; i < waypointsVisualFeedback.Count; i++)
            Destroy(waypointsVisualFeedback[i]);
    }
}