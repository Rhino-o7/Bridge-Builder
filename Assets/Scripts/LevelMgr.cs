using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;

public class LevelMgr : MonoBehaviour
{
    public static LevelMgr levelMgr;
    [SerializeField] int startSeconds;
    [SerializeField] int addesSecondsPerLevel;
    [SerializeField] BlockGen blockGen;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] BridgeBuilder bridgeBuilder;
    [SerializeField] GameObject startImg;
    [SerializeField] GameObject endImg;
    [SerializeField] GameObject lvlImg;
    [SerializeField] GameObject pauseMenu;
     
    [SerializeField] TextMeshProUGUI lvlTxt;
    [SerializeField] GameObject PetCounter;
    [SerializeField] GameObject BridgeCounter;
    [SerializeField] GameObject TimerObj;
    [SerializeField] GameObject CatPrefab;
    [SerializeField] Texture2D cursor;
    [SerializeField] AudioClip MenuAudio;
    [SerializeField] AudioClip GameOverClip;
    AudioSource source;
    bool playingMusic;
    List<GameObject> cats;
    
    TextMeshProUGUI petTxt;
    TextMeshProUGUI bridgeTxt;
    TextMeshProUGUI timerTxt;

    
    delegate void EnterDelegate();
    EnterDelegate enterFunction;

    Inputs inputs;

    int currentLevel = 1;
   
    int catsFound = 0;
    int totalCats = 0;
    int secondsLeft;
    void Awake(){
        levelMgr = this;
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Enter.performed += Enter;
        petTxt = PetCounter.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        bridgeTxt = BridgeCounter.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        timerTxt = TimerObj.GetComponent<TextMeshProUGUI>();
        source = GetComponent<AudioSource>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

    }

    void Start(){
        cats = new List<GameObject>();
        ClearMenu();
        ShowStartMenu();
    }
    void SetLvlVars(){
        totalCats = currentLevel;
        int _amt = currentLevel * 15 - (currentLevel-1) * 2;
        SetBridges(_amt);
        bridgeBuilder.bridgeCount = _amt;
    }
    public void SetBridges(int _amt){
        bridgeTxt.text = "x" + _amt;
        if (_amt == 0){
            bool _passed = catsFound == totalCats;
            //EndLevel(_passed);
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
        source.clip = MenuAudio;
        source.Play();
        playingMusic = true;
    }
    void ShowLevelMenu(){
        ClearMenu();
        startImg.SetActive(false);
        lvlImg.SetActive(true);
        endImg.SetActive(false);
        lvlTxt.text = "LEVEL " + currentLevel;
        enterFunction = StartLevel;
        if (!playingMusic){
            source.clip = MenuAudio;
            source.Play();
        }
    }
    void ShowGameOverMenu(){
        ClearMenu();
        startImg.SetActive(false);
        lvlImg.SetActive(false);
        endImg.SetActive(true);
        source.clip = GameOverClip;
        source.Play();
    }
    void ClearMenu(){
        PetCounter.SetActive(false);
        BridgeCounter.SetActive(false);
        TimerObj.SetActive(false);
        startImg.SetActive(false);
        lvlImg.SetActive(false);
        endImg.SetActive(false);
        pauseMenu.SetActive(false);
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
        TimerObj.SetActive(true);
        blockGen.StartLevel();
        playerMove.StartPos();
        bridgeBuilder.StartBridge();
        SpawnCats(Random.Range(currentLevel, (int)(currentLevel*1.5f)));
        enterFunction = Pause;
        source.Stop();
        playingMusic = false;
        secondsLeft = startSeconds + addesSecondsPerLevel * currentLevel;
        StartCoroutine(Timer());
        playerMove.canMove = true;
    }

    public void EndLevel(bool _passed){
        
        StopAllCoroutines();
        playerMove.canMove = false;
        if (_passed){
            currentLevel++;
            BlockGen.mapSize += 10;
            ShowLevelMenu();

            return;
        }
        EndGame();
        foreach (GameObject g in cats){
            Destroy(g);
        }

    }

    public void Pause(){
        pauseMenu.SetActive(true);
        bridgeBuilder.enabled = false;
        playerMove.canMove = false;
        playerMove.audioSource.Stop();
        enterFunction = Resume;
        Time.timeScale = 0;
        inputs.Player.Restart.performed += EndFromPause;
        
        
    }
    void EndFromPause(InputAction.CallbackContext c){
        inputs.Player.Restart.performed -= EndFromPause;
        Resume();
        EndLevel(false);
    }
    public void Resume(){
        inputs.Player.Restart.performed -= EndFromPause;
        pauseMenu.SetActive(false);
        bridgeBuilder.enabled = true;
        playerMove.canMove = true;
        Time.timeScale = 1;
        enterFunction = Pause;
    }

    void EndGame(){
        
        ShowGameOverMenu();
        BlockGen.mapSize = blockGen.genOptions.mapSize;
        enterFunction = ShowStartMenu;
        
    }

    IEnumerator Timer(){
        while (secondsLeft > 0){
            secondsLeft--;
            int _min = secondsLeft/60;
            int _sec = secondsLeft - 60*_min;
            string _txt = _min + ":";
            if (_sec < 10){
                _txt += 0;
            }
            _txt += _sec;
            timerTxt.text = _txt;
            yield return new WaitForSeconds(1);
        }
        EndLevel(false);
    }

}