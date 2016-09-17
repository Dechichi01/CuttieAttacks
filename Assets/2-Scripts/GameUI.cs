using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;
    public GameObject winUI;
    public Text ammoUI;
    public Image weaponImage;
    public void SetWeaponImage(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }
    public Text gameOverScoreUI;
    public Text winScoreUI;
    public RectTransform healthBar;

    private Player player;

	void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
        fadePlane.enabled = false;
        
    }

    void Update()
    {
        ammoUI.text = GameManager.instance.currentWeaponAmmo.ToString("D2");
        float healthPercent = 0;
        if (player != null)
            healthPercent = player.health / player.startingHealth;
        healthBar.localScale = new Vector3(healthPercent, 1, 1);

    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.75f), 1));
        gameOverScoreUI.text = ammoUI.text;
        ammoUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void OnWin()
    {
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .75f), 1));
        winScoreUI.text = ammoUI.text;
        ammoUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        winUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        fadePlane.enabled = true;
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }


}
