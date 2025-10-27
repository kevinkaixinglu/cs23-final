using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

      public void PlayGame() {
            SceneManager.LoadScene("Tutorial Level");
      }

      public void SettingsMenu() {
            SceneManager.LoadScene("Settings");
      }

      public void CreditsMenu() {
            SceneManager.LoadScene("Credits");
      }

      public void ExitGame() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
      }

      public void BackToMainMenu() {
            SceneManager.LoadScene("MainMenu");
      }
}
