using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurretBehavior : UniversalTurretBehaviors
{
    //Tells the basic turret how to behave

    [Header("Basic Turret Variables")]
    [SerializeField]
    private GameObject turretPivot;
    [SerializeField]
    private GameObject basicTurretBulletPrefab;
    [SerializeField]
    private GameObject shootPoint;
    [SerializeField]
    private GameObject turretDeathEffectPrefab;
    [SerializeField]
    private float turretDeathEffectDuration;

    [SerializeField]
    private float health = 25f;

    //One shot every shot cooldown
    [SerializeField]
    private float shotCooldown = 4f;
    [SerializeField]
    private float shotCooldownRandomRange = 0.25f;
    [SerializeField]
    private float turnSpeed = 0.02f;
    [SerializeField]
    private float shotVelocity = 3f;

    //[Header("Vertical Rotation Locks")]
    //[SerializeField]
    //private float positiveRotationRestraint = 85f;
    //[SerializeField]
    //private float negativeRotationRestraint = -85f;

    [Header("Idle Animation")]
    [SerializeField]
    private GameObject pointOne;
    private Vector3 pointOnePos;
    [SerializeField]
    private GameObject pointTwo;
    private Vector3 pointTwoPos;
    [SerializeField]
    private float idleHoldTime = 2f;
    [SerializeField]
    private AudioSource idleBeeper;
    //Is it trying to move towards idle point one or not, not means two
    private bool idlePointOne = true;
    //So it doesn't start a bunch of coroutines
    private bool isHeld = false;

    [SerializeField]
    private GameObject[] thingsToDestroy;

    private int visionMask;
    private bool canSeePlayer = false;
    private float shootTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        shotCooldown += Random.Range(-shotCooldownRandomRange, shotCooldownRandomRange);
        visionMask = CreateVisionMask();

        //Setup for idle animation
        pointOnePos = pointOne.transform.position;
        pointTwoPos = pointTwo.transform.position;

        Destroy(pointOne);
        Destroy(pointTwo);
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            RaycastHit _hit;

            //Debug.DrawRay(turretPivot.transform.position, target.transform.position - turretPivot.transform.position, new Color(255, 0, 0));

            //                                                                                                                layermask so turret nerds can shoot player in force fields
            if (Physics.Raycast(turretPivot.transform.position, target.transform.position - turretPivot.transform.position, out _hit, Mathf.Infinity, visionMask))
            {
                //Debug.Log(_hit.collider.gameObject.name);
                if (_hit.collider.CompareTag("Player"))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }



            if (canSeePlayer == true)
            {
                //Current best solution (should be better tho)
                Vector3 _targetDirection = target.transform.position - turretPivot.transform.position;

                //Makes it look like its looking at the player rather than their legs
                _targetDirection.y += 0.45f;

                Vector3 _newDirection = Vector3.RotateTowards(turretPivot.transform.forward, _targetDirection * Time.deltaTime, turnSpeed, 0f);

                turretPivot.transform.rotation = Quaternion.LookRotation(_newDirection);

                shootTimer += Time.deltaTime;

                if (shootTimer >= shotCooldown)
                {
                    Shoot();
                    shootTimer = 0;
                }
            }
            else
            {
                //Idle animation
                //Done like this so the turrets arent moving when they dont need to be
                //AKA: If a turret beeps and nobody's around to hear it, did it beep at all?
                //Probably gonna be two points around where it saves their positions and then destroys them and uses those points to look back and forth

                if (idlePointOne)
                {
                    Vector3 _targetDirection = pointOnePos - turretPivot.transform.position;

                    Vector3 _newDirection = Vector3.RotateTowards(turretPivot.transform.forward, _targetDirection * Time.deltaTime, turnSpeed * Random.Range(0.9f, 1f), 0f);

                    turretPivot.transform.rotation = Quaternion.LookRotation(_newDirection);

                    if (turretPivot.transform.forward == _newDirection && !isHeld)
                    {
                        isHeld = true;
                        StartCoroutine(SwapIdlePoint());
                    }
                }
                else
                {
                    Vector3 _targetDirection = pointTwoPos - turretPivot.transform.position;

                    Vector3 _newDirection = Vector3.RotateTowards(turretPivot.transform.forward, _targetDirection * Time.deltaTime, turnSpeed * Random.Range(0.9f, 1f), 0f);

                    turretPivot.transform.rotation = Quaternion.LookRotation(_newDirection);

                    if (turretPivot.transform.forward == _newDirection && !isHeld)
                    {
                        isHeld = true;
                        StartCoroutine(SwapIdlePoint());
                    }
                }
            }
        }
        else
        {
            canSeePlayer = false;
            shootTimer = 0;
        }

        CheckDamage();
    }

    //private void FixedUpdate()
    //{
    //    if (target != null)
    //    {
    //        RaycastHit _hit;

    //        //Debug.DrawRay(turretPivot.transform.position, target.transform.position - turretPivot.transform.position, new Color(255, 0, 0));

    //        //                                                                                                                layermask so turret nerds can shoot player in force fields
    //        if (Physics.Raycast(turretPivot.transform.position, target.transform.position - turretPivot.transform.position, out _hit, Mathf.Infinity, visionMask))
    //        {
    //            //Debug.Log(_hit.collider.gameObject.name);
    //            if (_hit.collider.CompareTag("Player"))
    //            {
    //                canSeePlayer = true;
    //            }
    //            else
    //            {
    //                canSeePlayer = false;
    //            }
    //        }
    //    }
    //}

    private void Shoot()
    {
        GameObject _bullet = Instantiate(basicTurretBulletPrefab, shootPoint.transform.position, shootPoint.transform.rotation);
        Vector3 _velocity = shootPoint.transform.forward.normalized * shotVelocity;
        _bullet.GetComponent<TurretBullet>().SetVelocity(_velocity);
    }

    private void CheckDamage()
    {
        if (isDamaged)
        {
            health -= unappliedDamage;
            unappliedDamage = 0;
            isDamaged = false;

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        DestroyExtraObjects();
        GameObject _hitEffect = Instantiate(turretDeathEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(_hitEffect, turretDeathEffectDuration);
        Destroy(gameObject);
    }

    private void DestroyExtraObjects()
    {
        foreach(GameObject _x in thingsToDestroy)
        {
            Destroy(_x);
        }
    }

    private IEnumerator SwapIdlePoint()
    {
        idleBeeper.Play();

        yield return new WaitForSeconds(idleHoldTime);

        isHeld = false;
        idlePointOne = !idlePointOne;
    }
}
