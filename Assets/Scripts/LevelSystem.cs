using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityPipeline.Microsoft.CodeAnalysis.FlowAnalysis;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public int level;
    public float currentXp;
    public float requiredXp;

    private float lerpTimer; //Interpolador de movimiento, ya que mide le tiempo transcurrido durante el movimiento de la barra delantera. 
    private float delayTimer; //Cuenta la cantidad de segundos desde que ganas experincia hasta que la barra delantera empieza a moverse. 

    [Header("UI")]
    public Image frontXPBar;
    public Image backXPBar;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    [Header("Multipliers")]
    [Range(1f, 300f)]
    public float additionMultiplier = 300;

    [Range(2f, 4f)]
    public float powerMultiplier = 2;

    [Range(7f, 14f)]
    public float divisionMultiplier = 7;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frontXPBar.fillAmount = currentXp / requiredXp;
        backXPBar.fillAmount = currentXp / requiredXp;
        requiredXp = CalculateRequiredXp();
        levelText.text = "Level: " + level;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateXpUI();
        if(Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            //Debug.Log("¡Espacio pulsado, sumando XP!");
            GainExperienceFlatRate(20);
        }

        if (currentXp >= requiredXp)
        {
            LevelUp();
        }

    }

    public void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;
        float FXP = frontXPBar.fillAmount;

        if (FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            backXPBar.fillAmount = xpFraction;

            if (delayTimer > 3)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 4;
                percentComplete = percentComplete * percentComplete;
                frontXPBar.fillAmount = Mathf.Lerp(FXP, backXPBar.fillAmount, percentComplete);
            }
        }

        xpText.text = currentXp + "/" + requiredXp;

    }

    public void GainExperienceFlatRate(float xpGained)
    {
        currentXp += xpGained;
        lerpTimer = 0f;

    }

    public void GainExperienceScalable(float xpGained, int passedLevel) //passed level is the level of the enemy or quest. 
    {
        if (passedLevel < level) // A mayor diferencia de niveles, mayor será el multiplicador aplicado a la experiencia base.
        {
            float multiplier = 1 + (level - passedLevel) * 0.1f;
            currentXp += xpGained * multiplier;
        }

        else
        {
            currentXp += xpGained;
        }

        lerpTimer = 0f;
        delayTimer = 0f; 

    }



    public void LevelUp()
    {
        level++;
        frontXPBar.fillAmount = 0;
        backXPBar.fillAmount = 0;
        currentXp = Mathf.RoundToInt(currentXp - requiredXp);
        GetComponent<PlayerHealth>().IncreaseHealth(level);
        requiredXp = CalculateRequiredXp();

        levelText.text = "Level: " + level;

    }

    private int CalculateRequiredXp() 
    {
        int solveForRequieredXp = 0;

        for (int levelCycle = 1; levelCycle <= level; levelCycle++)
        {

            solveForRequieredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle/divisionMultiplier)); //Si 10.9 retorna 10
        }

        return solveForRequieredXp / 4;
    }



}
