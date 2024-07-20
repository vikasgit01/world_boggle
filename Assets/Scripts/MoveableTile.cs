using System.Collections;
using UnityEngine;

//This Scipt is place on Tile Prefabs
public class MoveableTile : MonoBehaviour
{
    //ref tile script
    private Tile tile;
    //Ref to ienumerator
    private IEnumerator moveCoroutine;

    void Awake() => tile = GetComponent<Tile>();
    
    /// <summary>
    /// To animate the tile from one grid pos to other grid pos
    /// </summary>
    /// <param name="x">move pos to x</param>
    /// <param name="y">move pos to y</param>
    /// <param name="time">time to move in</param>
    public void Move(float x, float y, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = Movecoroutine(x, y, time);
        StartCoroutine(moveCoroutine);
    }

    /// <summary>
    /// Coroutine to move smoothly
    /// </summary>
    /// <param name="x">move pos to x</param>
    /// <param name="y">move pos to y</param>
    /// <param name="time">time to move in</param>
    /// <returns>retuns after one frame</returns>
    IEnumerator Movecoroutine(float x, float y, float time)
    {
        tile.X = x;
        tile.Y = y;

        Vector3 startPos = tile.transform.GetComponent<RectTransform>().anchoredPosition;
        Vector3 endPos = new Vector3(x, y , 0);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            tile.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos,
                    endPos, t / time);
            yield return 0;
        }

        tile.transform.GetComponent<RectTransform>().anchoredPosition = endPos;

    }
}
