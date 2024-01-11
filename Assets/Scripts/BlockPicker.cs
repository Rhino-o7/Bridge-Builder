using UnityEngine;

public static class BlockPicker
{
    [SerializeField] static int varientCols = 5;
 
    public static int GetBlockIndex(BlockType _type){
        int blockRow = (int) _type;
        int blockCol = Random.Range(0,varientCols); //Random Varient of block
        int shapeIndex = (blockRow * varientCols) + blockCol;
        return shapeIndex; 
    }
    
}

public enum BlockType{
    FULL,
    TOP,
    BOTTOM,
    TOP_BOTTOM,
    LEFT,
    RIGHT,
    LEFT_RIGHT,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
    TOP_ROUND,
    BOTTOM_ROUND,
    RIGHT_ROUND,
    LEFT_ROUND,
    FULL_ROUND,
    BRIGHTBACK,
    BLEFTBACK,
    WATER,
    WATERTOP
}
