using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Windows;

namespace AI
{
    public class BotAIState_Enter : State<BotData>
    {
        public BotAIState_Enter() : base(nameof(BotAIState_Enter))
        {
        }

        protected override void OnEnter()
        {
            Observable.Timer(0.5f.sec()).TakeUntilDestroy(Data.gameObject).Subscribe(_ => Current.SwitchTo(nameof(BotAIState_Idle)));
        }
    }

    public class BotAIState_Exit : State<BotData>
    {
        public BotAIState_Exit() : base(nameof(BotAIState_Exit))
        {
        }
    }

    public class BotAIState_Idle : State<BotData>
    {
        public BotAIState_Idle() : base(nameof(BotAIState_Idle))
        {
        }
        
        protected override void OnEnter()
        {
            Observable.Timer(Random.Range(0.2f,0.8f).sec()).TakeUntilDestroy(Data.gameObject).Subscribe(_ => Current.SwitchTo(nameof(BotAIState_PatternSelector)));
        }
    }

    public class BotAIState_SeekNumber : State<BotData>
    {
        private ItemTracker[] _items = new ItemTracker[32];
        private readonly Operation[] filter = new Operation[]
            {
                Operation.Summary,
                Operation.Multiplicate,
            };

        public BotAIState_SeekNumber() : base(nameof(BotAIState_SeekNumber))
        {
        }

        protected override void OnEnter()
        {
            var position = Data.transform.position;
            Vector2Int rangeToFind = new Vector2Int(1, 1000);

            var count = ItemsPresenter.Self.GetItemsNoAlloc(rangeToFind, filter, _items);

            ItemTracker selected = null;
            float selectedDistance = float.MaxValue;
            for(int i = 0; i < count; i++)
            {
                var currentDistance = Vector3.Distance(position, _items[i].transform.position);
                if(currentDistance < selectedDistance)
                {
                    selected = _items[i];
                    selectedDistance = currentDistance;
                }
            }

            Data.currentTarget = selected;
            if (selected != null)
                Observable.NextFrame().Subscribe(_ => Current.SwitchTo(nameof(BotAIState_GoToTarget)));
            else
                Observable.NextFrame().Subscribe(_ => Current.SwitchTo(nameof(BotAIState_EnemyHunting)));
        }
    }

    public class BotAIState_GoToTarget : State<BotData>
    {
        public BotAIState_GoToTarget() : base(nameof(BotAIState_GoToTarget))
        {
        }

        private bool triggerExit = false;

        protected override void OnEnter()
        {
            triggerExit = false;
            Data.currentTarget.OnDestroyEvent += CurrentTarget_OnDestroyEvent;
        }

        protected override void Update()
        {
            if (Data.currentTarget != null)
            {
                var dir = Data.currentTarget.transform.position - Data.transform.position;
                dir.y = 0;
                dir.Normalize();

                var vel = Data.rigidbody.velocity;
                var velY = vel.y;
                vel = Data.Speed() * dir;
                vel.y = velY;

                Data.rigidbody.velocity = vel;
            }
            else if(!triggerExit)
            {
                Current.SwitchTo(nameof(BotAIState_Idle));
                triggerExit = true;
            }
        }

        private void CurrentTarget_OnDestroyEvent(ItemTracker obj)
        {
            Current.SwitchTo(nameof(BotAIState_Idle));
        }

        protected override void OnExit()
        {
            if (Data.currentTarget != null)
                Data.currentTarget.OnDestroyEvent -= CurrentTarget_OnDestroyEvent;
        }
    }

    public class BotAIState_PatternSelector : State<BotData>
    {
        public BotAIState_PatternSelector() : base(nameof(BotAIState_PatternSelector))
        { }

        protected override void OnEnter()
        {
            var rnd = Random.value;
            if(rnd > 0.1f)
                Current.SwitchTo(nameof(BotAIState_SeekNumber));
            else
                Current.SwitchTo(nameof(BotAIState_EnemyHunting));
        }
    }

    public class BotAIState_EnemyHunting : State<BotData>
    {
        public BotAIState_EnemyHunting() : base(nameof(BotAIState_EnemyHunting))
        { }

        private float huntingTime;
        private IActor target;
        private List<IActor> sortList = new List<IActor>(16);

