using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This Script is attached to LevelTile Prefab 
public class LevelSelector : MonoBehaviour
{
    //Ref form its text and lock Image
    [SerializeField] private TMP_Text m_main;
    [SerializeField] private GameObject m_unloackImage;

    //Ref for data class
    [SerializeField] private Data m_data;

    //dictionary to store all the alphabets from dataclass
    private Dictionary<int, string> m_alphabets = new Dictionary<int, string>();
    //bool to check if the tile is locked
    private bool m_isLocked = true;

    /// <summary>
    /// Add listener to button and initalize dic with letters
    /// </summary>
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { LoadLevel(); });

        for (int i = 0; i < m_data.gridData.Length; i++)
        {
            m_alphabets[i] = m_data.gridData[i].letter;
        }
    }

    /// <summary>
    /// Set Tille letter value
    /// called from gameManager
    /// </summary>
    /// <param name="text"> value of the text a,b...z etc</param>
    public void SetText(string text)
    {
        m_main.text = text;
    }

    /// <summary>
    /// sets Data classes data
    /// called from GameManager
    /// </summary>
    /// <param name="data">class values</param>
    public void SetLevelData(Data data)
    {
        this.m_data = data;
    }

    /// <summary>
    /// Listerner for Level Button when
    /// the button is pressed grid is set and 
    /// targets to complete the level are set 
    /// </summary>
    private void LoadLevel()
    {
        GameManager gm = GameManager.instance;
        MainMenu menu = MainMenu.instance;

        menu.LevelGameView.SetActive(true);
        menu.MainView.SetActive(false);

        gm.LoadLevelGrid(m_data.gridSize.x, m_data.gridSize.y, m_alphabets);

        if (m_data.bugCount > 0) { gm.SetBugs(true); gm.SetTotalBugs(m_data.bugCount); gm.SetBugsText(); }
        if (m_data.wordCount > 0) { gm.SetWordsTarget(true); gm.SetTotalWords(m_data.wordCount); gm.SetWordText(); }
        if (m_data.totalScore > 0) { gm.SetTotalScore(true); gm.SetTotalScore(m_data.totalScore); gm.SetScoreText(); }
        if (m_data.timeSec > 0) { gm.SetTime(true); gm.SetTotalTimer(m_data.timeSec); gm.SetTimerText(); }

        gm.GameOver = false;
    }

    /// <summary>
    /// Loack controller to controller wheiter the button is pressable or not
    /// </summary>
    /// <param name="tf">value is set to buttons intderactable</param>
    public void LockControlle(bool tf) 
    {
        m_isLocked = tf; 
        GetComponent<Button>().interactable = (m_isLocked == true) ? true : false;
        m_unloackImage.SetActive(!tf);
    }

}
