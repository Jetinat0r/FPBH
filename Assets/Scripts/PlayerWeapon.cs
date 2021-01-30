using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    //WILL TELL GAME WHAT WEAPON PLAYER HAS
    public string name = "Glock ;)";

    public int damage = 10;
    public float range = 100f;
    public float lazerForce = 5f;

    public float fireRate = 0f;
    public float reloadTime = 2.5f;

    public int ammo = 9;

    public GameObject weaponGraphics;

    public PlayerWeapon GetWeaponInfo()
    {
        return this;
    }
}
