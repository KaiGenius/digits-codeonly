using System;
using StateMachine.Abstract;

namespace StateMachine
{
    public class StateMachine<TData> : BaseState.BaseStateMachine where TData : class
    {
        /// <summary>
        /// ����� ������ ��� ������� ���� ����� ������
        /// </summary>
        public TData sharedData;
        /// <summary>
        /// ������� �������� �����. ���� ����� ������ �� ��������, ������ <see cref="null"/>
        /// </summary>
        public new State<TData> ActiveState => base.ActiveState as State<TData>;
        /// <summary>
        /// ���������� �������� �����
        /// </summary>
        public new State<TData> PreviouslyState => base.PreviouslyState as State<TData>;
        /// <summary>
        /// ����� ��� ����� ������ � ����� ������, ��� 1�� �������� ��� � ����� ����� ������ ��� ������ �����, � 2�� �� ����� ����� ������������� ����� ������
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
        /// ����� ������ � ������� �������� ���� �����
        /// </summary>
        protected new StateMachine<TData> Current => base.Current as StateMachine<TData>;
        /// <summary>
        /// ����� ������ ��� ���� ������� ����� ������
        /// </summary>
        protected TData Data => Current.sharedData;

        public State(string stateName) : base(stateName)
        {

        }
    }
}
