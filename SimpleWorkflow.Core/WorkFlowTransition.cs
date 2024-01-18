using System;

namespace SimpleWorkflow.Core
{

    public class WorkFlowTransition
    {
        public TransitionItem CurrentState { get; set; }
        public TransitionItem NextState { get; set; }
        public TransitionItem Command { get; set; }

        public override int GetHashCode()
        {
            return ConcatCurrentStateAndCommand();
        }

        public override bool Equals(object obj)
        {

            return Equals(obj as WorkFlowTransition);
        }

        public bool Equals(WorkFlowTransition obj)
        {
            return obj != null && obj.ConcatCurrentStateAndCommand() == this.ConcatCurrentStateAndCommand();
        }

        //Returns xy like: x=1,y=2 then xy=12
        public int ConcatCurrentStateAndCommand()
        {
            int x = CurrentState.Value;
            int y = Command.Value;
            if (y < 10) return x * 10 + y;
            if (y < 100) return x * 100 + y;
            if (y < 1000) return x * 1000 + y;
            if (y < 10000) return x * 10000 + y;

            return x * (int)Math.Pow(10, Math.Floor(Math.Log(y, 10)) + 1) + y;
        }
    }

}
