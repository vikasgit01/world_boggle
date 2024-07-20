using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class LettersAndWords : MonoBehaviour
{
    //hasset for stroing the words
    public static HashSet<string> words = new HashSet<string>();

    //referances for paths and array to store the words
    string[] m_words;
    string m_filePath, m_fileName;

    /// <summary>
    /// SetsFile Path and Coroutine to download/load data from word list
    /// </summary>
    void Awake()
    {
        m_fileName = "wordlist.txt";
        m_filePath = Application.persistentDataPath + "/" + m_fileName;
        ReadFromTxt();
    }

    /// <summary>
    /// If file exits loads the data otherwise downloads the file
    /// </summary>
    /// <param name="url"> url of the file to download from</param>
    /// <returns></returns>
    IEnumerator DownloadTxt(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            if (!File.Exists(Path.Combine(m_filePath)))
            {
                File.WriteAllBytes(Path.Combine(m_filePath), uwr.downloadHandler.data);
            }
            else
            {
                ReadFromTxt();
            }
        }
    }
    /// <summary>
    /// function to read data from files and load it into hasset
    /// </summary>
    void ReadFromTxt()
    {
        m_words = File.ReadAllLines(m_filePath);

        System.Array.Sort(m_words);
        foreach (string word in m_words)
        {
            words.Add(word);
        }
    }
}
