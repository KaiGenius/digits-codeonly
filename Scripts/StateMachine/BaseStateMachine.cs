using System;
using System.Collections.Generic;

namespace StateMachine.Abstract
{
    public abstract class BaseState
    {
        /// <summary>
        /// ��� ������. ������ ���� ����������
        /// </summary>
        public readonly string name;
        private BaseStateMachine _stateMachine;
        private bool _isActive = false;
        /// <summary>
        /// ������� �� ������ ����� � ����� ������?
        /// </summary>
        public bool IsActive => _isActive;
        protected BaseStateMachine Current => _stateMachine;
        /// <summary>
        /// ������� ����� ����� ������ ������������� �� ���� �����
        /// </summary>
        protected virtual void OnEnter() { }
        /// <summary>
        /// ������� ����� ����� ������ ������ � ����� ������ ��� ���������������
        /// </summary>
        protected virtual void OnExit() { }
        /// <summary>
        /// ������� ���������� ������ �� ����� ������ <see cref="BaseStateMachine.Update"/> ���� ������ ����� �������
        /// </summary>
        protected virtual void Update() { }

        private void SetMachine(BaseStateMachine machine)
        {
            if(_isActive)
            {
                ExitThisState();
            }
            _stateMachine = machine;
        }

        private void EnterThisState()
        {
            _isActive = true;
            OnEnter();
        }

        private void ExitThisState()
        {
            OnExit();
            _isActive = false;
        }

        public BaseState(string stateName)
        {
            this.name = stateName;
        }

        public abstract class BaseStateMachine
        {
            protected readonly Dictionary<string, BaseState> map = new Dictionary<string, BaseState>(8);
            private BaseState _currentRunning, _previouslyRunning;
            protected BaseState ActiveState => _currentRunning;
            protected BaseState PreviouslyState => _previouslyRunning;
            /// <summary>
            /// ������� �� ����� ������?
            /// </summary>
            public bool IsRunning => _currentRunning != null;

            /// <summary>
            /// ��������� ����� ����� � ����� ������. ����� �����, ��� ���� ����� ��� ����������� � ����� ���� ����� ������ - �������� �� ���������� � �������� false ���� ���� ���� ����� ��� ����������� � ������� ����� ������
            /// </summary>
            /// <param name="newState">����� �����. ��� ������ ������ ���� ����������, ����� �� �� ��������� � ����� ������</param>
            /// <returns>������ �� �������� �������?</returns>
            public bool AddNewState(BaseState newState)
            {
                if (map.ContainsKey(newState.name))
                    return false;
                if (newState._stateMachine != null)
                    return false;
                else
                {
                    map.Add(newState.name, newState);
                    newState.SetMachine(this);
                    return true;
                }
            }
            /// <summary>
            /// ������� ������������ ����� �� ����� ������. ���� ����� ������� - �� ���������� � ����� ������ ����������
            /// </summary>
            /// <param name="stateName"></param>
            /// <returns></returns>
            public bool RemoveState(string stateName)
            {
                var result = map.Remove(stateName);
                if(result && stateName == _currentRunning.name)
                {
                    _currentRunning.SetMachine(null);
                    _currentRunning = null;
                    OnChangedState(null);
                }
                return result;
            }
            /// <summary>
            /// ���������� ����� �� ����� �� ����� ������
            /// </summary>
            /// <param name="stateName">��� ������</param>
            /// <returns></returns>
            public BaseState GetState(string stateName)
            {
                if (map.TryGetValue(stateName, out BaseState newState))
                    return newState;
                else return null;
            }
            /// <summary>
            /// ����������� ����� �� ����� � ����� ������. ���� ������ ���������, �� ��������� � � ���������� ������. ���� ������� ����� ��� ��������, �� ������ �� ������. ������ ����� �� ��������� ����� ������. ��� ����� ����������� <see cref="BaseStateMachine.Stop"/> �����. ���� ����� �� ������ ��� ��������� ����� ��� �������, �� ���������� false
            /// </summary>
            /// <param name="nextState">��� ������</param>
            /// <returns>��������� �� ������������ �������?</returns>
            public bool SwitchTo(string nextState) => SwitchTo(GetState(nextState));
            /// <summary>
            /// ����������� ����� �� ����� � ����� ������. ���� ������ ���������, �� ��������� � � ���������� ������. ���� ������� ����� ��� ��������, �� ������ �� ������. ������ ����� �� ��������� ����� ������. ��� ����� ����������� <see cref="BaseStateMachine.Stop"/> �����. ����� �����, ��� ���� �������� �����, ������� �� ����������� � ������� ����� ������, �� ����� ������� ���������� <see cref="ArgumentException"/>
            /// </summary>
            /// <param name="nextState">��������� �����</param>
            /// <returns>��������� �� ������������ �������?</returns>
            ///<exception cref="ArgumentException"></exception>
            public bool SwitchTo(BaseState nextState)
            {
                if (nextState is null || nextState == _currentRunning)
                    return false;
                if (nextState._stateMachine != this)
                    throw new ArgumentException($"This state {nextState.name} not contains in this machine");

                if(_currentRunning != null)
                {
                    _currentRunning.ExitThisState();
                }
                _previouslyRunning = _currentRunning;
                _currentRunning = nextState;
                OnChangedState(_currentRunning);
                _currentRunning.EnterThisState();
                return true;
            }
            /// <summary>
            /// ��������� ������� �������� ����� � ����� ������. ���� ����� ������ �� �������� - �� ������ �� ������
            /// </summary>
            public void Update()
            {
                if(_currentRunning != null)
                {
                    _currentRunning.Update();
                }
            }
            /// <summary>
            /// ��������� ����� ������. ���� ��� �� ������� - �� ������ �� ������
            /// </summary>
            public void Stop()
            {
                if(_currentRunning != null)
                {
                    _currentRunning.ExitThisState();
                    _previouslyRunning = _currentRunning;
                    _currentRunning = null;
                    OnChangedState(null);
                }
            }

            protected virtual void OnChangedState(BaseState newState) { }
        }
    }
}
