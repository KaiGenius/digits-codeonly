using System;
using System.Collections.Generic;

namespace StateMachine.Abstract
{
    public abstract class BaseState
    {
        /// <summary>
        /// Имя стейта. Должно быть уникальным
        /// </summary>
        public readonly string name;
        private BaseStateMachine _stateMachine;
        private bool _isActive = false;
        /// <summary>
        /// Активен ли сейчас стейт в стейт машине?
        /// </summary>
        public bool IsActive => _isActive;
        protected BaseStateMachine Current => _stateMachine;
        /// <summary>
        /// Каллбек когда стейт машина переключается на этот стейт
        /// </summary>
        protected virtual void OnEnter() { }
        /// <summary>
        /// Каллбек когда стейт машина уходит с этого стейта или останавливается
        /// </summary>
        protected virtual void OnExit() { }
        /// <summary>
        /// Каллбек обновления стейта от стейт машины <see cref="BaseStateMachine.Update"/> если сейчас стейт активен
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
            /// Активна ли стейт машина?
            /// </summary>
            public bool IsRunning => _currentRunning != null;

            /// <summary>
            /// Добавляет новый стейт в стейт машину. Важно знать, что если стейт уже принадлежит к какой либо стейт машине - привязка не произойдет и вернется false даже если этот стейт уже принадлежит к текущей стейт машине
            /// </summary>
            /// <param name="newState">Новый стейт. Имя стейта должно быть уникальным, иначе он не добавится в стейт машину</param>
            /// <returns>Прошла ли операция успешно?</returns>
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
            /// Удаляет существующий стейт из стейт машины. Если стейт активен - он завершится и стейт машина выключится
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
            /// Возвращает стейт по имени из стейт машины
            /// </summary>
            /// <param name="stateName">Имя стейта</param>
            /// <returns></returns>
            public BaseState GetState(string stateName)
            {
                if (map.TryGetValue(stateName, out BaseState newState))
                    return newState;
                else return null;
            }
            /// <summary>
            /// Переключает стейт на новый в стейт машине. Если машина выключена, то запускает её с выбранного стейта. Если текущий стейт уже работает, то ничего не делает. Данный метод не выключает стейт машину. Для этого используйте <see cref="BaseStateMachine.Stop"/> метод. Если стейт не найден или выбранный стейт уже активен, то возвращает false
            /// </summary>
            /// <param name="nextState">Имя стейта</param>
            /// <returns>Выполнено ли переключение успешно?</returns>
            public bool SwitchTo(string nextState) => SwitchTo(GetState(nextState));
            /// <summary>
            /// Переключает стейт на новый в стейт машине. Если машина выключена, то запускает её с выбранного стейта. Если текущий стейт уже работает, то ничего не делает. Данный метод не выключает стейт машину. Для этого используйте <see cref="BaseStateMachine.Stop"/> метод. Важно знать, что если передать стейт, который не принадлежит к текущей стейт машине, то будет вызвано исключение <see cref="ArgumentException"/>
            /// </summary>
            /// <param name="nextState">Следующий стейт</param>
            /// <returns>Выполнено ли переключение успешно?</returns>
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
            /// Обновляет текущий активный стейт в стейт машине. Если стейт машина не запущена - то ничего не делает
            /// </summary>
            public void Update()
            {
                if(_currentRunning != null)
                {
                    _currentRunning.Update();
                }
            }
            /// <summary>
            /// Выключает стейт машину. Если она не активна - то ничего не делает
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
