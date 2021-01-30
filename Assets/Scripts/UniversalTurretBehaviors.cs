using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalTurretBehaviors : MonoBehaviour
{
    //Applied to each category of turret

    [Header("Universal Turret Variables")]
    public GameObject target;
    public int unappliedDamage = 0;
    public bool isDamaged = false;

    [Header("Layermask Variables")]
    [SerializeField]
    private string environmentLayer = "Collidable Environment";
    [SerializeField]
    private string playerLayer = "Player";
    private int mask;

    public void SetTarget(GameObject _target)
    {
        target = _target;
        //Debug.Log(_target);
    }

    public void TakeDamage(int _amount)
    {
        isDamaged = true;
        unappliedDamage += _amount;
    }

    public int CreateVisionMask()
    {
        int _evLM = 1 << LayerMask.NameToLayer(environmentLayer);
        int _plLM = 1 << LayerMask.NameToLayer(playerLayer);
        mask = _evLM | _plLM;

        return mask;
    }

    public bool GetHasTarget()
    {
        if(target == null)
        {
            return false;
        }

        return true;
    }
}
