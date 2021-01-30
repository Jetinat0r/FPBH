using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    //private Player player;
    [Header("Hit Effects")]

    [SerializeField]
    private Image bloodSplotch;
    [SerializeField]
    private Image hitMarker;

    [SerializeField]
    private AudioSource hitmarkerSFX;

    [Header("Crosshair")]

    [SerializeField]
    public Image crosshair;
    [SerializeField]
    public Image grabbyHand;

    [Header("Health")]

    [SerializeField]
    private GameObject healthParentPanel;
    [SerializeField]
    private Sprite emptyHeartSprite;
    [SerializeField]
    private Sprite halfHeartSprite;
    [SerializeField]
    private Sprite fullHeartSprite;
    private List<GameObject> heartSprites;

    [Header("Ammo")]

    [SerializeField]
    private GameObject ammoIcon;
    [SerializeField]
    private Text currentAmmoCount;
    [SerializeField]
    private Text maxAmmoCount;

    [Header("Grenades")]
    [SerializeField]
    private Text currentGrenadeCount;

    private Animator screenBloodFX;
    private Animator hitMarkerFX;

    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
        screenBloodFX = bloodSplotch.GetComponent<Animator>();
        hitMarkerFX = hitMarker.GetComponent<Animator>();

        grabbyHand.enabled = false;
    }

    #region Health
    public void UpdateHealth(int _currentHealth)
    {
        //Makes Hearts go WOO!!!
        SetHeartHealth(_currentHealth);

        //Makes screen bloody correctly yay!!!
        screenBloodFX.StopPlayback();
        screenBloodFX.Play("BloodyScreenFX");
    }

    //Creates a list of hearts
    public void UpdateMaxHealth(int _amount, int _currentHealth)
    {
        if (heartSprites != null)
        {
            foreach (GameObject _heartSprite in heartSprites)
            {
                Destroy(_heartSprite);
                //_heartSprite.GetComponent<Image>().sprite = null;
            }
        }

        _amount /= 2;
        float _heartSpacer = (healthParentPanel.GetComponent<RectTransform>().sizeDelta.x / 2) + (healthParentPanel.GetComponent<RectTransform>().sizeDelta.x / 10);
        int _heartsPlaced = 0;

        heartSprites = new List<GameObject>();

        for (int i = _amount; i > 0; i--)
        {
            //Make a heart
            GameObject _emptyHeart = new GameObject();
            Image _heartImage = _emptyHeart.AddComponent<Image>();
            _heartImage.sprite = emptyHeartSprite;

            //Place a heart
            RectTransform _emptyHeartTransform = _emptyHeart.GetComponent<RectTransform>();
            _emptyHeartTransform.SetParent(healthParentPanel.transform);
            _emptyHeartTransform.sizeDelta /= 2;
            _emptyHeartTransform.position = healthParentPanel.GetComponent<RectTransform>().position + new Vector3(_heartSpacer * _heartsPlaced, 0f, 0f);

            heartSprites.Add(_emptyHeart);
            _heartsPlaced++;
        }

        SetHeartHealth(_currentHealth);
    }

    //Tells the list of hearts how to display themselves
    private void SetHeartHealth(int _amount)
    {
        for (int i = 0; i < heartSprites.Count; i++)
        {
            Image _heartSprite = heartSprites[i].GetComponent<Image>();

            if (_amount >= 2)
            {
                _heartSprite.sprite = fullHeartSprite;
            }
            else
            {
                if (_amount % 2 == 1)
                {
                    _heartSprite.sprite = halfHeartSprite;
                }
                else
                {
                    _heartSprite.sprite = emptyHeartSprite;
                }
            }

            _amount -= 2;
        }
    }
    #endregion

    public void PlayHitMarker()
    {
        hitMarkerFX.StopPlayback();
        hitMarkerFX.Play("Hit Marker Start");
        hitmarkerSFX.Play();
    }

    #region Ammo

    //_amount is turned into a string before called
    public void SetMaxAmmo(string _amount)
    {
        maxAmmoCount.text = _amount;
        currentAmmoCount.text = _amount;
    }

    public void SetCurrentAmmo(string _amount)
    {
        currentAmmoCount.text = _amount;
    }

    public void SetAmmoActive(bool _bool)
    {
        ammoIcon.SetActive(_bool);
    }
    #endregion
    
    public void SetGrenadeCount(int _amount)
    {
        currentGrenadeCount.text = _amount.ToString();
    }
}
