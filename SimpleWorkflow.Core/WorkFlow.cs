using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWorkflow.Core
{
    public class WorkFlow<TWFStates, TWFConditions> where TWFStates : new()
                                                   where TWFConditions : new()
    {

        private IDictionary<WorkFlowTransition, WorkFlowTransition> _workFlow;

        public WorkFlow(IDictionary<WorkFlowTransition, WorkFlowTransition> workflow)
        {
            _workFlow = workflow;
        }

        public TransitionItem GetNextState(Func<TWFStates, TransitionItem> currentStateExp, Func<TWFConditions, TransitionItem> conditionExp)
        {
            if (_workFlow == null)
                return default;

            if (conditionExp == null)
                return default;

            if (currentStateExp == null)
                return default;

            var condition = conditionExp(new TWFConditions());
            var currentState = currentStateExp(new TWFStates());

            var flowItem = new WorkFlowTransition { CurrentState = currentState, Command = condition };

            if (NextStateNotDefined(flowItem))
                return default;

            return _workFlow[flowItem].NextState;

        }

        public TransitionItem GetNextState(int state, int condition)
        {
            if (_workFlow == null)
                return default;

            return _workFlow.Values.Where(q => q.CurrentState.Value == state && q.Command.Value == condition)
                                        .Select(q => q.NextState)
                                        .FirstOrDefault();

        }

        public TransitionItem GetCommandItem(int commandValue)
        {
            if (_workFlow == null)
                return null;

            return _workFlow.Values.Where(q => q.Command.Value == commandValue)
                                  .Select(q => q.Command)
                                  .FirstOrDefault();

        }

        public TransitionItem GetStateItem(int stateValue)
        {
            if (_workFlow == null)
                return null;

            foreach (var item in _workFlow.Values)
            {
                if (item.CurrentState.Value == stateValue)
                    return item.CurrentState;
                if (item.NextState.Value == stateValue)
                    return item.NextState;
            }

            return null;
        }

        public IEnumerable<TransitionItem> GetConditions(Func<TWFStates, TransitionItem> currentStateExp = null)
        {
            if (_workFlow == null)
                return Enumerable.Empty<TransitionItem>();

            var query = _workFlow.Values.AsQueryable();
            if (currentStateExp != null)
            {
                var currentState = currentStateExp(new TWFStates());
                query = query.Where(q => q.CurrentState.Equals(currentState));
            }

            return query.Select(q => q.Command).Distinct().AsEnumerable();
        }

        private bool NextStateNotDefined(WorkFlowTransition flowItem) => !_workFlow.ContainsKey(flowItem);
    }

}
