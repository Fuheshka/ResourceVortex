using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject inGameMenuUI;

    [Header("Player Controller")]
    public FirstPersonController playerController;

    private bool isMenuActive = false;

    void Start()
    {
        if (inGameMenuUI != null)
        {
            inGameMenuUI.SetActive(false);
        }

        if (playerController == null)
        {
            playerController = FindObjectOfType<FirstPersonController>();
        }

        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            if (isMenuActive)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (inGameMenuUI != null)
        {
            inGameMenuUI.SetActive(false);
        }

        if (playerController != null)
        {
            playerController.playerCanMove = true;
            playerController.cameraCanMove = true;  // Enable camera movement on resume
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // Hide cursor on resume

        // Additional fix: force cursor lock and hide after a short delay
        StartCoroutine(ResetCursor());

        isMenuActive = false;
    }

    private System.Collections.IEnumerator ResetCursor()
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        if (inGameMenuUI != null)
        {
            inGameMenuUI.SetActive(true);
        }

        if (playerController != null)
        {
            playerController.playerCanMove = false;
            playerController.cameraCanMove = false;  // Disable camera movement on pause
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isMenuActive = true;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
