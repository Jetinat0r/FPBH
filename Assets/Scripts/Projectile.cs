using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //GOING TO BE USED FOR SECONDARY FIRES (GRENADES)
    [SerializeField]
    private GameObject explosionEffectPrefab;
    [SerializeField]
    private float explosionEffectDuration;

    [SerializeField]
    private float throwForce = 100f;
    private Vector3 velocity;
    private string explosionTag;
    private float timeBeforeExplode = 10f;
    private float timer = 0f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= timeBeforeExplode)
        {
            Explode();
        }

        gameObject.transform.position += velocity * throwForce * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider _other)
    {
        string _tag = _other.gameObject.tag;

        //Probably need to add more to this :P
        if (_tag == "Collidable Environment" || _tag == "Player Weapon" || _tag == "Explosive Environment" || _tag == "Enemy")
        {
            Explode();
        }

        if(_tag == "Force Field")
        {
            float _distForceField = Vector3.Distance(_other.transform.position, transform.position);
            float _distNextStep = Vector3.Distance(_other.transform.position, transform.position + velocity);

            //Do stuff with raycast to find dist from center to closest point on force field
            //Vector3 direction center sphere to grenade?
            //Could have a collider inside field to make grenade immune to that specific field, but would require giving field a script
            float _distToCenter = (transform.position - _other.transform.position).magnitude;

            float _radius = _other.GetComponent<SphereCollider>().radius * _other.transform.localScale.x;

            //Compares start pos to pos when it hits the field to determine if it should be destroyed or not
            if((startPos - _other.transform.position).magnitude > _other.GetComponent<SphereCollider>().radius * _other.transform.localScale.x)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        GameObject _hitEffect = Instantiate(explosionEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);
        _hitEffect.tag = explosionTag;
        Destroy(_hitEffect, explosionEffectDuration);
        Destroy(gameObject);
    }

    public void SetVelocity(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void SetTag(string _tag)
    {
        explosionTag = _tag;
    }
}
