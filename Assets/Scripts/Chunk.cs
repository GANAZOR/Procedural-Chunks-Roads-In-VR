using System;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public GameObject prefabGround;
    public GameObject prefabGrass;

    public static bool isFirstChunk = true;

    public GameObject[,,] cubes; // Almacena las referencias a los cubos del chunk

    public void GenerateChunk(int offset, int seed) //Estructura Chunks
    {
        UnityEngine.Random.InitState(seed); // Usar semilla para consistencia

        cubes = new GameObject[offset, 2, offset];

        for (int x = 0; x < offset; x++)
        {
            for (int z = 0; z < offset; z++)
            {
                for (int y = 0; y < 2; y++)
                {
                    GameObject prefab = y == 0 ? prefabGround : prefabGrass;
                    GameObject cube = Instantiate(prefab, transform);

                    Vector3 localPosition = new Vector3(x, y, z);
                    cube.transform.localPosition = localPosition;

                    cubes[x, y, z] = cube;

                    cube.tag = y == 0 ? "Ground" : "Grass";
                }
            }
        }

        if (isFirstChunk)
        {
            isFirstChunk = false;
            InitialCubePath(offset);
        }
    }

    public void InitialCubePath(int offset)
    {
        int centerX = offset / 2;
        int centerZ = offset / 2;

        GameObject centerCube = cubes[centerX, 1, centerZ];  // Obtener el cubo de "Grass" en el centro
        if (centerCube != null)
        {
            centerCube.SetActive(false);  // Desactivar el cubo en lugar de destruirlo
        }
    }

    public void CombineMeshes()  // Combinar meshes por material
    {
        var combineGround = new System.Collections.Generic.List<CombineInstance>();
        var combineGrass = new System.Collections.Generic.List<CombineInstance>();

        foreach (Transform child in transform)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter == null || !child.gameObject.activeSelf) continue;  // Excluir los cubos desactivados

            CombineInstance combineInstance = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = child.localToWorldMatrix
            };

            if (child.gameObject.CompareTag("Ground"))
            {
                combineGround.Add(combineInstance);
            }
            else if (child.gameObject.CompareTag("Grass"))
            {
                combineGrass.Add(combineInstance);
            }

            child.gameObject.SetActive(false);
        }

        CreateCombinedMesh(combineGround, prefabGround.GetComponent<MeshRenderer>().sharedMaterial, "GroundMesh");
        CreateCombinedMesh(combineGrass, prefabGrass.GetComponent<MeshRenderer>().sharedMaterial, "GrassMesh");

        DestroyOriginalCubes();
    }

    private void CreateCombinedMesh(System.Collections.Generic.List<CombineInstance> combineList, Material material, string name) //Combinar meshes
    {
        if (combineList.Count == 0) return;

        GameObject meshObject = new GameObject(name);
        meshObject.transform.parent = transform;

        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineList.ToArray());
        meshFilter.mesh = combinedMesh;

        meshRenderer.material = material;

        MeshCollider meshCollider = meshObject.AddComponent<MeshCollider>(); // Agregar un MeshCollider
        meshCollider.sharedMesh = combinedMesh;
    }

    private void DestroyOriginalCubes() // Eliminar los cubos iniciales desactivados
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
