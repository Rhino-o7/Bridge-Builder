using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//namespace UnityEngine{
public class BlockGen : MonoBehaviour
{
    [SerializeField] GenOptions genOptions;
    [SerializeField] NoiseMap noiseMapObj;
    
    [SerializeField] Tilemap groundMap;
    [SerializeField] public Tilemap waterMap;
    [SerializeField] public Tilemap colMap;
    [SerializeField] TileBase colTile;
    [SerializeField] TileBase[] tileArray;
    public TileTrait[,] tileTraits;
    float[,] noiseMap;
    public static Vector2 startPos;

    [HideInInspector]
    public static int mapSize;
    void Awake(){
        mapSize = genOptions.mapSize;
    }

    public void StartLevel(){
        
        groundMap.ClearAllTiles();
        waterMap.ClearAllTiles();
        colMap.ClearAllTiles();

        noiseMapObj.GenerateNoiseMap();
        noiseMap = noiseMapObj.noiseMap;
        tileTraits = new TileTrait[mapSize,mapSize];
        for (int i=0; i<mapSize; i++){
            for (int j=0; j<mapSize; j++){
                tileTraits[j,i] = new TileTrait();
            }
        }
    
        GenNoiseMap();
        GenBlocks();
        startPos = GetRandLandPos();
    }

    BlockType GetBlockTypeIndex(TileType[,] _x3){
        bool _top = false, _bottom = false, _left = false, _right = false, _cbl = false, _cbr = false;
        List<Vector2> waterTiles = new List<Vector2>();
        for (int i=0; i<3; i++){
            for (int j=0; j<3;j++){
                if (_x3[j,i] == TileType.WATER){
                    waterTiles.Add(new Vector2(j,i));
                }
            }
        }

        if (waterTiles.Contains(new Vector2(1,2))){
            _top = true;
        }
        if (waterTiles.Contains(new Vector2(1,0))){
            _bottom = true;
        }
        if (waterTiles.Contains(new Vector2(0,1))){
            _left = true;
        }
        if (waterTiles.Contains(new Vector2(2,1))){
            _right = true;
        }
        if (waterTiles.Contains(new Vector2(0,0))){
            _cbl = true;
        }
        if (waterTiles.Contains(new Vector2(2,0))){
            _cbr = true;
        }

        if (_top && _bottom && _left && _right){
            return BlockType.FULL_ROUND;
        }
        if (_left && _top && _right) {
            return BlockType.TOP_ROUND;
        }
        if (_left && _bottom && _right) {
            return BlockType.BOTTOM_ROUND;
        }
        if (_left && _top && _bottom){
            return BlockType.LEFT_ROUND;
        }
        if (_right && _top && _bottom){
            return BlockType.RIGHT_ROUND;
        }
        //
        if (_left && _top) {
            return BlockType.TOP_LEFT;
        }
        if (_right && _top) {
            return BlockType.TOP_RIGHT;
        }
        if (_left && _bottom) {
            return BlockType.BOTTOM_LEFT;
        }
        if (_right && _bottom) {
            return BlockType.BOTTOM_RIGHT;
        }
        if (_right && _left){
            return BlockType.LEFT_RIGHT;
        }
        if (_top && _bottom){
            return BlockType.TOP_BOTTOM;
        }
        //
        if (_top) {
            return BlockType.TOP;
        }
        if (_bottom) {
            return BlockType.BOTTOM;
        }
        if (_left) {
            return BlockType.LEFT;
        }
        if (_right) {
            return BlockType.RIGHT;
        }
        if (_cbl){
            return BlockType.BLEFTBACK;
        }
        if (_cbr){
            return BlockType.BRIGHTBACK;
        }
        return BlockType.FULL;

    }

   
    
    void GenBlocks(){
        for (int row=0; row<mapSize; row++){
            for(int col=0; col<mapSize; col++){
                //calculate type of block
                BlockType _thisType = BlockType.FULL;
                if (tileTraits[col,row].tileType == TileType.WATER || tileTraits[col,row].waterStrength >= 1){
                    if ( row +1 < genOptions.mapSize && tileTraits[col,row+1].tileType != TileType.WATER){
                        _thisType = BlockType.WATERTOP;
                    }else{
                        _thisType = BlockType.WATER;
                    }
                    colMap.SetTile(new Vector3Int(col,row,0), colTile);
                    waterMap.SetTile(new Vector3Int(col,row,0), tileArray[BlockPicker.GetBlockIndex(_thisType)]);
                    continue;
                }else{
                    tileTraits[col,row].tileType = TileType.GRASS;
                    TileType[,] _x3 = new TileType[3,3];
                    for (int i=0; i<3; i++){
                        for (int j=0; j<3; j++){
                            int _x3col = col-1+j;
                            int _x3row = row-1+i;
                            if (_x3row < 0 || _x3row > mapSize-1 || _x3col < 0 || _x3col > mapSize-1){
                                _x3[j,i] = TileType.NULL;
                                continue;
                            }
                            _x3[j,i] = tileTraits[col-1+j,row-1+i].tileType;
                        }
                    }
                    _thisType = GetBlockTypeIndex(_x3);
                }
                
                groundMap.SetTile(new Vector3Int(col,row,0), tileArray[BlockPicker.GetBlockIndex(_thisType)]);
                
            }
        }
    }


    void GenNoiseMap(){
        for (int i=0; i<mapSize; i++){
            for (int j=0; j<mapSize; j++){
                
                if (noiseMap[j,i] > genOptions.noiseWaterCap){
                    
                    tileTraits[j,i].tileType = TileType.WATER;
                }
            }
        }
    }
    public Vector2Int GetRandLandPos(){
        List<Vector2Int> landPoints = new List<Vector2Int>();
        for (int i=0;i<mapSize; i++){
            for (int j=0; j<mapSize; j++){
                if (tileTraits[j,i].tileType == TileType.GRASS){
                    landPoints.Add(new Vector2Int(j,i));
                }
            }
        }
        int _randIndex = Random.Range(0,landPoints.Count);
        return landPoints[_randIndex];

    }
    
}


public class TileTrait{
    public TileTrait(){
        tileType = TileType.NULL;
        waterStrength = 0;
    }
    public TileType tileType;
    public float waterStrength;


}



public enum TileType{
    NULL,
    GRASS,
    WATER
}