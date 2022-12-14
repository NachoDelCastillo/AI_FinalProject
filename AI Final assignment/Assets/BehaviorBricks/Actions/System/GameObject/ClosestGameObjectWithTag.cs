﻿using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BBUnity.Actions
{

    /// <summary>
    /// It is an action to find the closest game object with a given label.
    /// </summary>
    [Action("GameObject/ClosestGameObjectWithTag")]
    [Help("Finds the closest game object with a given tag")]
    public class ClosestGameObjectWithTag : GOAction
    {
        ///<value>Input Tag of the target game object.</value>
        [InParam("tag")]
        [Help("Tag of the target game object")]
        public string tag;

        ///<value>OutPut The closest game object with the given tag Parameter.</value>
        [OutParam("foundGameObject")]
        [Help("The closest game object with the given tag")]
        public GameObject foundGameObject;

        private float elapsedTime;

        /// <summary>Initialization Method of ClosestGameObjectWithTag.</summary>
        /// <remarks>Get all the GamesObject with that tag and check which is the closest one.</remarks>
        public override void OnStart()
        {
            GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);

            Transform nearestObj = null;
            float nearestDistance = Mathf.Infinity;
            for (int i = 0; i < allObjects.Length; i++)
            {
                Transform thisObj = allObjects[i].transform;
                float thisDistance = Vector3.Distance(thisObj.position, gameObject.transform.position);

                if (thisDistance < nearestDistance)
                {
                    nearestObj = thisObj;
                    nearestDistance = thisDistance;
                }
            }

            foundGameObject = nearestObj.gameObject;
        }
        /// <summary>Method of Update of ClosestGameObjectWithTag.</summary>
        /// <remarks>Complete the task.</remarks>
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}
