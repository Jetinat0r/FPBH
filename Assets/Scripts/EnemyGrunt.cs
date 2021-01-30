using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyGrunt : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    private Vector3 home;

    [SerializeField]
    private GameObject interestRadius;
    [SerializeField]
    private LayerMask playerMask;
    private bool canSeePlayer = false;
    private bool isHuntingPlayer = false;
    private bool canSeeForceField = false;
    private bool isInForceField = false;

    // Start is called before the first frame update
    void Start()
    {
        home = transform.position;
        Debug.Log("Where the heart is: " + home);
    }

    // Update is called once per frame
    void Update()
    {
        if (isHuntingPlayer)
        {
            Debug.Log("Moving towards player");
            navMeshAgent.SetDestination(FindObjectOfType<Player>().gameObject.transform.position);
        }
    }

    private void ChooseDestination()
    {
        if (canSeeForceField)
        {
            //Force field is destination
        }
        else if (canSeePlayer)
        {
            isHuntingPlayer = true;
        }
        else if (Vector3.Distance(transform.position, home) > 5f)
        {
            Debug.Log("Going home " + home);
            navMeshAgent.SetDestination(home);
        }
    }

    public void GainInterest(string _entity)
    {
        //Sets variables
        if(_entity == "Player")
        {
            canSeePlayer = true;
        }
        else if(_entity == "Force Field")
        {
            canSeeForceField = true;
        }

        ChooseDestination();
    }

    public void LoseInterest(string _entity)
    {
        if (_entity == "Player")
        {
            canSeePlayer = false;
            isHuntingPlayer = false;
        }
        else if (_entity == "Force Field")
        {
            //For when a shield bearer dies
            canSeeForceField = false;
        }

        ChooseDestination();
    }
}
