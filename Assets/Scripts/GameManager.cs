using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This Scipt is place on the Canvas Gameobject
public class GameManager : MonoBehaviour
{
    //creating an instance of the class using singleton pattern
    public static GameManager instance;

    //Referance for implementing Game Mechanics
    [SerializeField] private Grid2D m_endlessLevelGrid, m_levelGrid;
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Transform m_parent;

    //Referance for implementing Game UI
    [SerializeField] private TMP_Text m_bugsText, m_wordsText, m_totalText, m_timeText, m_endlessText;
    [SerializeField] private GameObject m_bugs, m_words, m_total, m_time;

    private List<GameObject> m_leveButtons = new List<GameObject>();

    //Referance for implementing Game Currency and tasks.
    private int m_totalBugs, m_totalWords, m_totalScore;
    private int m_currentBugs, m_currentWords, m_currentScore;
    private int m_totalLevels, m_currentEndlessScore;
    private float m_totalTime;
    private float m_currentTime;

    private bool m_hasBugs, m_hasWordsTarget, m_hasTotalScore, m_hasTime;
    private bool m_levelLoaded;

    //Referance for gameover condition
    private bool m_gameOver = true;
    public bool GameOver { get { return m_gameOver; } set { m_gameOver = value; } }

    /// <summary>
    /// Subscribed to events from MainMenu Script
    /// </summary>
    private void OnEnable()
    {
        MainMenu.levelButtonClicked += LoadTotalLevels;
        MainMenu.loadEndlessLevel += LoadEndlessLevel;
        MainMenu.ResetGrid += ResetValues;
    }
    /// <summary>
    /// UnSubscribed to events from MainMenu Script
    /// </summary>
    private void OnDisable()
    {
        MainMenu.levelButtonClicked -= LoadTotalLevels;
        MainMenu.loadEndlessLevel -= LoadEndlessLevel;
        MainMenu.ResetGrid -= ResetValues;
    }
    /// <summary>
    /// Init instance to this script and 
    /// check that there will be only one instance of it
    /// </summary>
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
    }
    /// <summary>
    /// Check for GameOver(Every level) conditions
    /// </summary>
    void Update()
    {
        if (!m_gameOver)
        {
            if (m_totalTime == m_currentTime && m_totalBugs == m_currentBugs && m_totalWords == m_currentWords && m_totalScore <= m_currentScore)
            {
                StartCoroutine(NextLevel());
                m_gameOver = true;
            }
        }
    }

    /// <summary>
    /// Coroutine Called when Gameover condition is true
    /// </summary>
    /// <returns>return .3f float value</returns>
    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(.3f);
        MainMenu.instance.OpenLevelsFromLevelGameView();
        m_totalLevels++;
        if (m_totalLevels < m_leveButtons.Count) m_leveButtons[m_totalLevels].GetComponent<LevelSelector>().LockControlle(true);
    }
    /// <summary>
    /// subscibed to an event in MainMenu Script
    /// instantiates the buttons for levels window
    /// data from *LEVEL DATA* Json file.
    /// </summary>
    /// <NOTE>*** THERE ARE MANY LEVEL, ONLY USING 8 LEVELS ***</NOTE>
    void LoadTotalLevels()
    {
        if (!m_levelLoaded)
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject go = (GameObject)Instantiate(m_prefab, m_parent);

                m_leveButtons.Add(go);

                LevelSelector ls = go.GetComponent<LevelSelector>();

                ls.SetText((i + 1).ToString());
                ls.SetLevelData(JsonReader.m_lD.data[i]);

                if (i == 0) { ls.LockControlle(true); }
                else ls.LockControlle(false);
            }

            m_levelLoaded = true;
        }
    }
    /// <summary>
    /// Loads endless level
    /// calls function from Grid2D to create grid with given dimentions
    /// </summary>
    void LoadEndlessLevel()
    {
        m_endlessLevelGrid.Gridsize = new Vector2(4, 4);
        m_endlessLevelGrid.CreateGrid();
    }
    /// <summary>
    /// called from LevelSelector scipt
    /// loops throw all the Grid tiles and assinges static 
    /// letters from *LEVEL DATA* file.
    /// </summary>
    /// <param GridSize.x="x"></param>
    /// <param GridSize.y="y"></param>
    /// <param alpha from level data="alphabets"></param>
    public void LoadLevelGrid(int x, int y, Dictionary<int, string> alphabets)
    {
        List<Tile> tile = new List<Tile>();

        m_levelGrid.Gridsize = new Vector2(x, y);
        tile = m_levelGrid.CreateGrid();

        for (int i = 0; i < tile.Count; i++)
        {
            tile[i].LetterText.text = alphabets[i];
            tile[i].SetAlphabetPoints();
        }
    }

    /// <summary>
    /// This functions determin what kind of score system does the level have 
    /// if any of them is true then there respective ui is set Active.
    /// </summary>
    /// <param bool="tf"></param>
    public void SetBugs(bool tf) { m_hasBugs = tf; m_bugs.SetActive((m_hasBugs == true) ? true : false); }
    public void SetWordsTarget(bool tf) { m_hasWordsTarget = tf; m_words.SetActive((m_hasWordsTarget == true) ? true : false); }
    public void SetTotalScore(bool tf) { m_hasTotalScore = tf; m_total.SetActive((m_hasTotalScore == true) ? true : false); }
    public void SetTime(bool tf) { m_hasTime = tf; m_time.SetActive((m_hasTime == true) ? true : false); }

    /// <summary>
    /// functions to get which score system is active
    /// </summary>
    /// <returns>bool of respective system</returns>
    public bool HasBugs() { return m_hasBugs; }
    public bool HasWordsTager() { return m_hasWordsTarget; }
    public bool HasTotalScore() { return m_hasTotalScore; }
    public bool HasTimeTager() { return m_hasTime; }

    /// <summary>
    /// functions to get how may points did the player get 
    /// </summary>
    /// <returns>float for time int for others</returns>
    public int GetCurrentBugs() { return m_currentBugs; }
    public int GetCurrentWords() { return m_currentWords; }
    public int GetCurrentScore() { return m_currentScore; }
    public float GetCurrentTime() { return m_currentTime; }

    /// <summary>
    /// function increments the score systems with give value or just 1
    /// </summary>
    public void IncreaseCurrentBugs() { if (m_currentBugs <= m_totalBugs - 1) m_currentBugs++; }
    public void IncreaseSetCurrentWords() { if (m_currentWords <= m_totalWords - 1) m_currentWords++; }
    /// <param int="no"></param>
    public void IncreaseSetCurrentScore(int no) { if (m_currentScore <= m_totalScore) m_currentScore = no; }
    /// <returns>bool if time is complete</returns>
    public bool IncresaeTimer(int time)
    {
        if (time > 0)
        {
            m_currentTime -= Time.deltaTime;
            return false;
        }else
        {
            return true;
        }
    }

    /// <summary>
    /// Sets total system socre to achive 
    /// </summary>
    /// <param value="no"></param>
    public void SetTotalBugs(int no) => m_totalBugs = no;
    public void SetTotalWords(int no) => m_totalWords = no;
    public void SetTotalScore(int no) => m_totalScore = no;
    public void SetTotalTimer(float time) => m_totalTime = time;

    /// <summary>
    /// Set respective game score Texts UI
    /// </summary>
    public void SetBugsText() => m_bugsText.text = GetCurrentBugs() + " / " + m_totalBugs;
    public void SetWordText() => m_wordsText.text = GetCurrentWords() + " / " + m_totalWords;
    public void SetScoreText() => m_totalText.text = GetCurrentScore() + " / " + m_totalScore;
    public void SetTimerText() => m_timeText.text = GetCurrentTime().ToString();

    /// <summary>
    /// Score system for endless Level and set text
    /// </summary>
    /// <param value="no"></param>
    public void SetEndlessScore(int no) => m_currentEndlessScore = no;
    public void SetEndlessText() => m_endlessText.text = m_currentEndlessScore.ToString();

    /// <summary>
    /// subscribed to event from mainmenu sript
    /// Rests all the data to zero and sets gameover to false
    /// </summary>
    private void ResetValues()
    {
        m_currentBugs = m_currentWords = m_currentScore = m_totalBugs = m_totalWords = m_totalScore = 0;
        m_currentTime = m_totalTime = 0.0f;

        SetBugs(false);
        SetTime(false);
        SetWordsTarget(false);
        SetTotalScore(false);
        SetBugsText();
        SetWordText();
        SetScoreText();
        SetTimerText();
        m_gameOver = true;
    }
}
