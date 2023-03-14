using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    public bool oneFrameSpawn = true;
    public int maxNegativeAlive = 50;
    public float respawnTime = 10f;
    public Vector2Int spawnRangeCount = new Vector2Int(4, 16);
    public Vector2Int firstSpawnRangeCount = new Vector2Int(4, 16);
    public GenerationNumbersDataSO dataSO;
    public ScaleFormulaSO scaleFormula;
    public Vector3 areaSize;
    public Vector3 boxCastOrientation = new Vector3(-90,-90,0);
    public float restrictedSpawnArea = 10f;
    public LayerMask filter;
    [SerializeField] private Number3D prefab;
    private uint spawnIteration = 0;
    private IDisposable spawnRepeater = null;
    private int currentNegativeCounter = 0;

    private void Start()
    {
        dataSO.Prewarm();
        StartCoroutine(DoSpawn(firstSpawnRangeCount, true));

        spawnRepeater = Observable.Timer(TimeSpan.FromSeconds(respawnTime)).Repeat().TakeUntilDestroy(gameObject).Subscribe(_ => StartCoroutine(DoSpawn(spawnRangeCount, oneFrameSpawn)));

        GameManager.Self.OnEndGame += Self_OnEndGame;
    }

    private void Self_OnEndGame(GameManager.ActorData[] obj)
    {
        spawnRepeater?.Dispose();
    }

    private IEnumerator DoSpawn(Vector2Int spawnRangeCount, bool oneFrameSpawn)
    {
        if (GameManager.IsGamePaused)
            yield break;

        int count = UnityEngine.Random.Range(spawnRangeCount.x, spawnRangeCount.y+1);
        for(int i = 0; i < count; i++)
        {
            var instance = Instantiate(prefab, transform);
            instance.transform.position = GetPosition() + new Vector3(0,-1000,0);
            instance.transform.rotation = Quaternion.Euler(boxCastOrientation);
            var dt = dataSO.GetRandomData(GetRandomFilter());
            instance.number = dt.number;
            instance.operation = dt.op;
            instance.size = GetSize(instance.number) * dt.sizeMultipiler;
            instance.spawnIteration = spawnIteration++;
            if(!instance.IsPositive() && instance.TryGetComponent<ItemTracker>(out var itemTracker))
            {
                currentNegativeCounter++;
                itemTracker.OnDestroyEvent += ItemTracker_OnDestroyEvent;
            }

            if(!oneFrameSpawn)
                yield return null;

            if(Physics.CheckBox(instance.transform.position + new Vector3(0, 1000, 0), instance.Bounds.extents * instance.size, instance.transform.rotation, filter.value, QueryTriggerInteraction.Collide))
            {
                Destroy(instance.gameObject);
                continue;
            }
            else
            {
                instance.transform.position += new Vector3(0, 1000.25f, 0);
            }
        }

        yield return null;
    }

    private bool? GetRandomFilter()
    {
        if (currentNegativeCounter >= maxNegativeAlive)
            return true;
        else return false;
    }

    private void ItemTracker_OnDestroyEvent(ItemTracker obj)
    {
        currentNegativeCounter--;
    }

    private Vector3 GetPosition()
    {
        Vector3 globalPosition = transform.position;
        Vector3 randomPoint = new Vector3(signedRnd * areaSize.x, signedRnd * areaSize.y, signedRnd * areaSize.z);
        var pos = transform.rotation * randomPoint + globalPosition;

        if (Vector3.Distance(transform.position, pos) <= restrictedSpawnArea)
        {
            return transform.rotation * (randomPoint.normalized * restrictedSpawnArea) + globalPosition;
        }
        else
            return pos;
    }

    private float signedRnd => UnityEngine.Random.value - 0.5f;

    public float GetSize(int number) => scaleFormula.CalculateScale(number);
    //public SizeLevel GetRandomSize() => sizeLevelList.GetRandomSize();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, areaSize);
        if (restrictedSpawnArea > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, restrictedSpawnArea);
        }
    }       
}
