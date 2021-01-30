using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterest : MonoBehaviour
{
    [SerializeField]
    private GameObject parentEnemy;

    [SerializeField]
    private float sightAngle = 30f;
    private bool isInterestedInPlayer = false;

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag("Player"))
        {
            float _angleToPlayer = Vector3.Angle(transform.forward, transform.position - _other.transform.position);
            if (_angleToPlayer <= sightAngle)
            {
                isInterestedInPlayer = true;
                parentEnemy.GetComponent<EnemyGrunt>().GainInterest("Player");
                Debug.Log("Player detected");
            }
        }
    }

    private void OnTriggerStay(Collider _other)
    {
        if (isInterestedInPlayer == false && _other.gameObject.CompareTag("Player"))
        {
            float _angleToPlayer = Vector3.Angle(transform.forward, transform.position - _other.transform.position);
            if (_angleToPlayer <= sightAngle)
            {
                isInterestedInPlayer = true;
                parentEnemy.GetComponent<EnemyGrunt>().GainInterest("Player");
                Debug.Log("Player detected");
            }
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.CompareTag("Player"))
        {
            isInterestedInPlayer = false;
            Debug.Log("Lost interest");
            parentEnemy.GetComponent<EnemyGrunt>().LoseInterest("Player");
        }
    }
}
