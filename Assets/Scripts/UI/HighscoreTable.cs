using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    public float templateHeight = 30f;
    public int maxLines = 8;
    public Color highlightLine;


    public Color goldColor;
    public Color silverColor;
    public Color bronzeColor;

    private Transform entryContainer;
    private Transform entryTemplate;

    private Highscores highscores;
    private List<Transform> highscoreEntryTransformList;
    
    //private List<HighscoreEntry> tempHighscores;
    
    public int playerEntryIndex = 0;
    
    //[HideInInspector]
    //public InputField playerName;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        entryTemplate.gameObject.SetActive(false);

        // Load saved Highscores
        highscores = LoadTable();

        highscoreEntryTransformList = new List<Transform>();
        FillTable(highscores.highscoreEntryList);
    }

    private Highscores LoadTable()
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            highscores = new Highscores();
            highscores.highscoreEntryList = InitializeHighscoreEntryList();
        }

        return highscores;
    }

    private List<HighscoreEntry> InitializeHighscoreEntryList()
    {
        List<HighscoreEntry> highscoreEntryList = new List<HighscoreEntry>();
        for (int i = 0; i < maxLines; i++)
        {
            highscoreEntryList.Add(new HighscoreEntry { name = "----", score = 0 });
        }
        return highscoreEntryList;
    }

    public void UpdateHighscoreTable(int score, string name)
    {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = ValidateName(name) };


        List<HighscoreEntry> tempHighscores = new List<HighscoreEntry>();
        tempHighscores.AddRange(highscores.highscoreEntryList);
        tempHighscores.Add(highscoreEntry);
        tempHighscores = SortList(tempHighscores);
        playerEntryIndex = tempHighscores.IndexOf(highscoreEntry);
    
        FillTable(tempHighscores);
    }

    public void AddHighscoreEntry(int score, string name)
    {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = ValidateName(name) };

        highscores.highscoreEntryList.Add(highscoreEntry);
        highscores.highscoreEntryList = SortList(highscores.highscoreEntryList);
        playerEntryIndex = highscores.highscoreEntryList.IndexOf(highscoreEntry);

        SaveTable(highscores);
        FillTable(highscores.highscoreEntryList);
    }

    private string ValidateName(string name)
    {
        if (name.Length < 1)
        {
            name = "----";
        }
        return name;
    }

    private List<HighscoreEntry> SortList(List<HighscoreEntry> list)
    {
        //Sort entry list by score
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[j].score > list[i].score)
                {
                    //Swap
                    HighscoreEntry tmp = list[i];
                    list[i] = list[j];
                    list[j] = tmp;
                }
            }
        }

        // Check size of table
        if (list.Count > maxLines - 1)
        {
            for (int i = maxLines; i < list.Count; i++)
            {
                list.RemoveAt(i);
            }
        }
        return list;
    }

    //Save updated Highscored
    private void SaveTable(Highscores highscores)
    {
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public void FillTable(List<HighscoreEntry> highscoreList)
    {
        // Delete old highscore entries
        if (highscoreEntryTransformList.Count > 1)
        {
            foreach (var item in highscoreEntryTransformList)
            {
                Destroy(item.gameObject);
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry _highscoreEntry in highscoreList)
        {
            CreateHighScoreEntryTransform(_highscoreEntry, entryContainer, highscoreEntryTransformList);
        }   
    }
    
    private void CreateHighScoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        Transform entryTransform = Instantiate(entryTemplate, container);

        //Set position in container of entry
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        Text posText = entryTransform.Find("posText").GetComponent<Text>();
        Text scoreText = entryTransform.Find("scoreText").GetComponent<Text>();
        InputField nameField = entryTransform.Find("nameField").GetComponent<InputField>();
        Text nameText = nameField.transform.Find("Text").GetComponent<Text>();

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH";
                break;
            case 1: 
                rankString = "1ST";
                posText.color = goldColor;
                scoreText.color = goldColor;
                nameText.color = goldColor;
                break;
            case 2: 
                rankString = "2ND";
                posText.color = silverColor;
                scoreText.color = silverColor;
                nameText.color = silverColor;
                break;
            case 3: 
                rankString = "3RD";
                posText.color = bronzeColor;
                scoreText.color = bronzeColor;
                nameText.color = bronzeColor;
                break;
        }

        posText.text = rankString;
        scoreText.text = highscoreEntry.score.ToString();
        nameField.text = highscoreEntry.name;
        nameField.interactable = false;

        transformList.Add(entryTransform);
    }

    public void HighlightLine(bool changeName)
    {
        if (playerEntryIndex == -1)
            return;
            
        Transform highlightEntry = highscoreEntryTransformList[playerEntryIndex];

        if (changeName)
        {
            InputField playerField = highlightEntry.Find("nameField").GetComponent<InputField>();
            playerField.interactable = true;
            playerField.ActivateInputField();
        }
        
        // Set color
        Image image = highlightEntry.GetComponent<Image>(); ;
        image.color = highlightLine;
    }

    public void changeName(string name)
    {
        //Rename player
        HighscoreEntry highscoreEntry = highscores.highscoreEntryList[playerEntryIndex];

        if (name.Length <= 0)
        {
            highscoreEntry.name = "----";
        }
        else
        {
            highscoreEntry.name = name;

        }
        PlayerPrefs.SetString("name", name);
        highscores.highscoreEntryList[playerEntryIndex] = highscoreEntry;

        SaveTable(highscores);
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }
}
