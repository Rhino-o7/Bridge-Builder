using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    int width = 256;
    int height = 256;
    public float scale = 15f;
    public int octaves = 6;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public Vector2 seed;
    public Gradient colorGradient;
    public float[,] noiseMap;

    public void GenerateNoiseMap(int _size)
    {
        width = _size;
        height = _size;
        float tmpScale = scale * (width / 100f);
        
        seed.x = Random.Range(0,9000);
        seed.y = Random.Range(0,9000);
        
        noiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * tmpScale + seed.x;
                float yCoord = (float)y / height * tmpScale + seed.y;
                

                float perlinValue = 0f;
                float amplitude = 1f;
                float frequency = 1f;

                for (int i = 0; i < octaves; i++)
                {
                    perlinValue += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = perlinValue;
            }
        }

        //VisualizeNoiseMap(noiseMap);
    }

    void VisualizeNoiseMap(float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point; // Set filter mode to point for pixel-perfect rendering

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the noise value in the range [0, 1]
                float noiseValue = noiseMap[x, y];
                // Map the noise value to the gradient color
                Color color = colorGradient.Evaluate(noiseValue);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        // Create a SpriteRenderer component and attach the texture
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        // Set the sorting order to ensure the sprite is visible
        spriteRenderer.sortingOrder = 0;
    }
}

