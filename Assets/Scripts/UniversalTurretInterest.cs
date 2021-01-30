using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalTurretInterest : MonoBehaviour
{
    //Applied to the interest field of each turret base

    [SerializeField]
    private UniversalTurretBehaviors[] parentTurretScripts;

    private bool isInterestedInPlayer = false;

    private void OnTriggerEnter(Collider _other)
    {
        foreach(UniversalTurretBehaviors _parentTurretScript in parentTurretScripts)
        {
            if (!_parentTurretScript.GetHasTarget())
            {
                if (_other.CompareTag("Player"))
                {
                    isInterestedInPlayer = true;
                    //Debug.Log("Player detected");
                    _parentTurretScript.SetTarget(_other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        foreach (UniversalTurretBehaviors _parentTurretScript in parentTurretScripts)
        {
            if (_other.CompareTag("Player"))
            {
                isInterestedInPlayer = false;
                //Debug.Log("Lost interest");
                _parentTurretScript.SetTarget(null);
            }
        }
    }
}
