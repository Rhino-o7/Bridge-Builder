using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BridgeBuilder : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase[] tileArr;
    [SerializeField] Tilemap hoverMap;
    [SerializeField] TileBase hoverTile;
    [SerializeField] int cols = 3;
    [SerializeField] public LayerMask tileLayerMask;
    public int bridgeCount;
    Inputs inputs;
    BlockGen blockGen;
    BridgeBlock[,] bridgeGrid;
    int mapSize;
    Vector2Int lastMousePos;
    AudioSource audioSource;
    void Awake(){
        audioSource = GetComponent<AudioSource>();
        blockGen = GetComponent<BlockGen>();
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.L_Click.performed += Click;
    }
    
    public void StartBridge(){
        tilemap.ClearAllTiles();

        mapSize = BlockGen.mapSize;
        bridgeGrid = new BridgeBlock[mapSize,mapSize];
        for (int i=0; i<mapSize; i++){
            for (int j=0; j<mapSize; j++){
                bridgeGrid[j, i] = new BridgeBlock
                {
                    type = BridgeType.NULL,
                    pos = new Vector2Int(j, i)
                };
            }
        }
    }
    void Update(){
        Vector2 _pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int _mousePos = new Vector2Int((int)_pos.x, (int)_pos.y);
        if (_mousePos.x >= 0 && _mousePos.x <mapSize && _mousePos.y >= 0 && _mousePos.y < mapSize){
            if (_mousePos != lastMousePos){
                hoverMap.SetTile(new Vector3Int(lastMousePos.x, lastMousePos.y, 0), null);
                if (blockGen.tileTraits[_mousePos.x, _mousePos.y].tileType == TileType.WATER && blockGen.xtraMap.GetTile(new Vector3Int(_mousePos.x, _mousePos.y, 0)) == null){

                    hoverMap.SetTile(new Vector3Int(_mousePos.x, _mousePos.y, 0), hoverTile);
                    
                }

            }
            lastMousePos = _mousePos;
        }
    }
    void Click(InputAction.CallbackContext _c){
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int _clickPos = new Vector2Int((int)_mousePos.x, (int)_mousePos.y);
        if (_mousePos.x >0 && _mousePos.x <mapSize && _mousePos.y >= 0 && _mousePos.y < mapSize 
        && blockGen.tileTraits[_clickPos.x,  _clickPos.y].tileType == TileType.WATER && tilemap.GetTile(new Vector3Int(_clickPos.x, _clickPos.y, 0)) == null && bridgeCount > 0
        && blockGen.xtraMap.GetTile(new Vector3Int((int)_mousePos.x, (int)_mousePos.y, 0)) == null){
            PlaceBridge(_clickPos);
        }
        

           
    }
    
   
    
    public void PlaceBridge(Vector2Int _pos){
        audioSource.Play();
        PlaceBlockTile(_pos);
        UpdateNearBridges(_pos);
        blockGen.colMap.SetTile(new Vector3Int(_pos.x, _pos.y,0), null);
        bridgeCount--;
        LevelMgr.levelMgr.SetBridges(bridgeCount);
    }   
   
    void PlaceBlockTile(Vector2Int _pos){
        BridgeType _type = GetBridgeType(_pos);
        if (_type == BridgeType.NULL){
            _type = BridgeType.UP;
        }
        bridgeGrid[_pos.x,_pos.y].type = _type;
        tilemap.SetTile(new Vector3Int(_pos.x,_pos.y,0), tileArr[GetBlockIndex(_type)]);
    }
    void UpdateNearBridges(Vector2Int _pos)
{
    for (int i = 0; i < 4; i++){
        Vector2Int _newPos = _pos;
        switch (i)
        {
        case 0:
            _newPos.x = _pos.x + 1;
            break;
        case 1:
            _newPos.x = _pos.x - 1;
            break;
        case 2:
            _newPos.y = _pos.y + 1;
            break;
        case 3:
            _newPos.y = _pos.y - 1;
            break;
        }
        if (_newPos.x < mapSize && _newPos.x >= 0 && _newPos.y < mapSize && _newPos.y >= 0 && bridgeGrid[_newPos.x, _newPos.y].type != BridgeType.NULL){
            PlaceBlockTile(_newPos);
        }
    }
}

    BridgeType GetBridgeType(Vector2Int _pos){
        bool top= false, bottom= false, left= false, right = false;
        
        if (_pos.y+1 < mapSize && bridgeGrid[_pos.x, _pos.y+1].type != BridgeType.NULL){
            top = true;
        }
        if (_pos.y-1 >=0 && bridgeGrid[_pos.x, _pos.y-1].type != BridgeType.NULL){
            bottom = true;
        }
        if (_pos.x+1 < mapSize && bridgeGrid[_pos.x+1, _pos.y].type != BridgeType.NULL){
            right = true;
        }
        if (_pos.x-1 >=0 && bridgeGrid[_pos.x-1, _pos.y].type != BridgeType.NULL){
            left = true;
        }
        
        if (top && bottom && left && right){
            return BridgeType.ALL;
        }
        if (left && right && bottom){
            return BridgeType.ALL;
        }  
        if (left && right && top){
            return BridgeType.UP_LR;
        } 

        if (top && bottom && left){
            return BridgeType.ALL; //ADD LATER
        }
        if (top && bottom && right){
            return BridgeType.ALL; //ADD LATER
        }

        if (top && right){
            return BridgeType.DOWN_RIGHT;
        }
        if (top && left){
            return BridgeType.DOWN_LEFT;
        }
        if (bottom && right){
            return BridgeType.UP_RIGHT;
        }
        if (bottom && left){
            return BridgeType.UP_LEFT;
        }
        if (left || right){
            return BridgeType.SIDE;
        }
        if (bottom || top){
            return BridgeType.UP;
        }
        return BridgeType.NULL;
    }

    public int GetBlockIndex(BridgeType _type){
        int blockRow = (int) _type;
        int blockCol = Random.Range(0,cols); //Random Varient of block
        int shapeIndex = (blockRow * cols) + blockCol;
        return shapeIndex; 
    }

    void OnDisable(){
        inputs.Player.L_Click.performed -= Click;
    }
    void OnEnable(){
        inputs.Player.L_Click.performed += Click;
    }

    
}

public enum BridgeType{
    SIDE,
    UP,
    UP_LEFT,
    UP_RIGHT,
    DOWN_LEFT,
    DOWN_RIGHT,
    UP_LR,
    ALL,
    NULL
}

public class BridgeBlock{
    public Vector2Int pos;
    public BridgeType type;
    public BridgeBlock(){
        pos = new Vector2Int(-1,-1);
        type = BridgeType.NULL;
    }
    
}
