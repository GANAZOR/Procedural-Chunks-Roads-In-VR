using System.Collections.Generic;
using UnityEngine;

public class RoadMaker : MonoBehaviour
{
    private List<Chunk> chunks;
    private int chunkSize;

    private float raycastRadius = 0.1f;
    private bool isFirstChunk = true;
    private Vector3 lastPathPosition;

    public void Initialize(List<Chunk> chunks, int chunkSize)
    {
        this.chunks = chunks;
        this.chunkSize = chunkSize;
    }

    public void GenerateRoads()
    {
        if (chunks == null || chunks.Count < 2)
        {
            Debug.Log("No hay suficientes chunks para generar caminos.");
            return;
        }

        lastPathPosition = GetChunkCenter(chunks[0]);

        foreach (Chunk chunk in chunks)
        {
            Vector3 currentChunkCenter = GetChunkCenter(chunk);

            GeneratePathBetweenPoints(chunk, new Vector3(lastPathPosition.x, 1, lastPathPosition.z), new Vector3(currentChunkCenter.x, 1, currentChunkCenter.z));
        }

        foreach (Chunk chunk in chunks)
        {
            chunk.CombineMeshes(); // Combinar meshes del chunk
        }
    }

    private Vector3 GetChunkCenter(Chunk chunk)
    {
        if (isFirstChunk) //Centro real del chunk
        {
            isFirstChunk = false;
            return chunk.transform.position + new Vector3(chunkSize / 2, 0, chunkSize / 2);
        }
        else //Random controlado (obligado a pasar por una matriz central de chunkSize/2)
        {
            float subChunkSize = chunkSize / 2;

            float centerX = chunk.transform.position.x + chunkSize / 2;
            float centerZ = chunk.transform.position.z + chunkSize / 2;

            float randomX = Random.Range(centerX - subChunkSize / 2, centerX + subChunkSize / 2);
            float randomZ = Random.Range(centerZ - subChunkSize / 2, centerZ + subChunkSize / 2);

            return new Vector3(randomX, 0, randomZ);
        }
    }

    private void GeneratePathBetweenPoints(Chunk chunk, Vector3 start, Vector3 end) //Detectar y apagar por raycast los cubos entre el inicio y final
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        RaycastHit[] hits = Physics.SphereCastAll(start, raycastRadius, direction, distance);

        Debug.Log("Hits encontrados: " + hits.Length);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;

            hitObject.SetActive(false);

            lastPathPosition = hitObject.transform.position;
        }
    }
}