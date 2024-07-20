using UnityEngine;

//This script is attached to the Canvas gameobject 
public class Letters : MonoBehaviour
{
    //array of letters 
    public static char[] letters = "abcdefgnijklmnopqrstuvwxyz".ToCharArray();

    //letters for their respective points
    public static char[] point10Letters = "abcdefghi".ToCharArray();
    public static char[] point20Letters = "jklmnopq".ToCharArray();
    public static char[] point30Letters = "rstuvwxyz".ToCharArray();

    /// <summary>
    /// Generates Random Letter 
    /// called from Tile script attached to tiles prefabs
    /// </summary>
    /// <returns>char that is randomly generated</returns>
    public static char GetRandomLetter()
    {
        int r = Random.Range(0, letters.Length);

        return letters[r];
    }
}
