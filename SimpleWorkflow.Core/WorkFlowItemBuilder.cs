using System;

namespace SimpleWorkflow.Core
{
    public class WorkFlowItemBuilder<TWFStates, TWFCommands> where TWFStates : new()
                                                          where TWFCommands : new()
    {

        private WorkFlowTransition _wfItem;
        public WorkFlowItemBuilder()
        {
            _wfItem = new WorkFlowTransition();
        }

        public WorkFlowItemBuilder<TWFStates, TWFCommands> If(Func<TWFCommands, TransitionItem> command)
        {
            _wfItem.Command = command(new TWFCommands());

            return this;
        }

        public WorkFlowItemBuilder<TWFStates, TWFCommands> From(Func<TWFStates, TransitionItem> currentState)
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