        protected override void OnEnter()
        {
            huntingTime = Random.Range(15f, 24f);
            target = null;
            float selectedDistance = float.MaxValue;
            Vector3 botPos = Data.transform.position;

            sortList.Clear();
            sortList.AddRange(GameManager.Self.EnumerateAllAlive());
            sortList.RemoveAll(x => 
            {
                return x.CurrentLevel > Data.Body.CurrentLevel || x.CurrentScore is 1;
            });

            if (sortList.Exists(x => Player.ActiveInstance == (Object)x) && Random.value >= 0.5f)
            {
                target = Player.ActiveInstance;
            }
            else
            {
                int selfLevel = Data.Body.CurrentLevel;
                int selfScore = Data.Body.CurrentScore;

                foreach (var actor in sortList)
                {
                    if (actor == Data.Body as IActor)
                        continue;
                    if (actor.CurrentLevel == selfLevel && actor.CurrentScore >= selfScore)
                        continue;

                    var currentDistance = Vector3.Distance(botPos, actor.transform.position);
                    if (currentDistance < selectedDistance)
                    {
                        target = actor;
                        selectedDistance = currentDistance;
                    }
                }
            }

            if(target == null)
            {
                Current.SwitchTo(nameof(BotAIState_SeekNumber));
            }
            else
            {
                Data.inHunterMode = true;
                target.OnEated += Target_OnEated;
                Debug.Log($"{Data.name} start hunting on {target.name}");
            }
        }

        private void Target_OnEated(IActor obj)
        {
            Data.inHunterMode = false;
            huntingTime = 0;
        }

        protected override void Update()
        {
            if((Object)target == null)
            {
                Data.inHunterMode = false;
                Current.SwitchTo(nameof(BotAIState_Idle));
                return;
            }

            if(huntingTime <= 0 || target.CurrentLevel >= Data.Body.CurrentLevel)
            {
                if((Object)target!=null)
                {
                    target.OnEated -= Target_OnEated;
                }
                Data.inHunterMode = false;
                Current.SwitchTo(nameof(BotAIState_Idle));
                return;
            }

            var dir = target.transform.position - Data.transform.position;
            dir.Normalize();

            var vel = Data.rigidbody.velocity;
            var velY = vel.y;
            vel = Data.Speed() * dir;
            vel.y = velY;

            Data.rigidbody.velocity = vel;

            huntingTime -= Time.deltaTime;
        }
    }

    public class BotAIState_RunFromHunter : State<BotData>
    {
        public BotAIState_RunFromHunter() : base(nameof(BotAIState_RunFromHunter))
        {
            Observable.Timer(1f.sec()).Subscribe(_ => 
            Observable.Timer(0.1f.sec()).Repeat().Subscribe(_ => CheckNearestEnemy())
            );
        }

        private IActor agressor = null;

        private void CheckNearestEnemy()
        {
            if (Data == null)
                return;

            if (Data.inHunterMode)
                return;
            if (Current.ActiveState == this)
                return;

            var all = GameManager.Self.EnumerateAllAlive();
            agressor = null;
            float selectedDistance = float.MaxValue;

            int selfLevel = Data.Body.CurrentLevel;
            int selfScore = Data.Body.CurrentScore;

            foreach(var actor in all)
            {
                if (selfLevel > actor.CurrentLevel)
                    continue;
                else if (selfLevel == actor.CurrentLevel && selfScore >= actor.CurrentScore)  //include self
                    continue;

                var currentDistance = Vector3.Distance(Data.transform.position, actor.transform.position);
                if(currentDistance < selectedDistance)
                {
                    agressor = actor;
                    selectedDistance = currentDistance;
                }
            }

            if(selectedDistance <= Data.SeekRange && agressor != null)
            {
                Current.SwitchTo(nameof(BotAIState_RunFromHunter));
            }
        }

        protected override void OnEnter()
        {
            agressor.OnEated += Agressor_OnEated;        
        }

        protected override void Update()
        {
            var dir = agressor.transform.position - Data.transform.position;
            dir = -dir.normalized;

            var vel = Data.rigidbody.velocity;
            var velY = vel.y;
            vel = Data.Speed() * dir;
            vel.y = velY;

            Data.rigidbody.velocity = vel;

            if(Vector3.Distance(agressor.transform.position, Data.transform.position) > Data.SeekRange *1.1f)
            {
                Current.SwitchTo(nameof(BotAIState_Idle));
            }
        }

        private void Agressor_OnEated(IActor obj)
        {
            Current.SwitchTo(nameof(BotAIState_Idle));
        }

        protected override void OnExit()
        {
            if(agressor != null)
                agressor.OnEated -= Agressor_OnEated;   
        }
    }
}
