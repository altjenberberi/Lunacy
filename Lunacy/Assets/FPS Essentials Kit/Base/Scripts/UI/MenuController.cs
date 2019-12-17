/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_HUDCanvas;

    [SerializeField]
    private GameObject m_PauseCanvas;

    [SerializeField]
    private GameObject m_DeathScreenCanvas;

    [SerializeField]
    private GameObject m_EventSystem;

    [SerializeField]
    private BlackScreen m_PauseBlackScreen;

    [SerializeField]
    private BlackScreen m_DeathBlackScreen;

    private bool m_Restarting;

    private void Start ()
    {
        Resume();
    }
	
	private void Update ()
    {
        if (GameplayManager.Instance.IsDead && !m_Restarting)
        {
            DeathScreen();
        }
        else
        {
            if (InputManager.GetButtonDown("Pause"))
            {
                Pause();
            }
        }    
    }

    public void Resume ()
    {
        Time.timeScale = 1;
        m_HUDCanvas.SetActive(true);
        m_PauseCanvas.SetActive(false);
        m_DeathScreenCanvas.SetActive(false);
        m_EventSystem.SetActive(false);
        AudioListener.pause = false;
        HideCursor(true);
    }

    public void Pause ()
    {
        Time.timeScale = 0;
        m_HUDCanvas.SetActive(false);
        m_PauseCanvas.SetActive(true);
        m_DeathScreenCanvas.SetActive(false);
        m_EventSystem.SetActive(true);
        AudioListener.pause = true;
        HideCursor(false);
    }

    private void DeathScreen ()
    {
        Time.timeScale = 1;
        m_HUDCanvas.SetActive(false);
        m_PauseCanvas.SetActive(false);
        m_DeathScreenCanvas.SetActive(true);
        m_EventSystem.SetActive(true);
        AudioListener.pause = false;
        HideCursor(false);
    }

    public void Restart ()
    {
        m_Restarting = true;
        if (GameplayManager.Instance.IsDead)
        {
            m_DeathBlackScreen.Show = true;
            Invoke("LoadLastLevel", 1f);
        }
        else
        {
            Time.timeScale = 1;
            m_PauseBlackScreen.Show = true;
            Invoke("LoadLastLevel", 1f);
        }
    }

    private void LoadLastLevel ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit ()
    {
        AudioListener.pause = false;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnApplicationQuit ()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    private void HideCursor (bool hide)
    {
        if (hide)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
