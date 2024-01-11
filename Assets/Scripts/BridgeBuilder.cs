using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BridgeBuilder : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase[] tileArr;
    [SerializeField] int cols = 3;
    [SerializeField] public LayerMask tileLayerMask;
    Inputs inputs;
    BlockGen blockGen;
    BridgeBlock[,] bridgeGrid;
    int mapSize;
    void Awake(){
        blockGen = GetComponent<BlockGen>();
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.L_Click.performed += Click;
    }
    
    void Start(){
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
    void Click(InputAction.CallbackContext _c){
        RaycastHit2D hit;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(worldPoint, Vector2.down);

        if (hit.collider != null){
            //Debug.Log(hit.point);

            var tpos = blockGen.waterMap.WorldToCell(hit.point);
            print(tpos);
            //Vector2Int v2 = new Vector2Int(tpos.x, tpos.y);
            
            //var tile = blockGen.waterMap.GetTile(tpos);
            //PlaceBridge(v2);
            

        
        }      
    }
    
   
    
    public void PlaceBridge(Vector2Int _pos){
        PlaceBlockTile(_pos);
        UpdateNearBridges(_pos);
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
