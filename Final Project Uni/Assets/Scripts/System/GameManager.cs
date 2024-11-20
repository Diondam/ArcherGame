using System.Collections;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [CanBeNull] public GameObject Player;
    [CanBeNull] public GameObject ManagerObj;
    [CanBeNull] public GenerationManager genManager;
    //Scene Address
    [FoldoutGroup("Scene Address")]
    public string Expedition;
    [FoldoutGroup("Scene Address")]
    public string Lobby;

    [FoldoutGroup("Event")]
    public UnityEvent fadeInAnim, fadeOutAnim;
    public UnityEvent<Color> changeColorTransition;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if(Player != null) DontDestroyOnLoad(Player);
            if (ManagerObj != null) DontDestroyOnLoad(ManagerObj);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        // Logic for starting a new game
        SceneManager.LoadScene("Lobby");
    }

    public void LoadGame()
    {
        // Logic for loading a saved game
    }

    public void QuitGame()
    {
        // Logic for quitting the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    #region SceneLogic

    IEnumerator LoadExpedition()
    {
        fadeInAnim.Invoke();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(Expedition);
        genManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        fadeOutAnim.Invoke();
    }
    IEnumerator LoadLobby()
    {
        fadeInAnim.Invoke();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(Lobby);
        genManager.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        fadeOutAnim.Invoke();

    }
    public void StartExpedition()
    {
        PlayerController.Instance._playerData.SaveClaimReward();
        StartCoroutine(LoadExpedition());
    }
    public void StartLobby()
    {
        StartCoroutine(LoadLobby());
    }
    #endregion
}