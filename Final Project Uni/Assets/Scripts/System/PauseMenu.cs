using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void PauseGame()
    {
        Time.timeScale = 0.00001f;
        //AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        //AudioListener.pause = false;
    }

    public void Quit()
    {
        GameManager.Instance.QuitMenu();
    }
    
    public async void OnNewGameClicked()
    {
        GameManager.Instance.fadeInAnim.Invoke();
            
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SceneManager.LoadScene("Lobby");
            
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        gameObject.SetActive(false);
    }
}
