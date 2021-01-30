using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject playerUI;
    [SerializeField]
    private PlayerUI playerUIScript;
    [SerializeField]
    private GameObject victoryScreen;
    [SerializeField]
    private GameObject deathScreen;
    
    [SerializeField]
    public int currentHealth = 6;
    [SerializeField]
    public int maxHealth = 6;

    [SerializeField]
    private float invincibilityFrames = 0.3f;
    private bool isInvincible = false;

    [SerializeField]
    private float maxWeaponPickupDistance = 3f;
    private bool isGrabbyHand = false;

    [SerializeField]
    private int grenadesOnPickup = 5;

    [SerializeField]
    private AudioSource grenadePickupSFX;
    [SerializeField]
    private AudioSource victorySFX;

    //RE-USES "Bullet Hit"
    [SerializeField]
    private AudioSource defeatSFX;

    public bool victoryAchieved = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        playerUI.SetActive(true);
        victoryScreen.SetActive(false);
        deathScreen.SetActive(false);
        playerUIScript.UpdateMaxHealth(maxHealth, currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug kill
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    TakeDamage(1);
        //}

        //Debug increase & decrease maxHealth
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    maxHealth += 2;
        //    currentHealth += 2;
        //    playerUIScript.UpdateMaxHealth(maxHealth, currentHealth);
        //}
        //if (Input.GetKeyDown(KeyCode.L) && maxHealth > 2)
        //{
        //    maxHealth -= 2;
        //    if(currentHealth > maxHealth)
        //    {
        //        currentHealth = maxHealth;
        //    }
        //    playerUIScript.UpdateMaxHealth(maxHealth, currentHealth);
        //}

        RaycastHit _hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, maxWeaponPickupDistance))
        {
            if(_hit.collider.gameObject.CompareTag("Player Weapon"))
            {
                if (!isGrabbyHand)
                {
                    //If the grabby hand isn't enabled, enable it
                    playerUIScript.crosshair.enabled = false;
                    playerUIScript.grabbyHand.enabled = true;

                    isGrabbyHand = true;
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Set gun
                    gameObject.GetComponent<WeaponManager>().EquipWeapon(_hit.collider.gameObject);

                    //Set UI
                    playerUIScript.SetAmmoActive(true);

                    //Set player shoot
                    int _ammo = gameObject.GetComponent<WeaponManager>().GetCurrentWeapon().ammo;
                    gameObject.GetComponent<PlayerShoot>().SetMaxAmmo(_ammo);

                    //Stop player shooting
                    gameObject.GetComponent<PlayerShoot>().RemoteStopShoot();
                }
            }
            else if(isGrabbyHand)
            {
                //If the grabby hand is enabled, disable it
                //(Both of these are needed)
                //(This one bc if the ray hits something that isn't a gun it won't play the other)
                playerUIScript.crosshair.enabled = true;
                playerUIScript.grabbyHand.enabled = false;

                isGrabbyHand = false;
            }
        }
        else if (isGrabbyHand)
        {
            //If the grabby hand is enabled, disable it
            //(Both of these are needed)
            //(This one bc if the ray doesn't hit anything it won't play the other)
            playerUIScript.crosshair.enabled = true;
            playerUIScript.grabbyHand.enabled = false;

            isGrabbyHand = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.GetComponent<WeaponManager>().ThrowWeapon();
            playerUIScript.SetAmmoActive(false);

            //May solve an error, is good to have anyway
            gameObject.GetComponent<PlayerShoot>().RemoteStopShoot();
        }
    }

    public void ToggleIsPaused()
    {
        if (!victoryAchieved)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            playerUI.SetActive(!playerUI.activeSelf);
            GetComponent<PlayerShoot>().RemoteStopShoot();
        }
    }

    public void TakeDamage(int _amount)
    {
        if (!isInvincible)
        {
            currentHealth -= _amount;

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            playerUIScript.UpdateHealth(currentHealth);

            if (currentHealth == 0)
            {
                Die();

                isInvincible = true;
                return;
            }

            isInvincible = true;
            StartCoroutine(EndIFrames());
        }
    }

    private IEnumerator EndIFrames()
    {
        yield return new WaitForSeconds(invincibilityFrames);

        isInvincible = false;
    }

    private void Die()
    {
        if (!victoryAchieved)
        {
            Time.timeScale = 0;

            pauseMenu.SetActive(false);
            playerUI.SetActive(false);
            deathScreen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;

            FindObjectOfType<GameManager>().isPaused = true;

            defeatSFX.Play();
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if(_other.gameObject.CompareTag("Grenade Pickup"))
        {
            grenadePickupSFX.Play();
            gameObject.GetComponent<PlayerShoot>().PickupGrenade(grenadesOnPickup);
            Destroy(_other.gameObject);
        }
    }

    public void Win()
    {
        if (!victoryAchieved)
        {
            Time.timeScale = 0;
            victoryAchieved = true;

            pauseMenu.SetActive(false);
            playerUI.SetActive(false);
            deathScreen.SetActive(false);
            victoryScreen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;

            FindObjectOfType<GameManager>().isPaused = true;

            victorySFX.Play();
        }
    }
}
