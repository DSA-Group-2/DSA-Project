using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    private float health;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;

    [Header("Damage Overlay")]

    //public Image overlay; //our DamageOverlay GameObject

    public Volume globalVolume;
    private Vignette vignette;
    public float duration; //How long the image stays fully opaque
    public float fadespeed; // How quickly the image will fade.
    
    private float durationTimer; //Timer to check against the duration.



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        //overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out vignette);

            if (vignette != null)
            {
                vignette.intensity.value = 0f;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        /*if (overlay.color.a > 0)
        {
            if (health <30)
            {
                return;
            }

            durationTimer += Time.deltaTime;
            if(durationTimer > duration)
            {
                //fade image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadespeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }*/

        if (vignette != null)
        {
            if (health < 30)
            {
                vignette.intensity.value =0.6f;
                durationTimer = 0f;
                return;
            }

            durationTimer += Time.deltaTime;

            if (durationTimer > duration)
            {
                //fade image
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0f, Time.deltaTime * fadespeed);
            }
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }


        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }

        healthText.text = Mathf.Round(health) + "/" + Mathf.Round(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        //overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);

        if (vignette != null)
        {
            vignette.intensity.value = 0.75f;
        }

    }

    public void RestoreHealth(float healthAmount)
    {
        health += healthAmount;
        lerpTimer = 0f;
    }


    public void IncreaseHealth(int level)
    {
        maxHealth += (health * 0.01f) * ((100-level) * 0.1f);
        health = maxHealth;
    }
}
