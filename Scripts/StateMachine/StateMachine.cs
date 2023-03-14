using System;
using StateMachine.Abstract;

namespace StateMachine
{
    public class StateMachine<TData> : BaseState.BaseStateMachine where TData : class
    {
        /// <summary>
        /// Общие данные для стейтов этой стейт машины
        /// </summary>
        public TData sharedData;
        /// <summary>
        /// Текущий активный стейт. Если стейт машина не запущена, вернет <see cref="null"/>
        /// </summary>
        public new State<TData> ActiveState => base.ActiveState as State<TData>;
        /// <summary>
        /// Предыдущий активный стейт
        /// </summary>
        public new State<TData> PreviouslyState => base.PreviouslyState as State<TData>;
        /// <summary>
        /// Эвент при смене стейта в стейт машине, где 1ый аргумент это в какой стейт машине был вызван эвент, а 2ой на какой стейт переключилась стейт машина
        /// </summary>
        public event Action<StateMachine<TData>, State<TData>> OnStateChaged;
        protected override void OnChangedState(BaseState newState)
        {
            OnStateChaged?.Invoke(this, newState as State<TData>);
        }

        public StateMachine(TData sharedData)
        {
            this.sharedData = sharedData;
        }
    }

    public abstract class State<TData> : BaseState where TData : class
    {
        /// <summary>
        /// Стейт машина к которой присвоен этот стейт
        /// </summary>
        protected new StateMachine<TData> Current => base.Current as StateMachine<TData>;
        /// <summary>
        /// Общие данные для всех стейтов стейт машины
        /// </summary>
        protected TData Data => Current.sharedData;

        public State(string stateName) : base(stateName)
        {

        }
    }
}
