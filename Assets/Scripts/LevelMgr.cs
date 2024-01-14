using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;

public class LevelMgr : MonoBehaviour
{
    public static LevelMgr levelMgr;
    [SerializeField] BlockGen blockGen;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] BridgeBuilder bridgeBuilder;
    [SerializeField] GameObject startImg;
    [SerializeField] GameObject endImg;
    [SerializeField] GameObject lvlImg;
    
    [SerializeField] TextMeshProUGUI lvlTxt;
    [SerializeField] GameObject PetCounter;
    [SerializeField] GameObject BridgeCounter;
    [SerializeField] GameObject CatPrefab;
    AudioSource source;
    bool playingMusic;
    List<GameObject> cats;
    
    TextMeshProUGUI petTxt;
    TextMeshProUGUI bridgeTxt;

    
    delegate void EnterDelegate();
    EnterDelegate enterFunction;

    Inputs inputs;

    int currentLevel = 1;
   
    int catsFound = 0;
    int totalCats = 0;
    void Awake(){
        levelMgr = this;
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Enter.performed += Enter;
        petTxt = PetCounter.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        bridgeTxt = BridgeCounter.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        source = GetComponent<AudioSource>();

    }

    void Start(){
        cats = new List<GameObject>();
        ClearMenu();
        ShowStartMenu();
    }
    void SetLvlVars(){
        totalCats = currentLevel;
        int rand = Random.Range(currentLevel*10, currentLevel*15);
        SetBridges(rand);
        bridgeBuilder.bridgeCount = rand;
    }
    public void SetBridges(int _amt){
        bridgeTxt.text = "x" + _amt;
        if (_amt == 0){
            bool _passed = catsFound == totalCats;
            EndLevel(_passed);
        }
    }
    void SpawnCats(int _amt){
        for (int i=0; i<_amt; i++){
            GameObject g = Instantiate(CatPrefab);
            Vector2Int _pos = blockGen.GetRandLandPos();
            g.transform.position = new Vector3(_pos.x + 0.5f, _pos.y + 1f, 0);
            cats.Add(g);
        }   
        totalCats = _amt;
        petTxt.text = _amt + "x";
        
    }
    
    public void AddCatFound(){
        catsFound++;
        petTxt.text = totalCats - catsFound  + "x";
        if (totalCats == catsFound){
            EndLevel(true);
        }
        
    }
    void Enter(InputAction.CallbackContext _c){
        enterFunction?.Invoke();

    }
    void ShowStartMenu(){
        currentLevel = 1;
        startImg.SetActive(true);
        lvlImg.SetActive(false);
        endImg.SetActive(false);
        enterFunction = ShowLevelMenu;
        source.Play();
        playingMusic = true;
    }
    void ShowLevelMenu(){
        startImg.SetActive(false);
        lvlImg.SetActive(true);
        endImg.SetActive(false);
        lvlTxt.text = "LEVEL " + currentLevel;
        enterFunction = StartLevel;
        if (!playingMusic){
            source.Play();
        }
    }
    void ShowGameOverMenu(){
        startImg.SetActive(false);
        lvlImg.SetActive(false);
        endImg.SetActive(true);
    }
    void ClearMenu(){
        PetCounter.SetActive(false);
        BridgeCounter.SetActive(false);
        startImg.SetActive(false);
        lvlImg.SetActive(false);
        endImg.SetActive(false);
    }
    void StartLevel(){
        ClearMenu();
        
        foreach (GameObject g in cats){
            Destroy(g);
        }
        cats.Clear();
        totalCats = 0;
        catsFound = 0;
        SetLvlVars();
        PetCounter.SetActive(true);
        BridgeCounter.SetActive(true);
        blockGen.StartLevel();
        playerMove.StartPos();
        bridgeBuilder.StartBridge();
        SpawnCats(Random.Range(currentLevel, (int)(currentLevel*1.5f)));
        enterFunction = null;
        source.Stop();
        playingMusic = false;
    }

    public void EndLevel(bool _passed){
        if (_passed){
            currentLevel++;
            BlockGen.mapSize += 10;
            ShowLevelMenu();

            return;
        }
        EndGame();

    }

    void EndGame(){
        
        ShowGameOverMenu();
        BlockGen.mapSize = blockGen.genOptions.mapSize;
        enterFunction = ShowStartMenu;
        
    }

}