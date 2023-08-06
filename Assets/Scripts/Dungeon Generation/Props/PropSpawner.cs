using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PropSpawner : MonoBehaviour
{
    [SerializeField]
    float[] percentages;
    [SerializeField]
    GameObject[] props;

    [SerializeField]
    Vector3 spawnPositionMin;
    [SerializeField]
    Vector3 spawnPositionMax;

    [SerializeField]
    Vector3 spawnRotationMin;
    [SerializeField]
    Vector3 spawnRotationMax;

    void SpawnProps()
    {
        PhotonNetwork.InstantiateRoomObject(Path.Combine("Prefabs", "Briefcase Chest"), GetRandomSpawnPosition(spawnPositionMin, spawnPositionMax), GetRandomSpawnRotation(spawnRotationMin, spawnRotationMax));
    }

    int GetRandomPropIndex()
    {
        float randomNum = Random.Range(0f, 1f);
        float numForAdding = 0;
        float total = 0;
        for(int i = 0; i < percentages.Length; i++)
        {
            total += percentages[i];
        }

        for (int i = 0; i < props.Length; i++)
        {
            if (percentages[i] / total + numForAdding >= randomNum)
                return i;
            else
                numForAdding += percentages[i] / total;
        }
        return 0;
    }

    Vector3 GetRandomSpawnPosition(Vector3 spawnPositionMin, Vector3 spawnPositionMax)
    {
        Vector3 position = transform.position;
        float randomX = Random.Range(spawnPositionMin.x, spawnPositionMax.x) + position.x;
        float randomY = Random.Range(spawnPositionMin.y, spawnPositionMax.y) + position.y;
        float randomZ = Random.Range(spawnPositionMin.z, spawnPositionMax.z) + position.z;
        return new Vector3(randomX, randomY, randomZ);
    }

    Quaternion GetRandomSpawnRotation(Vector3 spawnRotationMin, Vector3 spawnRotationMax)
    {
        float randomX = Random.Range(spawnRotationMin.x, spawnRotationMax.x);
        float randomY = Random.Range(spawnRotationMin.y, spawnRotationMax.y);
        float randomZ = Random.Range(spawnRotationMin.z, spawnRotationMax.z);
        return Quaternion.Euler(randomX, randomY, randomZ);
    }

}
