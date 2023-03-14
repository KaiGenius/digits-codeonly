using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BotSpawner : MonoBehaviour
{
    [SerializeField] private GameObject botPrefab;
    [SerializeField] private int botsCount = 6;
    [SerializeField] private Vector2 botSpawnAreaCircle = new Vector2(5, 10);
    [SerializeField] private List<string> botNames;
    private List<string> botNames_cash;

    IEnumerator Spawn(int count)
    {
        Quaternion rotation = Quaternion.identity;
        Vector3 tangent = Vector3.forward;

        float angleStep = 360f / count;

        for(int i =0; i < count; i++)
        {
            rotation = Quaternion.Euler(0, angleStep * i, 0);
            tangent = rotation * Vector3.forward;

            DoSpawnOperation(GetSpawnPosition(tangent) + new Vector3(0,1,0));

            yield return null;
        }
    }

    private Vector3 GetSpawnPosition(in Vector3 tangent)
    {
        return transform.position + tangent * UnityEngine.Random.Range(botSpawnAreaCircle.x, botSpawnAreaCircle.y);
    }

    private void DoSpawnOperation(Vector3 spawnPos)
    {
        var bot = Instantiate(botPrefab, transform, true);
        bot.gameObject.name = GetRandomName();
        bot.transform.position = spawnPos;
        bot.GetComponent<IActor>().OnKilled += BotSpawner_OnEated;
    }

    private void BotSpawner_OnEated(IActor obj)
    {
        if (!Application.isPlaying)
            return;
        obj.transform.GetComponent<BotExpProvider>().DecreaseLevel(3);
        obj.transform.gameObject.SetActive(false);
        obj.transform.position = GetSpawnPosition(Quaternion.Euler(0, Random.Range(0,360), 0) * Vector3.forward) + Vector3.up;
        obj.ResetScore(false);
        Observable.Timer(Random.Range(5f, 12f).sec()).TakeUntilDestroy(obj.transform.gameObject).Subscribe(_ => obj.transform.gameObject.SetActive(true));
    }

    private void Start()
    {
        botNames_cash = new List<string>(botNames);
        StartCoroutine(Spawn(botsCount));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, botSpawnAreaCircle.x);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, botSpawnAreaCircle.y);

        if (botSpawnAreaCircle.y <= botSpawnAreaCircle.x)
            botSpawnAreaCircle.y = botSpawnAreaCircle.x + 1f;
    }

    private string GetRandomName()
    {
        var index = Random.Range(0, botNames_cash.Count);
        var output = botNames_cash[index];
        botNames_cash.RemoveAt(index);
        return output;
    }
}
