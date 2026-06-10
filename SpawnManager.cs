using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] lixoPrefabs;
    public float intervalo = 2.5f;
    public float xRange = 2.2f;

    void Start()
    {
        if (lixoPrefabs.Length > 0) StartCoroutine(GerarObjetos());
    }

    IEnumerator GerarObjetos()
    {
        while (true) {
            yield return new WaitForSeconds(intervalo);
            int index = Random.Range(0, lixoPrefabs.Length);
            Vector3 posicao = new Vector3(Random.Range(-xRange, xRange), 6f, 0f);
            Instantiate(lixoPrefabs[index], posicao, Quaternion.identity);
        }
    }
}