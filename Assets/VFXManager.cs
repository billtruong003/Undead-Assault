using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class VFXManager : MonoBehaviourPun
{
    public static VFXManager Instance { get; private set; }

    public GameObject hitVFXPrefab;
    public GameObject hitOtherVFXPrefab;

    [OdinSerialize, ReadOnly]
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void InitVFX()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        InitializePool(hitVFXPrefab, 20);
        InitializePool(hitOtherVFXPrefab, 20);
    }

    private void InitializePool(GameObject prefab, int size)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
            obj.transform.parent = transform;
        }

        poolDictionary.Add(prefab.name, objectPool);
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab.name))
        {
            Debug.LogWarning("Pool for " + prefab.name + " does not exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[prefab.name].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        StartCoroutine(ReturnToPoolAfterDelay(objectToSpawn, prefab.name, 2));

        return objectToSpawn;
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, string prefabName, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        poolDictionary[prefabName].Enqueue(obj);
    }

    public void PlayHitVFX(Vector3 position)
    {
        GetFromPool(hitVFXPrefab, position, Quaternion.identity);
    }

    public void PlayHitOtherVFX(Vector3 position)
    {
        GetFromPool(hitOtherVFXPrefab, position, Quaternion.identity);
    }
}
