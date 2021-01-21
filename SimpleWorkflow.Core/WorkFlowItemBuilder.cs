using System;

namespace SimpleWorkflow.Core
{
    public class WorkFlowItemBuilder<TWFStates, TWFConditions> where TWFStates : new()
                                                          where TWFConditions : new()
    {

        private WorkFlowTransition _wfItem;
        public WorkFlowItemBuilder()
        {
            _wfItem = new WorkFlowTransition();
        }

        public WorkFlowItemBuilder<TWFStates, TWFConditions> If(Func<TWFConditions, TransitionItem> condition)
        {
            _wfItem.Command = condition(new TWFConditions());

            return this;
        }

        public WorkFlowItemBuilder<TWFStates, TWFConditions> From(Func<TWFStates, TransitionItem> currentState)
        {
            _wfItem.CurrentState = currentState(new TWFStates());

            return this;
        }

        public WorkFlowTransition GoTo(Func<TWFStates, TransitionItem> nextState)
        {
            _wfItem.NextState = nextState(new TWFStates());

            return _wfItem;
        }


    }

}
