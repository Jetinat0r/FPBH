using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float explosiveForce = 100f;

    [SerializeField]
    private GameObject barrelExplosionPrefab;
    [SerializeField]
    private float barrelExplosionDuration = 2.1f;
    [SerializeField]
    private GameObject grenadeExplosionPrefab;
    [SerializeField]
    private float grenadeExplosionDuration = 2.1f;
    [SerializeField]
    private int enemyDamage = 30;

    private PlayerUI playerUIScript;

    private void Start()
    {
        playerUIScript = FindObjectOfType<PlayerUI>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        //Player is handled on itself
        //Enemies probably will be too

        if (_other.gameObject.CompareTag("Player Weapon") && _other.GetComponent<Rigidbody>() != null)
        {
            _other.GetComponent<Rigidbody>().AddForceAtPosition((_other.transform.position - transform.position).normalized * explosiveForce, _other.ClosestPointOnBounds(gameObject.transform.position), ForceMode.Force);
        }

        if(_other.gameObject.CompareTag("Explosive Environment"))
        {
            GameObject _explosion = Instantiate(barrelExplosionPrefab, _other.gameObject.transform.position, _other.gameObject.transform.rotation);
            Util.SetTagRecursively(_explosion, tag);
            Destroy(_other.gameObject);
            Destroy(_explosion, barrelExplosionDuration);

            //Does work correctly or no?
            if(tag == "Player Explosion" && playerUIScript != null)
            {
                playerUIScript.PlayHitMarker();
            }
        }

        //Grenades
        if(_other.gameObject.CompareTag("Player Projectile"))
        {
            GameObject _explosion = Instantiate(grenadeExplosionPrefab, _other.gameObject.transform.position, _other.gameObject.transform.rotation);
            Util.SetTagRecursively(_explosion, tag);
            Destroy(_other.gameObject);
            Destroy(_explosion, grenadeExplosionDuration);
        }

        //MAKE SURE TO DAMAGE ENEMIES AND THEIR SHOTS
        if(_other.gameObject.CompareTag("Enemy Shot"))
        {
            //Maybe a puff prefab for destroyed bullets?

            Destroy(_other.gameObject);
        }

        //PLAYER EXPLOSIONS DEAL 50 DMG
        if(tag == "Player Explosion" && _other.gameObject.CompareTag("Enemy"))
        {
            //IS ABLE TO HIT ENEMY TWICE OR MORE, DEPENDING ON WHERE THE EXPLOSION IS BC THE ENEMY IS MADE OUT OF MULIPLE PARTS
            _other.GetComponent<UniversalTurretBehaviors>().TakeDamage(enemyDamage);

            //playerUIScript just straight up doesn't exist some times
            //Oh well
            if (playerUIScript != null)
            {
                playerUIScript.PlayHitMarker();
            }
        }

        //ENEMY EXPLOSIONS DEAL 1 DMG
        if(tag == "Enemy Explosion" &&_other.gameObject.CompareTag("Player"))
        {
            _other.GetComponent<Player>().TakeDamage(1);
        }
    }
}
