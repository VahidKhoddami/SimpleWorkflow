using System;
using System.Collections.Generic;

namespace SimpleWorkflow.Core
{
    public class WorkFlowBuilder<TWFStates, TWFConditions> where TWFStates : new()
                                                   where TWFConditions : new()
    {

        private Dictionary<WorkFlowTransition, WorkFlowTransition> _workFlow;

        public WorkFlowBuilder<TWFStates, TWFConditions> Add(Func<WorkFlowItemBuilder<TWFStates, TWFConditions>, WorkFlowTransition> workFlowBuilder)
        {
            if (workFlowBuilder == null)
                throw new Exception("Workflow builder is not defined!");

            var wfItem = workFlowBuilder(new WorkFlowItemBuilder<TWFStates, TWFConditions>());

            if (wfItem.Command == null)
                throw new Exception("'If' statement is not defined!");

            if (wfItem.CurrentState == null)
                throw new Exception("'From' statement is not defined!");

            if (_workFlow == null)
                InstantiateWorkflow();

            if (IsCurrentStateAdded(wfItem))
            {
                UpdateNextState(wfItem);
            }
            else
            {
                AddNewFlow(wfItem);
            }

            return this;
        }

        public WorkFlow<TWFStates, TWFConditions> Build()
        {
            return new WorkFlow<TWFStates, TWFConditions>(_workFlow);
        }
        private void UpdateNextState(WorkFlowTransition wfItem) => _workFlow[wfItem].NextState = wfItem.NextState;

        private void AddNewFlow(WorkFlowTransition wfItem) => _workFlow.Add(wfItem, wfItem);

        private bool IsCurrentStateAdded(WorkFlowTransition wfItem) => _workFlow.ContainsKey(wfItem);

        private void InstantiateWorkflow() => _workFlow = new Dictionary<WorkFlowTransition, WorkFlowTransition>();
    }

}
