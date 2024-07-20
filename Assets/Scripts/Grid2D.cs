using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grid2D : MonoBehaviour
{
    //values to set grid
    [SerializeField] private Vector2 m_gridsize;
    public Vector2 Gridsize { get { return m_gridsize; } set { m_gridsize = value; } }
    [SerializeField] private Vector2 m_space;
    [SerializeField] private float m_outerSpace, fillTime;
    [SerializeField] private bool m_canAnimate;
    [SerializeField] private TMP_Text m_wordText;

    //Type of tiles
    public enum TileType
    {
        NORMAL,
        EMPTY,
        BUG,
        BLOCKED
    }
    //SpawnTile sets its type and pregab
    [System.Serializable]
    public struct spawnTile
    {
        public TileType tileType;
        public GameObject prefab;
    };
    public spawnTile[] m_spawnTile;

    //bool to check if player is dragging on tiles
    private bool m_isDraging;
    //Dictionary to store tiles type and prefab
    private Dictionary<TileType, GameObject> m_tilePrefabDic;
    //Array to store tile scipt of grid tiles
    private Tile[,] m_tiles;
    //List of selected tiles
    private List<Tile> m_selectedTiles = new List<Tile>();
    //current selected tiles
    private Tile m_currentSelectedTile;
    //HashSet for storing words that are done.
    private HashSet<string> m_doneWords = new HashSet<string>();
    //float to store total grid size with spaces and outline spacing
    private float m_totalx = 0.0f, m_totaly = 0.0f;
    //bool to check inverse of the grid
    private bool m_inverse = false;
    //string to store the word that player is creating
    private string m_word = "";
    //int to keep track of points got for words and letters
    private int m_points, tempPoints;

    /// <summary>
    /// Subscribed to Event from MainMenu
    /// </summary>
    private void OnEnable()
    {
        MainMenu.ResetGrid += ResetGrid;
    }
    /// <summary>
    /// UnSubscribed to Evennt From MainMenu
    /// </summary>
    private void OnDisable()
    {
        MainMenu.ResetGrid -= ResetGrid;
    }
    /// <summary>
    /// Function to check made word is correct or not
    /// </summary>
    public void CheckAnswer()
    {
        //word sld be grater that 2 letters
        if (m_word.Length >= 3)
        {
            //Should not contain in hashset of words
            if (!m_doneWords.Contains(m_word))
            {
                //if word contains in hashset made in LetterAndWords Script from wordlist.txt
                if (LettersAndWords.words.Contains(m_word))
                {
                    //if tiles are Dynamic
                    if (!m_canAnimate) { CheckWord(); return; }
                    //if tiles are Dynamic
                    if (m_canAnimate)
                    {
                        tempPoints = m_points;
                        GameManager.instance.SetEndlessScore(m_points);
                        GameManager.instance.SetEndlessText();

                        for (int x = 0; x < m_gridsize.x; x++)
                        {
                            for (int y = 0; y < m_gridsize.y; y++)
                            {
                                CheckWordForCanAnimate(x, y);
                            }
                        }
                        StartCoroutine(Fill());
                        m_doneWords.Add(m_word);
                        ResetWord();
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Funciton create Grid
    /// </summary>
    /// <returns>List of tile script created</returns>
    public List<Tile> CreateGrid()
    {
        List<Tile> tile = new List<Tile>();

        //Add gameobject to dictionay
        m_tilePrefabDic = new Dictionary<TileType, GameObject>();
        for (int i = 0; i < m_spawnTile.Length; i++)
        {
            if (!m_tilePrefabDic.ContainsKey(m_spawnTile[i].tileType))
            {
                m_tilePrefabDic.Add(m_spawnTile[i].tileType, m_spawnTile[i].prefab);
            }
        }

        //if tiles are Dynamic
        if (m_canAnimate)
        {
            m_tiles = new Tile[(int)m_gridsize.x, (int)m_gridsize.y];
            for (int x = 0; x < m_gridsize.x; x++)
            {
                for (int y = 0; y < m_gridsize.y; y++)
                {
                    SpawnTile(x, y, TileType.EMPTY);
                }
            }

            StartCoroutine(Fill());
        }
        else if (!m_canAnimate) //If tiles are static
        {
            m_tiles = new Tile[(int)m_gridsize.x, (int)m_gridsize.y];
            for (int x = 0; x < m_gridsize.x; x++)
            {
                for (int y = 0; y < m_gridsize.y; y++)
                {
                    tile.Add(SpawnTile(x, y, TileType.NORMAL));
                }
            }
        }
        //set background panel to fit the grid
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2((m_totalx - m_space.x) + (m_outerSpace * 2), (m_totaly - m_space.y) + (m_outerSpace * 2));
        return tile;
    }
    /// <summary>
    /// if tile under the grid or diagonal is empty then it will fill
    /// </summary>
    private IEnumerator Fill()
    {
        while (FillStep())
        {
            m_inverse = !m_inverse;
            yield return new WaitForSeconds(fillTime);
        }
    }
    private bool FillStep()
    {
        bool MovePiece = false;
        for (int y = 1; y < m_gridsize.y; y++)
        {
            for (int lx = 0; lx < m_gridsize.x; lx++)
            {
                int x = lx;

                if (m_inverse)
                {
                    x = (int)m_gridsize.x - 1 - lx;
                }

                Tile tile = m_tiles[x, y];
                if (tile.IsMovable())
                {
                    Tile tileBelow = m_tiles[x, y - 1];
                    if (tileBelow.Type == TileType.EMPTY)
                    {
                        float posx = tileBelow.GetComponent<RectTransform>().anchoredPosition.x;
                        float posy = tileBelow.GetComponent<RectTransform>().anchoredPosition.y;
                        Destroy(tileBelow.gameObject);

                        tile.Moveable.Move(posx, posy, fillTime);
                        m_tiles[x, y - 1] = tile;
                        SpawnTile(x, y, TileType.EMPTY);
                        MovePiece = true;
                    }
                    else
                    {

                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagx = x + diag;
                                if (m_inverse)
                                {
                                    diagx = x - diag;
                                }
                                if (diagx >= 0 && diagx < m_gridsize.x)
                                {
                                    Tile diagTile = m_tiles[diagx, y - 1];
                                    if (diagTile.Type == TileType.EMPTY)
                                    {
                                        bool hasTileAbove = true;
                                        for (int abovey = y; abovey >= 0; abovey--)
                                        {
                                            Tile tileabove = m_tiles[diagx, abovey];
                                            if (tileabove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!tileabove.IsMovable() && tileabove.Type != TileType.EMPTY)
                                            {
                                                hasTileAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasTileAbove)
                                        {
                                            float posx = diagTile.GetComponent<RectTransform>().anchoredPosition.x;
                                            float posy = diagTile.GetComponent<RectTransform>().anchoredPosition.y;
                                            Destroy(diagTile.gameObject);

                                            tile.Moveable.Move(posx, posy, fillTime);
                                            m_tiles[diagx, y - 1] = tile;
                                            SpawnTile(x, y, TileType.EMPTY);
                                            MovePiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < m_gridsize.x; x++)
        {
            Tile tileBelow = m_tiles[x, (int)m_gridsize.y - 1];
            if (tileBelow.Type == TileType.EMPTY)
            {
                Destroy(tileBelow.gameObject);
                GameObject newTile = (GameObject)Instantiate(m_tilePrefabDic[TileType.NORMAL]);

                newTile.transform.SetParent(transform);
                float posx = tileBelow.GetComponent<RectTransform>().anchoredPosition.x;
                float posy = tileBelow.GetComponent<RectTransform>().anchoredPosition.y;

                newTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(posx, 2 * posy, 0);

                m_tiles[x, (int)m_gridsize.y - 1] = newTile.GetComponent<Tile>();
                m_tiles[x, (int)m_gridsize.y - 1].Init(x, m_gridsize.y - 1, this, TileType.NORMAL);
                m_tiles[x, (int)m_gridsize.y - 1].Moveable.Move(posx, posy, fillTime);

                MovePiece = true;
            }
        }

        return MovePiece;
    }

    /// <summary>
    /// Spawn Empty tile
    /// </summary>
    /// <param name="x">grid spawn point x</param>
    /// <param name="y">grid spawn point y</param>
    /// <param name="type">Tile Type</param>
    /// <returns>return spawn tile</returns>
    private Tile SpawnTile(int x, int y, TileType type)
    {
        GameObject newTile = (GameObject)Instantiate(m_tilePrefabDic[type]);
        newTile.transform.SetParent(transform);
        newTile.name = "Tile(" + x + "," + y + ")";

        Rect rect = newTile.GetComponent<RectTransform>().rect;
        float posx = (x * (rect.width + m_space.x)) + (rect.width / 2 + m_outerSpace);
        float posy = (y * (rect.height + m_space.y)) + (rect.height / 2 + m_outerSpace);

        newTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(posx, posy, 0);

        m_tiles[x, y] = newTile.GetComponent<Tile>();
        m_tiles[x, y].Init(x, y, this, type);

        m_totalx = (rect.width + m_space.x) * m_gridsize.x;
        m_totaly = (rect.height + m_space.y) * m_gridsize.y;

        return m_tiles[x, y];
    }
    /// <summary>
    /// if word is correctly made and isselected clear tiles and reset word
    /// </summary>
    /// <param name="x">position of the tile at x</param>
    /// <param name="y">position of the tile at y</param>
    private void CheckWordForCanAnimate(int x, int y)
    {
        if (m_tiles[x, y].IsClearable())
        {
            Debug.Log(m_tiles[x, y].IsSelected);
            if (m_tiles[x, y].IsSelected)
            {

                SetDraging(false);
                ClearTile(m_tiles[x, y], x, y);
            }
        }

    }
    /// <summary>
    /// if word is correctly made
    /// </summary>
    private void CheckWord()
    {
        GameManager gm = GameManager.instance;
        gm.IncreaseSetCurrentWords();
        gm.SetWordText();
        if (gm.HasTotalScore())
        {
            tempPoints = m_points;
            gm.IncreaseSetCurrentScore(m_points);
            gm.SetScoreText();
        }
        ResetTile();
        SetDraging(false);
        m_doneWords.Add(m_word);
        ResetWord();
    }
    /// <summary>
    /// function to clear tile and spawn new empty ones before fill
    /// </summary>
    /// <param name="tile">Tile script of clearing tile</param>
    /// <param name="x">x pos in grid</param>
    /// <param name="y">y pos in grid</param>
    private void ClearTile(Tile tile, int x, int y)
    {
        if (tile.IsClearable() && !tile.Clearable.IsBeingCleared)
        {
            m_selectedTiles.Remove(tile);
            tile.Clearable.Clear();
            SpawnTile(x, y, TileType.EMPTY);
        }
    }
    /// <summary>
    /// this is subsribed to event from mainmenu
    /// reset grid size to zero tiles to zero and made words to zero
    /// </summary>
    private void ResetGrid()
    {
        m_tilePrefabDic.Clear();
        m_selectedTiles.Clear();
        m_doneWords.Clear();

        foreach (var item in m_tiles)
        {
            Destroy(item.gameObject);
        }

        transform.GetComponent<RectTransform>().sizeDelta = Vector3.zero;

        ResetWord();
        m_points = 0;
        tempPoints = 0;
        m_currentSelectedTile = null;
        SetDraging(false);
    }
    /// <summary>
    /// Set All the Adjucent Tiles canSelect to True and Other to false
    /// </summary>
    /// <param name="tile">selected tile</param>
    private void SetAdjucentTileCanSelectToTrue(Tile tile)
    {

        for (int x = 0; x < m_gridsize.x; x++)
        {
            for (int y = 0; y < m_gridsize.y; y++)
            {
                SetTileCanSelect(m_tiles[x, y], true);

                if (m_currentSelectedTile == m_tiles[x, y])
                {
                    if (x == 0)
                    {
                        if (y == 0)
                        {
                            SetTileCanSelect(m_tiles[x, y + 1], true);
                            SetTileCanSelect(m_tiles[x + 1, y], true);
                            SetTileCanSelect(m_tiles[x + 1, y + 1], true);
                        }
                        else if (y == m_gridsize.y)
                        {
                            SetTileCanSelect(m_tiles[x + 1, y], true);
                            SetTileCanSelect(m_tiles[x, y - 1], true);
                            SetTileCanSelect(m_tiles[x - 1, y - 1], true);
                        }
                        else if (y < m_gridsize.y)
                        {
                            SetTileCanSelect(m_tiles[x + 1, y - 1], true);
                            SetTileCanSelect(m_tiles[x + 1, y], true);
                            SetTileCanSelect(m_tiles[x + 1, y + 1], true);
                            SetTileCanSelect(m_tiles[x, y + 1], true);
                            SetTileCanSelect(m_tiles[x, y - 1], true);
                        }
                    }
                    else if (y == 0)
                    {
                        if (x == m_gridsize.x)
                        {
                            SetTileCanSelect(m_tiles[x, y + 1], true);
                            SetTileCanSelect(m_tiles[x - 1, y], true);
                            SetTileCanSelect(m_tiles[x - 1, y + 1], true);
                        }
                        else
                        {
                            SetTileCanSelect(m_tiles[x, y + 1], true);
                            SetTileCanSelect(m_tiles[x - 1, y], true);
                            SetTileCanSelect(m_tiles[x + 1, y], true);
                            SetTileCanSelect(m_tiles[x - 1, y + 1], true);
                            SetTileCanSelect(m_tiles[x + 1, y + 1], true);
                        }
                    }
                    else if (y == m_gridsize.y)
                    {
                        if (x == m_gridsize.x)
                        {
                            SetTileCanSelect(m_tiles[x - 1, y], true);
                            SetTileCanSelect(m_tiles[x - 1, y - 1], true);
                            SetTileCanSelect(m_tiles[x, y - 1], true);
                        }
                        else
                        {
                            SetTileCanSelect(m_tiles[x - 1, y], true);
                            SetTileCanSelect(m_tiles[x + 1, y], true);
                            SetTileCanSelect(m_tiles[x - 1, y - 1], true);
                            SetTileCanSelect(m_tiles[x + 1, y - 1], true);
                            SetTileCanSelect(m_tiles[x, y - 1], true);
                        }
                    }
                    else if (x == m_gridsize.x)
                    {
                        SetTileCanSelect(m_tiles[x, y - 1], true);
                        SetTileCanSelect(m_tiles[x, y + 1], true);
                        SetTileCanSelect(m_tiles[x - 1, y + 1], true);
                        SetTileCanSelect(m_tiles[x - 1, y - 1], true);
                        SetTileCanSelect(m_tiles[x - 1, y], true);

                    }
                    else if(x < m_gridsize.x && y < m_gridsize.x)
                    {
                        SetTileCanSelect(m_tiles[x + 1, y + 1], true);
                        SetTileCanSelect(m_tiles[x - 1, y - 1], true);
                        SetTileCanSelect(m_tiles[x, y - 1], true);
                        SetTileCanSelect(m_tiles[x, y + 1], true);
                        SetTileCanSelect(m_tiles[x - 1, y], true);
                        SetTileCanSelect(m_tiles[x + 1, y], true);
                        SetTileCanSelect(m_tiles[x + 1, y - 1], true);
                        SetTileCanSelect(m_tiles[x - 1, y + 1], true);
                    }
                }
            }
        }
    }

    void SetTileCanSelect(Tile tile, bool tf)
    {
        tile.CanSelect = tf;
    }

    /// <returns> tile draging or not</returns>
    public bool IsDraging() { return m_isDraging; }
    /// <summary>
    /// set drag value
    /// </summary>
    /// <param name="tf">draging to true to false</param>
    public void SetDraging(bool tf) { m_isDraging = tf; }
    /// <summary>
    /// Add characters to word
    /// </summary>
    /// <param name="ch">character value to add</param>
    public void SetWord(string ch) { m_word += ch; m_wordText.text = m_word; }
    /// <summary>
    /// Reset word back to empty
    /// </summary>
    public void ResetWord() { m_word = ""; m_wordText.text = m_word; }
    /// <summary>
    /// add points to given no
    /// </summary>
    /// <param name="no">int to add to points</param>
    public void SetPoints(int no) => m_points += no;
    /// <summary>
    /// ResetPoints to tempPoints
    /// </summary>
    public void ResetPoints() => m_points = tempPoints;
    /// <summary>
    /// add selected tile script to array
    /// </summary>
    /// <param name="tile">selected tile</param>
    public void SelectedTiles(Tile tile)
    {
        /*m_currentSelectedTile = tile;
        SetAdjucentTileCanSelectToTrue(tile);*/
        m_selectedTiles.Add(tile);
        //Debug.Log(IsAdjacent(m_selectedTiles[0], m_selectedTiles[1]));
    }
    public bool IsAdjacent(Tile tile1, Tile tile2)
    {
        Rect rec = tile1.GetComponent<RectTransform>().rect;

        return (tile1.X == tile2.X && (int)Mathf.Abs(tile1.Y - tile2.Y) == rec.height + m_space.y) ||
            (tile1.Y== tile1.Y && (int)Mathf.Abs(tile1.X - tile2.X) == rec.width + m_space.x);
    }

    /// <summary>
    /// Reset tile color and set isselected to false also clears selected tiles form list
    /// </summary>
    public void ResetTile()
    {
        for (int x = 0; x < m_gridsize.x; x++)
        {
            for (int y = 0; y < m_gridsize.y; y++)
            {
                Color Temp = m_tiles[x, y].Main.color;
                Temp.a = 1f;
                m_tiles[x, y].Main.color = Temp;
                m_tiles[x, y].CanSelect = true;
                m_tiles[x, y].IsSelected = false;
            }
        }

        m_selectedTiles.Clear();
        m_currentSelectedTile = null;
    }

}
