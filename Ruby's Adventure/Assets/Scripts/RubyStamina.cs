using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyStamina : MonoBehaviour
{
    public float normalMoveSpeed = 5f;
    public StaminaBar staminaBar;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // Check if there is enough stamina to move
        if (staminaBar.currentStamina > 0)
        {
            // Consume stamina while moving
            if (movement.magnitude > 0.1f)
            {
                float staminaCost = 5f * Time.deltaTime; // Adjust the stamina cost as needed
                staminaBar.ConsumeStamina(staminaCost);
            }

            // Move at normal speed
            transform.Translate(movement * normalMoveSpeed * Time.deltaTime);
        }
        else
        {
            // Player has no stamina, slow down movement
            float depletedMoveSpeed = 2f; // Adjust the depleted movement speed as needed
            transform.Translate(movement * depletedMoveSpeed * Time.deltaTime);
        }
    }
}
