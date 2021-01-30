using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    #region Player Movement Settings
    [Header("Player Movement Settings")]

    [SerializeField]
    private float moveSpeed = 12f;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float jumpHeight = 12f;
    [SerializeField]
    private float jumpTimer = 0.9f;
    private bool canStopJump = false;

    [SerializeField]
    private float crouchMoveSpeed = 0.5f;
    [SerializeField]
    private float crouchJumpSpeed = 0.75f;
    private bool isCrouched = false;
    private bool groundedSinceCrouch = false;

    private Vector3 velocity;
    #endregion

    #region Ground Check Settings Variables
    [Header("Ground Check Settings")]

    [SerializeField]
    private Transform groundChecker;
    [SerializeField]
    private float groundDistance = 0.4f;

    private bool isGrounded;
    #endregion

    #region Ceiling Check Settings Variables
    [Header("Ceiling Check Settings")]

    [SerializeField]
    private Transform ceilingChecker;
    [SerializeField]
    private float ceilingDistance = -0.4f;


    private bool isCeilinged;
    #endregion

    #region Environment Check Settings
    [Header("Environment Check Settings")]

    [SerializeField]
    private LayerMask collidableEnvironmentMask;
    [SerializeField]
    private LayerMask enemyMask;
    #endregion

    #region Animation Settings
    [Header("Animation Settings")]

    [SerializeField]
    private Animator playerAnimator;
    #endregion

    #region Explosion Push Settings
    [Header("Explosion Push Settings")]

    [SerializeField]
    private float explosionPushForce = 20f;
    [SerializeField]
    private float explosionCancelForce = 1.95f;
    [SerializeField]
    private float blastedDuration = 0.15f;

    private bool isInExplosion = false;
    private bool groundCanCancelExplosion = false;
    private float timeSinceExploded = 0f;
    private Vector3 explosionForceVector = Vector3.zero;
    private Vector3 explosionCancelVector = Vector3.zero;
    private bool isContingencyActive = false;
    #endregion

    #region Push Physics Objects
    [SerializeField]
    private float bonkForce = 250f;
    private Vector3 lastMove;
    #endregion

    #region SFX
    [Header("SFX")]

    [SerializeField]
    private AudioSource jumpSFX;
    [SerializeField]
    private AudioSource bonkSFX;

    [SerializeField]
    private float bonkSpeed = -10f;
    #endregion

    void Update()
    {
        #region Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouched = !isCrouched;
            if (isCrouched)
            {
                playerAnimator.Play("Crouch");
                if (isGrounded)
                {
                    groundedSinceCrouch = true;
                }
            }
            else
            {
                playerAnimator.Play("Uncrouch");
            }
        }
        #endregion

        #region Moving
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, collidableEnvironmentMask);
        if (!isGrounded)
        {
            isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, enemyMask);
        }

        if (!isGrounded)
        {
            groundCanCancelExplosion = true;
            isCeilinged = Physics.CheckSphere(ceilingChecker.position, ceilingDistance, collidableEnvironmentMask);
            if (!isCeilinged)
            {
                isCeilinged = Physics.CheckSphere(ceilingChecker.position, ceilingDistance, enemyMask);
            }

            if (isCeilinged && (velocity.y > 0 || explosionForceVector.y > 0))
            {
                explosionForceVector.y = 0;
                explosionCancelVector.y = 0;
                velocity.y = 0f;
                bonkSFX.Play();
            }
        }

        if (isGrounded && velocity.y < 0)
        {
            if (isCrouched)
            {
                groundedSinceCrouch = true;
            }
            else
            {
                groundedSinceCrouch = false;
            }

            if(velocity.y < bonkSpeed)
            {
                bonkSFX.Play();
            }

            canStopJump = false;
            velocity.y = -2f;
        }

        float _x = Input.GetAxis("Horizontal");
        float _z = Input.GetAxis("Vertical");

        Vector3 _move = transform.right * _x + transform.forward * _z;

        if (groundedSinceCrouch)
        {
            _move *= crouchMoveSpeed;
        }
        #endregion

        #region Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpSFX.Play();

            canStopJump = true;

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (isCrouched)
            {
                velocity.y *= crouchJumpSpeed;
            }

            StartCoroutine(EndJump());
        }

        if(Input.GetButtonUp("Jump") && canStopJump)
        {
            velocity.y *= (2f/3f);
            canStopJump = false;
        }

        velocity.y += gravity * Time.deltaTime;
        #endregion

        #region Exploding
        //Timer for last time in an explosion
        if (!groundCanCancelExplosion && !isInExplosion)
        {
            timeSinceExploded += Time.deltaTime;
            if(timeSinceExploded > blastedDuration)
            {
                groundCanCancelExplosion = true;
            }
        }

        if (explosionForceVector != Vector3.zero && groundCanCancelExplosion && isGrounded)
        {
            explosionForceVector.y = 0;
            explosionForceVector.x *= (999 / 1000) * Time.deltaTime;
            explosionForceVector.z *= (999 / 1000) * Time.deltaTime;
        }

        if (Mathf.Abs(_move.x) > Mathf.Abs(explosionForceVector.x))
        {
            explosionForceVector.x = 0f;
        }

        if (Mathf.Abs(_move.z) > Mathf.Abs(explosionForceVector.z))
        {
            explosionForceVector.z = 0f;
        }

        //Applies explosion force if necessary
        if (explosionForceVector != Vector3.zero && !isInExplosion)
        {
            explosionForceVector += explosionCancelVector * Time.deltaTime;
        }
        #endregion

        //Caps fall speed
        if(velocity.y < -30f)
        {
            velocity.y = -30f;
        }
        
        //Applies final movement for the player
        lastMove = (_move * moveSpeed * Time.deltaTime) + (velocity * Time.deltaTime) + (explosionForceVector * Time.deltaTime);
        controller.Move(lastMove);
    }

    private IEnumerator EndJump()
    {
        yield return new WaitForSeconds(jumpTimer);

        canStopJump = false;
    }

    private void ExplosionForce(Vector3 _direction, Vector3 _cancelDirection)
    {
        explosionForceVector = _direction;
        explosionCancelVector = _cancelDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        isInExplosion = true;

        

        if(other.gameObject.CompareTag("Player Explosion") || other.gameObject.CompareTag("Enemy Explosion"))
        {
            if (isContingencyActive)
            {
                StopCoroutine(ContingencyExplosionExit());
            }

            velocity.y = 0;

            //Calculates angle between collision point and the player
            Vector3 _direction = other.transform.position - transform.position;

            //Gets the opposite Vector3 and normalizes it
            _direction = -_direction.normalized;
            Vector3 _cancelDirection = -_direction * explosionCancelForce;

            _direction *= explosionPushForce;

            //Sends the force to be applied
            ExplosionForce(_direction, _cancelDirection);

            //Ensuring timer is correct
            timeSinceExploded = 0f;

            groundCanCancelExplosion = false;

            if(other.gameObject.transform.parent.CompareTag("Enemy Explosion"))
            {
                gameObject.GetComponent<Player>().TakeDamage(1);
            }

            StartCoroutine(ContingencyExplosionExit());
            isContingencyActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player Explosion") || other.gameObject.CompareTag("Enemy Explosion"))
        {
            isInExplosion = false;

            //Ensuring timer is correct
            timeSinceExploded = 0f;
        }
    }

    private IEnumerator ContingencyExplosionExit()
    {
        yield return new WaitForSeconds(blastedDuration / 4);

        isContingencyActive = false;
        if (isInExplosion)
        {
            isInExplosion = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit _hit)
    {
        if (_hit.collider.CompareTag("Explosive Environment"))
        {
            _hit.collider.attachedRigidbody.AddForce(lastMove * bonkForce);
        }

        if (_hit.collider.CompareTag("Player Weapon"))
        {
            _hit.collider.attachedRigidbody.AddForce(lastMove * bonkForce);
        }

        if (_hit.collider.CompareTag("Goal"))
        {
            GetComponent<Player>().Win();
        }
    }
}