using System.Collections;
using UnityEngine;

//This script is placed on Tile Prefabs Which are Clearable.
public class ClearablePiece : MonoBehaviour
{
    //Animation Clip For block dissapear
    [SerializeField] private AnimationClip m_animationClip;

    //Weither the tile is in progress of beeingg cleared
    private bool m_isBeingCleared = false;
    public bool IsBeingCleared
    { get { return m_isBeingCleared; } }

    //Ref to Tile script
    private Tile tile;

    /// <summary>
    /// Init tile 
    /// </summary>
    private void Awake() => tile = GetComponent<Tile>();

    /// <summary>
    /// Clear function to be called to 
    /// clear the tile from the grid
    /// funcition is called from Grid2D script
    /// </summary>
    public void Clear()
    {
        m_isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    /// <summary>
    /// Coroutine to wait untill the animation is done before clearing the tile
    /// </summary>
    /// <returns> float value of animation clip length</returns>
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(m_animationClip.name);
            yield return new WaitForSeconds(m_animationClip.length);
            Destroy(gameObject);
        }
    }
}
