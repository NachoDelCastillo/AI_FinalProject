using UnityEngine;
using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus

namespace BBUnity.Actions // Programmers Quick Start Guide
{

    [Action("Samples/ProgQuickStartGuide/GunBehaviour")]
    [Help("Clone a 'bullet' and shoots it throught the Forward axis with the " +
      "specified velocity.")]
    public class GunBehaviour : GOAction
    {

        [InParam("NearestZombie", DefaultValue = null)]
        public GameObject nearestZombie;

        public override void OnStart()
        {
            Debug.Log("ON START GUN BEHAVIOUR" + gameObject);

            Gun gunScript = gameObject.GetComponent<Gun>();

            gunScript.Attack(nearestZombie.transform);

            base.OnStart();
        } // OnStart


        //public override TaskStatus OnUpdate()
        //{

        //    // The action is completed. We must inform the execution engine.
        //    return TaskStatus.COMPLETED;
        //} // OnUpdate
    }
}