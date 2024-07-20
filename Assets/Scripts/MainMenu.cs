using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is attached to the Canvas gameObject
public class MainMenu : MonoBehaviour
{
    //This is a instance of the MainMenu 
    public static MainMenu instance;

    //Events 
    public static event System.Action levelButtonClicked;
    public static event System.Action loadEndlessLevel;
    public static event System.Action ResetGrid;

    //All the Interactable and NonInteractable UI Elements
    [SerializeField] private GameObject mainView, gameView, levelGameView, buttons, level, endlessBtn, levelBtn, 
        backFromLevelBtn, backFromGameViewBtn,backFromLevelGameViewBtn,quitBtn;
    public GameObject LevelGameView { get { return levelGameView; } }
    public GameObject MainView { get { return mainView; } }
    
    /// <summary>
    /// Set instance to this and check so that there
    /// are no other.
    /// </summary>
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
    }

    /// <summary>
    /// Assign all the Buttons its functionality
    /// </summary>
    void Start()
    {
        endlessBtn.GetComponent<Button>().onClick.AddListener(() => { OnEnlessButtonClicked(); });
        levelBtn.GetComponent<Button>().onClick.AddListener(() => { OnLevelButtonClicked(); });
        backFromLevelBtn.GetComponent<Button>().onClick.AddListener(() => { OnBackFromLevelButtonCLicked(); });
        backFromGameViewBtn.GetComponent<Button>().onClick.AddListener(() => { OnBackFromGameViewClicked(); });
        backFromLevelGameViewBtn.GetComponent<Button>().onClick.AddListener(() => { OnBackFromLevelGameViewClicked(); });
        quitBtn.GetComponent<Button>().onClick.AddListener(() => { Application.Quit(); });
    }

    /// <summary>
    /// Function for levelButton From mainMenu
    /// </summary>
    void OnLevelButtonClicked()
    {
        buttons.SetActive(false);
        level.SetActive(true);
        levelButtonClicked?.Invoke();
    }

    /// <summary>
    /// Function For back Button From Levels Menu
    /// </summary>
    void OnBackFromLevelButtonCLicked()
    {
        buttons.SetActive(true);
        level.SetActive(false);
    }

    /// <summary>
    /// Function For endless Button From MainMenu
    /// </summary>
    void OnEnlessButtonClicked()
    {
        mainView.SetActive(false);
        gameView.SetActive(true);
        loadEndlessLevel?.Invoke();
    }

    /// <summary>
    /// Function For BackButton from Enless Level game View
    /// </summary>
    void OnBackFromGameViewClicked()
    {
        ResetGrid?.Invoke();
        mainView.SetActive(true);
        gameView.SetActive(false);
    }

    /// <summary>
    /// Function For BackButton From level Game View
    /// </summary>
    void OnBackFromLevelGameViewClicked()
    {
        ResetGrid?.Invoke();
        mainView.SetActive(true);
        gameView.SetActive(false);
        levelGameView.SetActive(false);
    }

    /// <summary>
    /// calledfrom GameManager
    /// When LevelButton is Clicked in Level Menu
    /// </summary>
    public void OpenLevelsFromLevelGameView()
    {
        ResetGrid?.Invoke();
        mainView.SetActive(true);
        gameView.SetActive(false);
        levelGameView.SetActive(false);
        buttons.SetActive(false);
        level.SetActive(true);
    }
}
