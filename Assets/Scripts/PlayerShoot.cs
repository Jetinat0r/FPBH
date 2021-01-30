using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private string collidableEnvironmentTag = "Collidable Environment";
    [SerializeField]
    private string enemyTag = "Enemy";

    //[SerializeField]
    //private Player playerScript;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private PlayerUI playerUIScript;

    //Change if u wanna use any weapon other than pistol as start weapon :P
    [HideInInspector]
    public int currentAmmo = 9;
    [HideInInspector]
    public int maxAmmo = 9;

    [SerializeField]
    private GameObject barrelExplosionPrefab;
    [SerializeField]
    private float barrelExplosionDuration;

    [SerializeField]
    private GameObject grenadePrefab;
    private int grenadeCount = 0;
    [SerializeField]
    private float grenadeThrowForce = 2.5f;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private bool canShoot = true;
    private bool isMidReload = false;

    private GameManager gameManager;

    private int shootMask;

    //RE-USES "Weapon Throw"
    [SerializeField]
    private AudioSource grenadeThrowSFX;

    // Start is called before the first frame update
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        gameManager = FindObjectOfType<GameManager>();

        int _plLM = 1 << LayerMask.NameToLayer("Player");
        int _ncELM = 1 << LayerMask.NameToLayer("NonCollidableEnvironment");
        int _igRLM = 1 << LayerMask.NameToLayer("Ignore Raycast");
        shootMask = _plLM | _ncELM | _igRLM;
        shootMask = ~shootMask;
    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        //                           WILL CHANGE WHEN YOU ARE ABLE TO CATCH THESE HANDS
        if (!gameManager.isPaused && weaponManager.GetCurrentWeaponInstance() != null)
        {
            if(Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
            {
                weaponManager.ReloadSpin();

                currentAmmo = maxAmmo;
                playerUIScript.SetCurrentAmmo(currentAmmo.ToString());
            }

            if (currentWeapon.fireRate <= 0f && currentAmmo > 0 && canShoot)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && canShoot && currentAmmo > 0)
                {
                    InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
                }
                else if (Input.GetButtonUp("Fire1") || currentAmmo <= 0 && canShoot)
                {
                    CancelInvoke("Shoot");
                    canShoot = false;
                    StartCoroutine(CanShootAgain(currentWeapon.fireRate));
                }
            }
        }

        if (!gameManager.isPaused)
        {
            if (Input.GetButtonDown("Fire2") && grenadeCount > 0)
            {
                LobGrenade();
            }
        }
    }

    private void Shoot()
    {
        currentAmmo--;
        playerUIScript.SetCurrentAmmo(currentAmmo.ToString());

        //Ray _direction;
        RaycastHit _hit;

        WeaponComponents _currentComponents = weaponManager.GetCurrentComponents();
        Transform _shootPoint = weaponManager.GetShootPoint();

        //Ray shot out of player bc gun does not point at the crosshair
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, weaponManager.GetCurrentWeapon().range, shootMask)){
            //Makes a game object where the raycast hits
            //                  MIGHT BE DANGEROUS \/
            GameObject _hitPoint = Instantiate(gameObject, _hit.point, Quaternion.LookRotation(_hit.normal));

            //Makes a particle effect on the barrel of the gun
            GameObject _shotEffect = Instantiate(_currentComponents.shotEffectPrefab, _shootPoint.position, _shootPoint.rotation);

            //Makes a laser beam
            GameObject _lazerBeam = Instantiate(_currentComponents.lazerPrefab, _shootPoint.position, _shootPoint.rotation);

            //Makes lazer go between gun shoot point and the object the ray hit
            _lazerBeam.GetComponent<LineRenderer>().SetPosition(0, _shootPoint.position);
            _lazerBeam.GetComponent<LineRenderer>().SetPosition(1, _hit.point);

            //Makes a particle effect at the hit point
            GameObject _hitEffect = Instantiate(_currentComponents.lazerHitPrefab, _hitPoint.transform.position, _hitPoint.transform.rotation);

            //Destroys relevent objects
            Destroy(_lazerBeam, _currentComponents.lazerDuration);
            Destroy(_hitEffect, _currentComponents.hitEffectDuration);
            Destroy(_shotEffect, _currentComponents.shotEffectDuration);
            Destroy(_hitPoint);

            //MAKE LAZERS INTERACT WITH THINGS
            if(_hit.collider.CompareTag("Player Weapon"))
            {
                _hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(-_hit.normal * currentWeapon.lazerForce, _hit.point, ForceMode.Force);
            }
            else if(_hit.collider.CompareTag("Explosive Environment"))
            {
                GameObject _explosion = Instantiate(barrelExplosionPrefab, _hit.collider.gameObject.transform.position, _hit.collider.gameObject.transform.rotation);
                Util.SetTagRecursively(_explosion, "Player Explosion");
                Destroy(_hit.collider.gameObject);
                Destroy(_explosion, barrelExplosionDuration);
                playerUIScript.PlayHitMarker();
            }else if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<UniversalTurretBehaviors>().TakeDamage(currentWeapon.damage);
                playerUIScript.PlayHitMarker();
            }
        }
        else
        {
            //Makes a particle effect on the barrel of the gun
            GameObject _shotEffect = Instantiate(_currentComponents.shotEffectPrefab, _shootPoint.position, _shootPoint.rotation);

            //Makes a laser beam
            GameObject _lazerBeam = Instantiate(_currentComponents.lazerPrefab, _shootPoint.position, _shootPoint.rotation);

            //Makes lazer go between gun shoot point and the object the ray hit
            _lazerBeam.GetComponent<LineRenderer>().SetPosition(0, _shootPoint.position);
            _lazerBeam.GetComponent<LineRenderer>().SetPosition(1, playerCamera.transform.position + (playerCamera.transform.forward.normalized * currentWeapon.range));

            Destroy(_lazerBeam, _currentComponents.lazerDuration);
            Destroy(_shotEffect, _currentComponents.shotEffectDuration);
        }
    }

    private void LobGrenade()
    {
        grenadeThrowSFX.Play();

        grenadeCount--;
        
        playerUIScript.SetGrenadeCount(grenadeCount);

        GameObject _grenade = Instantiate(grenadePrefab, playerCamera.transform.position, playerCamera.transform.rotation);
        _grenade.GetComponent<Projectile>().SetTag("Player Explosion");

        Vector3 _direction = playerCamera.transform.forward.normalized;
        _direction *= grenadeThrowForce;
        _grenade.GetComponent<Projectile>().SetVelocity(_direction);
    }

    public void SetMaxAmmo(int _amount)
    {
        maxAmmo = _amount;
        currentAmmo = _amount;

        playerUIScript.SetMaxAmmo(_amount.ToString());
    }

    public void RemoteStopShoot()
    {
        CancelInvoke("Shoot");
    }

    public void RemoteSetCanShoot(bool _bool)
    {
        canShoot = _bool;
    }

    private IEnumerator CanShootAgain(float _fireRate)
    {
        //Somehow messes with auto-clickers without messing with normal gameplay, good job!
        yield return new WaitForSeconds(0.25f / _fireRate);

        if (!isMidReload)
        {
            canShoot = true;
        }
    }

    public void SetIsMidReload(bool _bool)
    {
        isMidReload = _bool;
    }

    public void PickupGrenade(int _amount)
    {
        grenadeCount += _amount;
        playerUIScript.SetGrenadeCount(grenadeCount);
    }
}
