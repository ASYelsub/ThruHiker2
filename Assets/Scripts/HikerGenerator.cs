using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HikerGenerator : MonoBehaviour
{
    public List<Hiker> activeHikers = new List<Hiker>();
    public List<GameObject> preMadehikers = new List<GameObject>();
    Queue<Hiker> queuedHikers = new Queue<Hiker>();
    public GameObject hikerPrefab;
    [SerializeField] TextAsset firstNamesAsset;
    [SerializeField] TextAsset lastNamesAsset;

    class NameData
    {
        public string name;
        public bool selected;
        public NameData(string n)
        {
            name = n;
            selected = false;
        }

    }
    List<NameData> firstNames = new List<NameData>();
    List<NameData> lastNames = new List<NameData>();
    public void Awake() { Services.HikerGenerator = this; }

    public void Init()
    {
        InitTextAssetData();
        ResetHikerSelection();
        GenerateHikers();
    }

    void InitTextAssetData()
    {
        string[] fn = firstNamesAsset.text.Split('\n');
        string[] ln = lastNamesAsset.text.Split('\n');

        for (int i = 0; i < fn.Length; i++) { firstNames.Add(new NameData(fn[i])); }
        for (int i = 0; i < ln.Length; i++) { lastNames.Add(new NameData(ln[i])); }
    }

    public void ResetHikerSelection()
    {
        foreach (NameData nd in firstNames)
        {
            nd.selected = false;
        }
        foreach (NameData nd in lastNames)
        {
            nd.selected = false;
        }
    }
    void GenerateHikers()
    {
        int hikersInLevel = Services.Game.currentLevel.hikersInLevel;

        for (int i = 0; i < hikersInLevel; i++)
        {
            GameObject newHikerObject = Instantiate(hikerPrefab, Vector3.zero, Quaternion.identity);
            Hiker newHiker = newHikerObject.GetComponent<Hiker>();
            queuedHikers.Enqueue(newHiker);
            newHiker.Init(GetRandomFirstName(), GetRandomLastName());
        }

    }

    string GetRandomFirstName()
    {
        NameData firstNameData = firstNames[Random.Range(0, firstNames.Count)];
        while (firstNameData.selected) { firstNameData = firstNames[Random.Range(0, firstNames.Count)]; }
        firstNameData.selected = true;
        return firstNameData.name;
    }
    string GetRandomLastName()
    {
        NameData lastNameData = lastNames[Random.Range(0, lastNames.Count)];
        while (lastNameData.selected) { lastNameData = lastNames[Random.Range(0, lastNames.Count)]; }
        lastNameData.selected = true;
        return lastNameData.name;
    }







}
