using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Vars

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

    #region Bones

    public TMP_Text BoneAmount;

    #endregion

    #region Scenes

    public int actualScene;

    #endregion

    #region Pause Menu

    public GameObject pauseMenu;
    public static bool isPaused;

    #endregion

    #region Save and Load

    public GameObject player;

    #endregion

    #region Pages

    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject page5;
    public GameObject page6;
    public GameObject page7;
    public GameObject page8;
    public GameObject page9;

    private bool reading;
    #endregion

    #endregion


    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        #region Stats Bars

        #region Health
        if (healthAmount <= 0)
        {
            //actualScene = SceneManager.GetActiveScene().buildIndex;
            //SceneManager.LoadScene(actualScene);
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

        #region Pause Menu

        if (Input.GetKeyDown(KeyCode.Escape) && !reading)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (reading)
        {
            Time.timeScale = 0f;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopReading();
            }
        }

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

    #region Path Update



    #endregion

    #region Collecting Pages

    public void PageFragment(int fragmentNumber)
    {
        switch (fragmentNumber)
        {
            case 1:
                page1.SetActive(true);
                reading = true;
                break;

            case 2:
                page2.SetActive(true);
                reading = true;
                break;

            case 3:
                page3.SetActive(true);
                reading = true;
                break;

            case 4:
                page4.SetActive(true);
                reading = true;
                break;

            case 5:
                page5.SetActive(true);
                reading = true;
                break;

            case 6:
                page6.SetActive(true);
                reading = true;
                break;

            case 7:
                page7.SetActive(true);
                reading = true;
                break;

            case 8:
                page8.SetActive(true);
                reading = true;
                break;

            case 9:
                page9.SetActive(true);
                reading = true;
                break;

            default:
                return;
                break;

        }
    }

    #endregion

    #region Collecting Bones
    public void BoneCollected(int BoneValue)
    {
        BoneAmount.text = BoneValue.ToString();
    }

    #endregion

    #region Pages

    public void StopReading()
    {
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);
        page6.SetActive(false);
        page7.SetActive(false);
        page8.SetActive(false);
        page9.SetActive(false);
        Time.timeScale = 1f;
        reading = false;
    }


    #endregion

    #region Pause Menu

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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

    #region Save and Load

    
    #endregion
}
