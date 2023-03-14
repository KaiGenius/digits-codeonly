using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 size = new Vector3(200, 0, 200);
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Vector3 cellSize = new Vector3(14, 0, 14);
    [SerializeField] private Vector3 positionFix = new Vector3(0.95f, 0, 1.02f);

    int counter = 0;
    private void Start()
    {
        var halfSize = size * 0.5f;
        for (float x = -halfSize.x; x <= halfSize.x; x += cellSize.x)
        {
            for (float z = -halfSize.z; z <= halfSize.z; z += cellSize.z)
            {
                var pos = new Vector3(x, 0, z);
                var instance = Instantiate(towerPrefab, pos + positionFix, Quaternion.identity, transform);
                counter++;
            }
        }

        Debug.Log($"Towers count: {counter}");
    }
}
