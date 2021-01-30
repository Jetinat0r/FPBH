using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private GameObject initialWeapon;

    [SerializeField]
    private float throwForce = 1500f;

    private bool weaponIsAttatched = false;
    private bool isReloading = false;
    private bool isSpinning = false;

    [SerializeField]
    private AudioSource reloadSFX;
    [SerializeField]
    private AudioSource throwWeaponSFX;

    //
    //ADD HANDS WEAPON LATER
    //LIKE, FOR REAL
    //pfft, why would i do that?
    //

    //[SerializeField]
    //private GameObject fightingHands;


    //^^^^
    //^^^^^^^^^
    //^^^^^^^^^^^^^^^^^^^^

    private PlayerWeapon currentWeapon;
    private Transform shootPoint;
    private WeaponComponents currentComponents;

    private GameObject currentWeaponInstance;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _weaponInstance = Instantiate(initialWeapon, weaponHolder.transform.position, weaponHolder.transform.rotation);
        EquipWeapon(_weaponInstance);
    }

    private void Update()
    {
        if (currentWeaponInstance != null && !weaponIsAttatched)
        {
            currentWeaponInstance.transform.position = Vector3.MoveTowards(currentWeaponInstance.transform.position, weaponHolder.position, 15f * Time.deltaTime);
            currentWeaponInstance.transform.rotation = Quaternion.RotateTowards(currentWeaponInstance.transform.rotation, weaponHolder.rotation, 720f * Time.deltaTime);
        }

        //IF: there is a weapon,          it is not yet attatched    it is close enough to the weapon holder,                                                    and it is close enough to facing the correct direction
        if (currentWeaponInstance != null && !weaponIsAttatched && Vector3.Distance(currentWeaponInstance.transform.position, weaponHolder.position) < 0.01f && Quaternion.Angle(currentWeaponInstance.transform.rotation, weaponHolder.rotation) < 0.01f)
        {
            weaponIsAttatched = true;
            gameObject.GetComponent<PlayerShoot>().RemoteSetCanShoot(true);
        }

        if (isReloading && currentWeaponInstance != null)
        {
            float _xStep = 360 / currentWeapon.reloadTime;

            if (!isSpinning)
            {
                StartCoroutine(MakeGunSpin(_xStep));
            }
        }
    }

    #region Getters
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    //For things like muzzle flash and hit effects, might put hit effects under bullet
    public WeaponComponents GetCurrentComponents()
    {
        return currentComponents;
    }

    public Transform GetShootPoint()
    {
        return shootPoint;
    }

    public GameObject GetCurrentWeaponInstance()
    {
        return currentWeaponInstance;
    }
    #endregion

    public void EquipWeapon(GameObject _playerWeapon)
    {
        if(currentWeaponInstance != null)
        {
            ThrowWeapon();
            throwWeaponSFX.Stop();
        }

        reloadSFX.Play();

        //So the new weapon can move into position
        weaponIsAttatched = false;

        gameObject.GetComponent<PlayerShoot>().RemoteSetCanShoot(false);
        
        currentWeaponInstance = _playerWeapon;
        Util.SetLayerRecursively(currentWeaponInstance, LayerMask.NameToLayer("Weapon"));
        currentWeaponInstance.tag = "Untagged";

        currentWeapon = _playerWeapon.GetComponent<WeaponComponents>().playerWeaponInformation.GetWeaponInfo();

        currentWeaponInstance.transform.SetParent(weaponHolder);

        //NEW STUFF
        currentWeaponInstance.transform.localScale = new Vector3(1, 1, 1);

        currentComponents = currentWeaponInstance.GetComponent<WeaponComponents>();
        if (currentComponents == null)
        {
            Debug.LogError("No WeaponGraphics component on the weapon object " + currentWeaponInstance.name);
        }

        currentComponents.WeaponPickedUp();

        shootPoint = currentComponents.shootPoint.transform;

        Util.SetLayerRecursively(currentWeaponInstance, LayerMask.NameToLayer(weaponLayerName));
    }

    //SHOULD EQUIP HANDS
    public void ThrowWeapon()
    {
        if(currentWeaponInstance == null)
        {
            return;
        }

        throwWeaponSFX.Play();

        isReloading = false;
        gameObject.GetComponent<PlayerShoot>().SetIsMidReload(false);
        isSpinning = false;

        Util.SetLayerRecursively(currentWeaponInstance, LayerMask.NameToLayer("Default"));
        currentWeaponInstance.layer = LayerMask.NameToLayer("Default");
        currentWeaponInstance.tag = "Player Weapon";
        currentComponents.WeaponDropped();
        currentWeaponInstance.transform.SetParent(null);

        //NEW STUFF
        currentWeaponInstance.transform.localScale = new Vector3(1, 1, 1);

        Vector3 _throwDirection = weaponHolder.transform.forward.normalized;
        _throwDirection *= throwForce;
        currentComponents.rb.AddForce(_throwDirection, ForceMode.Force);

        currentWeapon = null;
        shootPoint = null;
        currentComponents = null;
        currentWeaponInstance = null;
    }

    public void ReloadSpin()
    {
        gameObject.GetComponent<PlayerShoot>().RemoteSetCanShoot(false);
        gameObject.GetComponent<PlayerShoot>().RemoteStopShoot();
        isReloading = true;

        reloadSFX.Play();
        gameObject.GetComponent<PlayerShoot>().SetIsMidReload(true);
    }

    private IEnumerator MakeGunSpin(float _xStep)
    {
        isSpinning = true;
        float _angleTraveled = 0f;
        while (_angleTraveled < 360f)
        {
            if (!isSpinning)
            {
                yield break;
            }

            currentWeaponInstance.transform.Rotate(_xStep * Time.deltaTime, 0f, 0f);
            _angleTraveled += _xStep * Time.deltaTime;
            yield return null;
        }

        isReloading = false;
        gameObject.GetComponent<PlayerShoot>().SetIsMidReload(false);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
        gameObject.GetComponent<PlayerShoot>().RemoteSetCanShoot(true);
        isSpinning = false;
    }
}
