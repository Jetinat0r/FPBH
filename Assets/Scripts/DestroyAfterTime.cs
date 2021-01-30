using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    private GameObject toDestroy;
    [SerializeField]
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(time);

        Destroy(toDestroy);
    }
}
