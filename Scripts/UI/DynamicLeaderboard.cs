using Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DynamicLeaderboard : UnitySingleton<DynamicLeaderboard>
{
    public int printPositions = 3;
    public int minPlayerPosForImageShift = 8;
    public Vector3 deltaImgPerPlace = new Vector3(-50, 0, 0);
    [SerializeField] private LeaderboardPosition prefab;
    [SerializeField] private Transform container;
    private Dictionary<IActor, LeaderboardPosition> map;
    private List<LeaderboardPosition> sortList = new List<LeaderboardPosition>(16);

    protected override void SingletonAwake()
    {
        map = new Dictionary<IActor, LeaderboardPosition>();
    }

    public void RegisterNew(IActor actor)
    {
        var instance = Instantiate(prefab, container);
        instance.SetColor(actor.color);
        instance.actorName = actor.name;
        instance.actorScore = new Score(actor);

        map.Add(actor, instance);
        actor.OnUpdateScore += UpdateActorData;
        actor.transform.GetComponent<EXPProvider>().OnUpdateLevel += DynamicLeaderboard_OnUpdateLevel;
        //actor.OnEated += ActorEated;

        UpdateLeaderboard();
    }

    private void DynamicLeaderboard_OnUpdateLevel(EXPProvider arg1, int arg2)
    {
        UpdateActorData(arg1.GetComponent<IActor>(), false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LeaderboardPosition GetBy(IActor key)
    {
        if (map.TryGetValue(key, out var position))
        {
            return position;
        }
        else return null;
    }

    private void UpdateActorData(IActor actor, bool _)
    {
        var score = actor.CurrentScore;
        var name = actor.name;
        var lbPos = GetBy(actor);
        lbPos.actorName = actor.name;
        lbPos.actorScore = new Score(actor);

        UpdateLeaderboard();
    }

    private void ActorEated(IActor actor)
    {
        var lbPos = GetBy(actor);
        map.Remove(actor);
       
        actor.OnUpdateScore -= UpdateActorData;
        actor.transform.GetComponent<EXPProvider>().OnUpdateLevel -= DynamicLeaderboard_OnUpdateLevel;

        lbPos.transform.DOMoveX(-250, 0.5f).OnComplete(() => 
        {
            Destroy(lbPos.gameObject);
            UpdateLeaderboard();
        });
    }

    private void UpdateLeaderboard()
    {
        var lbps = map.Values;
        sortList.Clear();
        sortList.AddRange(lbps);
        sortList.Sort((x, y) => 
        {
            if (x.actorScore > y.actorScore)
                return -1;
            else if (x.actorScore < y.actorScore)
                return 1;
            else return 0;
        });
        int lbPosIndex = 1;
        bool playerInTop3 = PlayerInTop3Place();
        int actualPositions = printPositions;
        foreach(var lb in sortList)
        {
            if (lbPosIndex <= actualPositions)
            {
                lb.actorPosition = lbPosIndex++;
                lb.transform.SetAsLastSibling();
                lb.UpdateText();
                lb.ShiftImageElement(deltaImgPerPlace * (lb.actorPosition));
            }
            else
            {
                lb.Hide();
            }
        }

        if(!playerInTop3)
        {
            var lbPlayer = GetPlayerLB();
            if (lbPlayer == null)
                return;

            lbPlayer.transform.SetAsLastSibling();
            lbPlayer.UpdateText();
            var pos = lbPlayer.actorPosition > minPlayerPosForImageShift ? minPlayerPosForImageShift : lbPlayer.actorPosition;
            lbPlayer.ShiftImageElement(deltaImgPerPlace * pos);
        }
    }

    private bool PlayerInTop3Place()
    {
        string plName = Player.ActiveInstance.name;
        int amount = printPositions > sortList.Count ? sortList.Count : printPositions;
        for (int i = 0; i < amount; i++)
        {
            if (sortList[i].actorName == plName)
                return true;
        }

        return false;
    }

    private LeaderboardPosition GetPlayerLB()
    {
        string plName = Player.ActiveInstance.name;
        var lbIndex = sortList.FindIndex(x => x.actorName == plName);
        if (lbIndex < 0)
            return null;

        var lb = sortList[lbIndex];
        lb.actorPosition = lbIndex + 1;
        return lb;
    }
}
