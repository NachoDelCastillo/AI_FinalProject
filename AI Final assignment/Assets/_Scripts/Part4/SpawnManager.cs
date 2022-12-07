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

        StartCoroutine(InstantiateAtBeginning());
    }

    IEnumerator InstantiateAtBeginning()
    {
        int _column;
        do
        {
            _column = Random.Range(0, spawnPoints.GetLength(0));
            yield return null;
        }
        while (itemsInColumn[_column] == maxItemsPerColumn);

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

        bool continueCo = true;
        for (int i = 0; i < itemsInColumn.Length; i++)
        {
            if (itemsInColumn[i] < maxItemsPerColumn) break;
            if(i == itemsInColumn.Length - 1) continueCo = false;
        }

        if (continueCo) { yield return new WaitForSeconds(Random.Range(timerMinMax.x, timerMinMax.y)); StartCoroutine(InstantiateAtBeginning()); }
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
    }

    IEnumerator InstantiateItemInRandomRowCo(int _column, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

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
    }

    public void PickUpItem(int _column)
    {
        StartCoroutine(InstantiateItemInRandomRowCo(_column, Random.Range(timerMinMax.x, timerMinMax.y)));
    }
}
