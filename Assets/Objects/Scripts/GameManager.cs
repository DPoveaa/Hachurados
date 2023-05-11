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

    #region Scenes

    public int actualScene;

    #endregion
    void Start()
    {

    }

    void Update()
    {
        #region Stats Bars

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

        #endregion
    }

    #region Stats Bars Mechanics
    #region Health Mechanics
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
    #endregion

    #region Collecting Pages

    public void PageFragment(float fragmentNumber)
    {

    }

    #endregion

    #region Collecting Bones
    public void BoneCollected(float BoneValue)
    {

    }

    #endregion

    #region Main Menu
    public void PlayGame()
    {
        SceneManager.LoadScene(1); //Work with "World 1" and SceneManager.GetActiveScene().buildIndex + 1
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
