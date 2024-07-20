using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

//This is place on top of Tile Prefabs
public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    //tile x any y pos on grid
    private float m_x;
    public float X
    { get { return m_x; } set { if (IsMovable()) m_x = value; } }
    private float m_y;
    public float Y
    { get { return m_y; } set { if (IsMovable()) m_y = value; } }

    //ref to Grid2D script
    private Grid2D m_grid;
    public Grid2D Grid
    { get { return m_grid; } }

    //Type of tile
    private Grid2D.TileType m_type;
    public Grid2D.TileType Type
    { get { return m_type; } }

    //ref to MoveableTileScript
    private MoveableTile m_moveable;
    public MoveableTile Moveable
    { get { return m_moveable; } }

    //ref to clearablePiece Script
    private ClearablePiece m_clearable;
    public ClearablePiece Clearable
    { get { return m_clearable; } }

    //image to set when selected
    [SerializeField] private Image m_main;
    public Image Main
    { get { return m_main; } }

    //Text to set for tile
    [SerializeField] private TMP_Text m_letterText;
    public TMP_Text LetterText
    { get { return m_letterText; } set { m_letterText = value; } }

    //bool to check when the tile is selected
    private bool m_isSelected = false;
    public bool IsSelected
    { get { return m_isSelected; } set { m_isSelected = value; } }

    //check if the tile canbe selected
    private bool m_canSelect = true;
    public bool CanSelect 
    { get { return m_canSelect; } set { m_canSelect = value; } }

    //indicater for points
    [SerializeField] private GameObject[] m_dots;

    //variable to store points
    private int m_alphabetPoints;

    /// <summary>
    /// init moveable and clearable script referance 
    /// </summary>
    void Awake()
    {
        m_moveable = GetComponent<MoveableTile>();
        m_clearable = GetComponent<ClearablePiece>();
    }

    /// <summary>
    /// init values of this tile
    /// called from Grid2D script
    /// </summary> 
    /// <param name="x">position in grid at x</param>
    /// <param name="y">position in grid at y</param>
    /// <param name="grid"> grid script ref</param>
    /// <param name="type">type of tile</param>
    public void Init(float x, float y, Grid2D grid, Grid2D.TileType type)
    {
        m_x = x;
        m_y = y;
        m_grid = grid;
        m_type = type;

        //If the tile type is not empty then set its letters
        if (m_type != Grid2D.TileType.EMPTY)
        {
            m_letterText.text = Letters.GetRandomLetter().ToString();
            SetAlphabetPoints();
        }
    }

    /// <summary>
    /// check for moveable script
    /// </summary>
    /// <returns>true if movable scpit exists</returns>
    public bool IsMovable() { return m_moveable != null; }

    /// <summary>
    /// check for clearable script
    /// </summary>
    /// <returns>true if clearable scpit exists</returns>
    public bool IsClearable() { return m_clearable != null; }

    /// <summary>
    /// when clicked on tile
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_canSelect && !m_isSelected)
        {
            m_main.color = SetColor(m_main.color, .5f);
            Debug.Log("IsDown");
            m_grid.SetWord(m_letterText.text.ToLower());
            m_grid.SetPoints(m_alphabetPoints);
            //m_grid.StartTIleCLicked(this);
            m_grid.SelectedTiles(this);
            m_grid.SetDraging(true);
            m_isSelected = true;
        }
    }

    /// <summary>
    /// when lifted up from tile
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_grid.IsDraging())
        {
            if (m_isSelected)
            {
                Debug.Log("IsUp");
                m_grid.CheckAnswer();
                m_grid.SetDraging(false);
                m_grid.ResetTile();
                m_grid.ResetWord();
                m_grid.ResetPoints();
                m_isSelected = false;
            }
        }
    }

    /// <summary>
    /// when entered other grid
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_grid.IsDraging())
        {
            if (m_canSelect && !m_isSelected)
            {
                m_main.color = SetColor(m_main.color, .5f);
                m_grid.SetWord(m_letterText.text.ToLower());
                m_grid.SetPoints(m_alphabetPoints);
                m_grid.SelectedTiles(this);
                m_isSelected = true;
            }
        }
    }
    /// <summary>
    /// function to set alphabet points based on there letter
    /// called at init method which is called by grid2D script -> for endless level
    /// and called from gamemanager also -> for levels 
    /// </summary>
    public void SetAlphabetPoints()
    {
        foreach (var item in Letters.point10Letters)
        {
            if (item.ToString() == m_letterText.text.ToLower()) m_alphabetPoints = 10;
        }

        foreach (var item in Letters.point20Letters)
        {
            if (item.ToString() == m_letterText.text.ToLower()) m_alphabetPoints = 20;
        }

        foreach (var item in Letters.point30Letters)
        {
            if (item.ToString() == m_letterText.text.ToLower()) m_alphabetPoints = 30;
        }


        if (m_alphabetPoints == 10)
        {
            m_dots[0].GetComponent<Image>().color = SetColor(m_dots[0].GetComponent<Image>().color, 1f);
            m_dots[1].GetComponent<Image>().color = SetColor(m_dots[1].GetComponent<Image>().color, .3f);
            m_dots[2].GetComponent<Image>().color = SetColor(m_dots[2].GetComponent<Image>().color, .3f);

        }
        else if (m_alphabetPoints == 20)
        {
            m_dots[0].GetComponent<Image>().color = SetColor(m_dots[0].GetComponent<Image>().color, 1f);
            m_dots[1].GetComponent<Image>().color = SetColor(m_dots[1].GetComponent<Image>().color, 1f);
            m_dots[2].GetComponent<Image>().color = SetColor(m_dots[2].GetComponent<Image>().color, .3f);

        }
        else if (m_alphabetPoints == 30)
        {
            m_dots[0].GetComponent<Image>().color = SetColor(m_dots[0].GetComponent<Image>().color, 1f);
            m_dots[1].GetComponent<Image>().color = SetColor(m_dots[1].GetComponent<Image>().color, 1f);
            m_dots[2].GetComponent<Image>().color = SetColor(m_dots[2].GetComponent<Image>().color, 1f);
        }
    }

    /// <summary>
    /// Set alpha of the tile selected
    /// </summary>
    /// <param name="cl"> color component of the object u wanna change</param>
    /// <param name="alphaVlaue">alphavalue u wanna set it too</param>
    /// <returns>color after change</returns>
    Color SetColor(Color cl, float alphaVlaue)
    {
        Color temp = cl;
        temp.a = alphaVlaue;
        cl = temp;
        return cl;
    }

}
