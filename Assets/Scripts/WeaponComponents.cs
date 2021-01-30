using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponents : MonoBehaviour
{
    //Goes on each individual weapon

    [SerializeField]
    public PlayerWeapon playerWeaponInformation;

    [SerializeField]
    public Rigidbody rb;
    [SerializeField]
    public BoxCollider pickupCollider;

    [Header("Graphics")]
    //Barrel of the gun
    public GameObject shotEffectPrefab;
    public float shotEffectDuration;

    //Between gun and hit point
    public float lazerDuration;
    public GameObject lazerPrefab;

    //At hit point
    public float hitEffectDuration;
    public GameObject lazerHitPrefab;
    //public GameObject projectilePrefab;
    public Transform shootPoint;

    public void RecieveWeaponGraphics(GameObject _shotEffectPrefab, float _shootEffectDuration, GameObject _projectilePrefab, Transform _shootPoint)
    {
        shotEffectPrefab = _shotEffectPrefab;
        shotEffectDuration = _shootEffectDuration;
        lazerPrefab = _projectilePrefab;
        shootPoint = _shootPoint;
    }

    public void WeaponPickedUp()
    {
        rb.isKinematic = true;
        pickupCollider.enabled = false;

        //KINDA A HACK, MAY NOT ALWAYS WORK
        foreach (Transform child in playerWeaponInformation.weaponGraphics.transform)
        {
            child.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void WeaponDropped()
    {
        rb.isKinematic = false;
        pickupCollider.enabled = true;

        //KINDA A HACK, MAY NOT ALWAYS WORK
        foreach (Transform child in playerWeaponInformation.weaponGraphics.transform)
        {
            child.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
