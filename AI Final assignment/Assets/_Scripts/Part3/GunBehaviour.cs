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
        [InParam("NearestZombie")]
        public GameObject nearestZombie;

        public override void OnStart()
        {
            Gun gunScript = gameObject.GetComponent<Gun>();

            gunScript.Attack();

            base.OnStart();
        } // OnStart


        //public override TaskStatus OnUpdate()
        //{

        //    // The action is completed. We must inform the execution engine.
        //    return TaskStatus.COMPLETED;
        //} // OnUpdate
    }
}