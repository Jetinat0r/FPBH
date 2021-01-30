using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpin : MonoBehaviour
{
    [SerializeField]
    private float itemRotationSpeed = 50f;
    [SerializeField]
    private float itemBobSpeed = 2f;
    private float bobOffset;

    private Vector3 basePosition;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
        bobOffset = Random.Range(-100f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
        transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin((Time.time * itemBobSpeed) + bobOffset), 0f);
    }
}
