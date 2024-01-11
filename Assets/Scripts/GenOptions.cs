using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName ="Gen Options")]
public class GenOptions : ScriptableObject
{
    [Header("Map Options")]
    public int mapSize = 100;
    public float noiseWaterCap = 1f;

}
