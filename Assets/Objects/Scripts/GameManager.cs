using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Health
    [Header("HealthBar")]
    public Image healthBar;
    public float healthAmount = 100f;
    #endregion

    #region Stamina
    [Header("StaminaBar")]
    public Image staminaBar;
    public float staminaAmount = 100f;
    #endregion

    public int actualScene;

    void Start()
    {

    }

    void Update()
    {
        #region Health
        if (healthAmount <= 0)
        {
            actualScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(actualScene);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Heal(5);
        }
        #endregion

        #region Stamina
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TakeStamina(20);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Rest(5);
        }
        #endregion
    }

    #region Health Mechanic
    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }
    #endregion

    #region Stamina Mecanic
    public void TakeStamina(float consumedStamina)
    {
        staminaAmount -= consumedStamina;
        staminaBar.fillAmount = staminaAmount / 100f;
    }

    public void Rest(float staminaRecovered)
    {
        staminaAmount += staminaRecovered;
        staminaAmount = Mathf.Clamp(staminaAmount, 0, 100);

        staminaBar.fillAmount = staminaAmount / 100f;
    }

    #endregion
}
