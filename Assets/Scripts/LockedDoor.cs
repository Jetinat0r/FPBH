using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject[]  enemies;

    [SerializeField]
    private GameObject endDoor;
    [SerializeField]
    private GameObject startDoor;

    [SerializeField]
    private float edDistUp = 10f;
    [SerializeField]
    private float edTimeToUp = 4f;

    [SerializeField]
    private float sdDistDown = 10f;
    [SerializeField]
    private float sdTimeToDown = 4f;

    //This plays a win SFX to tell the player that all enemies are dead
    [SerializeField]
    private AudioSource doorOpenSFX;

    private Vector3 edStartPos;
    private Vector3 edEndPos;

    private Vector3 sdStartPos;
    private Vector3 sdEndPos;

    private bool isUp = false;
    private bool isAlive = true;
    private bool isActivated = false;

    private void Start()
    {
        edStartPos = endDoor.transform.position;
        float _edY = edStartPos.y + edDistUp;
        edEndPos = new Vector3(endDoor.transform.position.x, _edY, endDoor.transform.position.z);

        sdStartPos = startDoor.transform.position;
        float _sdY = sdStartPos.y - sdDistDown;
        sdEndPos = new Vector3(startDoor.transform.position.x, _sdY, startDoor.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            if (!isUp)
            {
                foreach (GameObject _x in enemies)
                {
                    if (_x != null)
                    {
                        isAlive = true;
                        break;
                    }

                    isAlive = false;
                }

                if (!isAlive)
                {
                    isActivated = false;
                    StartCoroutine(OpenDoor(1));
                    StartCoroutine(OpenDoor(3));
                }
            }
        }
    }

    private IEnumerator OpenDoor(int _action)
    {
        float _t = 0f;
        Vector3 _sdCPos = startDoor.transform.position;

        switch (_action)
        {
            //CASES:
            //1: Opens end door
            //2. Closes start door
            //3. Re-opens start door

            

            case 1:

                isUp = true;

                doorOpenSFX.Play();
                
                while (_t < 1)
                {
                    _t += Time.deltaTime / edTimeToUp;
                    endDoor.transform.position = Vector3.Lerp(edStartPos, edEndPos, _t);
                    yield return null;
                }

                break;

            case 2:

                while (_t < 1 && isAlive && isActivated)
                {
                    _t += Time.deltaTime / sdTimeToDown;
                    startDoor.transform.position = Vector3.Lerp(_sdCPos, sdEndPos, _t);
                    yield return null;
                }

                break;

            case 3:

                while (_t < 1 && (!isActivated || ! isAlive))
                {
                    _t += Time.deltaTime / sdTimeToDown;
                    startDoor.transform.position = Vector3.Lerp(_sdCPos, sdStartPos, _t);
                    yield return null;
                }

                break;
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Player"))
        {
            isActivated = true;

            if (isAlive)
            {
                StartCoroutine(OpenDoor(2));
            }

        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Player"))
        {
            isActivated = false;
            StartCoroutine(OpenDoor(3));
        }
    }
}
