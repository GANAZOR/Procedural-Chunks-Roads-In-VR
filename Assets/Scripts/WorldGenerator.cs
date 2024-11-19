using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject chunkPrefab;  
    public int numChunks = 5;
    public int chunkSize = 13;
    public int seed = 12345;
    private bool isFirstCall = true;
    private Vector3 LastChunkPosition;
    private System.Random randomGenerator;
    private int lastRandomIndex;

    void Start()
    {
        randomGenerator = new System.Random(seed);
        GenerateWorld();
    }

    private void GenerateWorld() //Instancia los chunks en las posiciones deseadas
    {
        for (int x = 0; x < numChunks; x++)
        {


            GameObject chunkObject = Instantiate(chunkPrefab, NextPositionChunkCalculator(), Quaternion.identity);

            Chunk chunk = chunkObject.GetComponent<Chunk>();

            int offset = chunkSize;

            // Llamar a GenerateChunk con la semilla
            chunk.GenerateChunk(offset, seed);
        }
    }

    private Vector3 NextPositionChunkCalculator()
    {
        if (isFirstCall)
        {
            LastChunkPosition = new Vector3(0, 0, 0);
            isFirstCall = false;

            return new Vector3(0, 0, 0); // Posición primer chunk
        }
        else
        {
            string randomDirection = GetRandomDirection(); //Elegir hacia donde ira el siguiente chunk

            if (randomDirection == "up")
            {
                LastChunkPosition = LastChunkPosition + new Vector3(chunkSize, 0, 0);
                return LastChunkPosition;
            }
            else if (randomDirection == "down")
            {
                LastChunkPosition = LastChunkPosition + new Vector3(-chunkSize, 0, 0);
                return LastChunkPosition;
            }
            else if (randomDirection == "left")
            {
                LastChunkPosition = LastChunkPosition + new Vector3(0, 0, -chunkSize);
                return LastChunkPosition;
            }
            else
                return new Vector3(0, 0, 0);
        }
    }

    private string GetRandomDirection() //Randomizador condicionado
    {
        string[] directions = { "up", "down", "left" };

        if (lastRandomIndex == 2)
        {
            int randomIndex = randomGenerator.Next(0, 2);
            lastRandomIndex = randomIndex;

            return directions[randomIndex];
        }
        else
        {
            lastRandomIndex = 2;
            return directions[2];
        }
    }
}
