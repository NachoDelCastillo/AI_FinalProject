using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    Transform[,] spawnPoints;

    [SerializeField] int maxItemsPerColumn;
    int[] itemsInColumn;

    [SerializeField] GameObject ammo, gun;

    [SerializeField] Vector2 timerMinMax;
    float timer, spawnCD;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = new Transform[transform.childCount, transform.GetChild(0).childCount];
        itemsInColumn = new int[transform.childCount];

        for (int column = 0; column < spawnPoints.GetLength(0); column++)
        {
            for (int row = 0; row < spawnPoints.GetLength(1); row++)
            {
                spawnPoints[column, row] = transform.GetChild(column).GetChild(row);
            }
        }

        spawnCD = Random.Range(timerMinMax.x, timerMinMax.y);
        //for (int column = 0; column < spawnPoints.GetLength(0); column++)
        //{
        //    while (itemsInColumn[column] < maxItemsPerColumn)
        //    {
        //        InstantiateItemInRandomRow(column);
        //    }
        //}
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnCD)
        {
            int column = Random.Range(0, spawnPoints.GetLength(0));
            Debug.Log(column);
            if (itemsInColumn[column] >= 2) return;

            StartCoroutine(InstantiateItemInRandomRowCo2(column));
            timer = 0;
            spawnCD = Random.Range(timerMinMax.x, timerMinMax.y);
            Debug.Log(spawnCD);
        }
    }

    void InstantiateItemInRandomRow(int _column)
    {
        int _row;
        do
        {
            _row = Random.Range(0, spawnPoints.GetLength(1));
        }
        while (spawnPoints[_column, _row].childCount > 0);

        GameObject clon;
        if (Random.Range(0, 2) == 0) clon = Instantiate(ammo, spawnPoints[_column, _row]);
        else clon = Instantiate(gun, spawnPoints[_column, _row]);

        clon.GetComponent<Item>().column = _column;
        clon.GetComponent<Item>().spawnManager = this;
        itemsInColumn[_column]++;
    }

    IEnumerator InstantiateItemInRandomRowCo2(int _column)
    {
        int _row;
        do
        {
            _row = Random.Range(0, spawnPoints.GetLength(1));
            yield return null;
        }
        while (spawnPoints[_column, _row].childCount > 0);

        GameObject clon;
        if (Random.Range(0, 2) == 0) clon = Instantiate(ammo, spawnPoints[_column, _row]);
        else clon = Instantiate(gun, spawnPoints[_column, _row]);

        clon.GetComponent<Item>().column = _column;
        clon.GetComponent<Item>().spawnManager = this;
        itemsInColumn[_column]++;
    }

    IEnumerator InstantiateItemInRandomRowCo(int _column, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        InstantiateItemInRandomRow(_column);
    }

    public void PickUpItem(int _column)
    {
        itemsInColumn[_column]--;

        StartCoroutine(InstantiateItemInRandomRowCo(_column, Random.Range(2, 5)));
    }
}
