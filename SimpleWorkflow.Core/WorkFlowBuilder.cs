using System;
using System.Collections.Generic;

namespace SimpleWorkflow.Core
{
    public class WorkFlowBuilder<TWFStates, TWFCommands> where TWFStates : new()
                                                   where TWFCommands : new()
    {

        private Dictionary<WorkFlowTransition, WorkFlowTransition> _workFlow;

        public WorkFlowBuilder<TWFStates, TWFCommands> Add(Func<WorkFlowItemBuilder<TWFStates, TWFCommands>, WorkFlowTransition> workFlowBuilder)
        {
            if (workFlowBuilder == null)
                throw new Exception("Workflow builder is not defined!");

            var wfItem = workFlowBuilder(new WorkFlowItemBuilder<TWFStates, TWFCommands>());

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

        public WorkFlow<TWFStates, TWFCommands> Build()
        {
            return new WorkFlow<TWFStates, TWFCommands>(_workFlow);
        }
        private void UpdateNextState(WorkFlowTransition wfItem) => _workFlow[wfItem].NextState = wfItem.NextState;

        private void AddNewFlow(WorkFlowTransition wfItem) => _workFlow.Add(wfItem, wfItem);

        private bool IsCurrentStateAdded(WorkFlowTransition wfItem) => _workFlow.ContainsKey(wfItem);

        private void InstantiateWorkflow() => _workFlow = new Dictionary<WorkFlowTransition, WorkFlowTransition>();
    }

}
