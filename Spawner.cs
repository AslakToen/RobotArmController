using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] cubes;
    public Vector3 spawnValues;
    public float spawnWait;
    public float spawnMostWait;
    public float spawnLeastWait;
    public int startWait;
    public bool stop;
    public int x;
    public int maxval;

    int randcube;


    // float totalLength = 1.246f;
    float totalLength = 1.899f;
    float limit;


    void Start()
    {
        StartCoroutine(waitSpawner());
        limit = totalLength / 1.414f;
    }

    void Update()
    {
        spawnWait = 1;
    }

    IEnumerator waitSpawner()
    {
        yield return new WaitForSeconds(startWait);
        // randcube = Random.Range(0, 2);

        while (x < maxval)
        {
            
            Vector3 spawnPosition = new Vector3(
                /*
                Random.Range(-spawnValues.x, spawnValues.x),
                Random.Range(0, spawnValues.y),
                Random.Range(-spawnValues.z, spawnValues.z)
                */
                Random.Range(-limit, limit),
                Random.Range(0.1f, limit),
                Random.Range(-limit, limit)
            );

            Instantiate(
                cubes[0],
                spawnPosition + transform.TransformPoint(0, 0, 0),
                gameObject.transform.rotation
            );

            yield return new WaitForSeconds(spawnWait);
            x = x + 1;
        }
    }
}