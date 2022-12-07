using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinPlayers : MonoBehaviour
{
    #region Singleton
    public static JoinPlayers instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }
    #endregion

    [Header("LISTS")]
    public List<GameObject> playerList;
    public List<string> tagList = new List<string>();
    public List<string> current_tagList = new List<string>();
    public List<Material> materialList = new List<Material>();
    public List<Material> current_MaterialList = new List<Material>();
    [HideInInspector] public List<int> id_list = new List<int>();

    public GameObject[] model_List;
    // From 0 to 3: Goblin [Purple - Blue - Green - Yellow]
    // From 4 to 7: Wizard [Same color order]
    // From 8 to 11: Knight [Same color order]
    // From 12 to 15: Orc [Same color order] --> STILL NO MODEL!

    [Space]
    [Header("PLAYER STUFF")]
    public GameObject playerPrefab;

    [SerializeField] private GameManager.Scene newScene;


    private void Start()
    {
        CreateAllPlayers();
        AddAvailableTags();
        AddAvailableMaterials();
    }

    public void CreateAllPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.GetComponent<Player_OldSystem>().playerID = i;
            newPlayer.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }
    public void AddAvailableTags()
    {
        current_tagList.Add(tagList[0]);
        current_tagList.Add(tagList[1]);
        current_tagList.Add(tagList[2]);
        current_tagList.Add(tagList[3]);
    }
    public void AddAvailableMaterials()
    {
        current_MaterialList.Add(materialList[0]);
        current_MaterialList.Add(materialList[1]);
        current_MaterialList.Add(materialList[2]);
        current_MaterialList.Add(materialList[3]);
    }

    public void ResetCurrentTagList()
    {
        current_tagList.Clear();
        current_tagList.Add(tagList[0]);
        current_tagList.Add(tagList[1]);
        current_tagList.Add(tagList[2]);
        current_tagList.Add(tagList[3]);
    }
    public void ResetCurrentMaterialList()
    {
        current_MaterialList.Clear();
        current_MaterialList.Add(materialList[0]);
        current_MaterialList.Add(materialList[1]);
        current_MaterialList.Add(materialList[2]);
        current_MaterialList.Add(materialList[3]);
    }
}
