using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletExplosionEffectPrefab;
    [SerializeField]
    private float bulletExplosionEffectDuration;
    [SerializeField]
    private GameObject barrelExplosionEffectPrefab;
    [SerializeField]
    private float barrelExplosionEffectDuration;
    [SerializeField]
    private float explosiveForce = 100f;
    [SerializeField]
    private float timeBeforeExplode = 10f;

    private Vector3 velocity;
    private float timer = 0f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBeforeExplode)
        {
            Explode();
        }

        gameObject.transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider _other)
    {
        string _tag = _other.gameObject.tag;

        //Probably need to add more to this :P
        if (_tag == "Collidable Environment")
        {
            Explode();
        }

        if (_other.gameObject.CompareTag("Player Weapon") && _other.GetComponent<Rigidbody>() != null)
        {
            _other.GetComponent<Rigidbody>().AddForceAtPosition((_other.transform.position - transform.position).normalized * explosiveForce, _other.ClosestPointOnBounds(gameObject.transform.position), ForceMode.Force);
            Explode();
        }

        if (_tag == "Explosive Environment")
        {
            GameObject _explosion = Instantiate(barrelExplosionEffectPrefab, _other.gameObject.transform.position, _other.gameObject.transform.rotation);
            Util.SetTagRecursively(_explosion, "Enemy Explosion");
            Destroy(_other.gameObject);
            Destroy(_explosion, barrelExplosionEffectDuration);
        }

        if (_tag == "Force Field")
        {
            float _distForceField = Vector3.Distance(_other.transform.position, transform.position);
            float _distNextStep = Vector3.Distance(_other.transform.position, transform.position + velocity);

            if ((startPos - _other.transform.position).magnitude > _other.GetComponent<SphereCollider>().radius * _other.transform.localScale.x)
            {
                Explode();
            }
        }

        if(_tag == "Player"){
            _other.GetComponent<Player>().TakeDamage(1);
            Explode();
        }

        if(_tag == "Player Projectile")
        {
            _other.GetComponent<Projectile>().Explode();
            Explode();
        }
    }

    private void Explode()
    {
        GameObject _hitEffect = Instantiate(bulletExplosionEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(_hitEffect, bulletExplosionEffectDuration);
        Destroy(gameObject);
    }

    public void SetVelocity(Vector3 _velocity)
    {
        velocity = _velocity;
    }
}
