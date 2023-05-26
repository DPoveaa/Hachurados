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
    public AudioSource pageSFX;
    private float pageTimer = 10;
    #endregion

    #region Soundtrack

    public AudioSource music1;
    public AudioSource music2;

    public static int actualMusic = 1;

    #endregion

    #region

    public GameObject dialogue1;
    public GameObject dialogue2;
    public GameObject dialogue3;
    public GameObject dialogue4;
    public GameObject dialogue5;
    public GameObject dialogue6;
    public GameObject dialogue7;
    public GameObject dialogue8;

    public float dialogueTimer;
    public int actualDialogue;

    private bool finishedDialogue;
    #endregion

    public Light worldLight;
    public Light light2;
    public Light light3;
    public Light light4;
    public Light light5;
    public static bool bossDead;

    public GameObject endText;

    #endregion


    void Start()
    {
        bossDead = false;
        DontDestroyOnLoad(gameObject);
        pauseMenu.SetActive(false);

        dialogueTimer = 2;
    }

    void Update()
    {


        Debug.Log(actualDialogue);
        Debug.Log("timer: " + dialogueTimer);
        if (bossDead)
        {
            worldLight.intensity = worldLight.intensity - 0.10f;
            light2.intensity = light2.intensity - 0.01f;
            light3.intensity = light3.intensity - 0.01f;
            light4.intensity = light4.intensity - 0.01f;
            light5.intensity = light5.intensity - 0.01f;
        }
        if (worldLight.intensity <= 0 && light2.intensity <= 0 || light3.intensity <= 0)
        {
            endText.SetActive(true);
        }

        if (actualDialogue < 9 && dialogueTimer > 0)
        {
            dialogueTimer -= Time.deltaTime;
        }

        if (dialogueTimer <= 0)
        {
                actualDialogue++;
                dialogueTimer = 10f;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            actualDialogue++;
            dialogueTimer = 10f;
        }

        switch (actualDialogue)
        {

            case 1:
                dialogue1.SetActive(true);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                break;

            case 2:
                dialogue1.SetActive(false);
                dialogue2.SetActive(true);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                break;

            case 3:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(true);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                break;

            case 4:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(true);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                break;

            case 5:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(true);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                break;

            case 6:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(true);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                break;

            case 7:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(true);
                dialogue8.SetActive(false);
                break;

            case 8:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(true);
                break;
            default:
                dialogue1.SetActive(false);
                dialogue2.SetActive(false);
                dialogue3.SetActive(false);
                dialogue4.SetActive(false);
                dialogue5.SetActive(false);
                dialogue6.SetActive(false);
                dialogue7.SetActive(false);
                dialogue8.SetActive(false);
                finishedDialogue = true;
                break;
        }

        if (actualMusic == 1)
        {
            music1.enabled = true;
            music2.enabled = false;
        }
        else if (actualMusic == 2)
        {
            music2.enabled = true;
            music1.enabled = false;
        }

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
            Debug.Log("Stoped");
            if (Input.GetKeyDown(KeyCode.Escape) && reading)
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
        pageSFX.Play();
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
