using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image staminaBarFill;
    public Text staminaText;
    
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;

    void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaBar();
    }

    void Update()
    {
        // Example: Simulate stamina regeneration over time
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            UpdateStaminaBar();
        }
    }

    public void ConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            UpdateStaminaBar();
        }
        else
        {
            Debug.Log("Not enough stamina!");
        }
    }

    void UpdateStaminaBar()
    {
        float fillAmount = currentStamina / maxStamina;
        staminaBarFill.fillAmount = fillAmount;

        // Update the Text UI element with the current stamina value
        if (staminaText != null)
        {
            staminaText.text = "Stamina: " + currentStamina.ToString("F0");
        }
    }
}
