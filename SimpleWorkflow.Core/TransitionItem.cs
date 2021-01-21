namespace SimpleWorkflow.Core
{
    public abstract class TransitionItem
    {
        public abstract int Value { get; }
        public abstract string Name { get; }

        public override int GetHashCode()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {

            return Equals(obj as TransitionItem);
        }

        public bool Equals(TransitionItem obj)
        {
            return obj != null && obj.Value == this.Value;
        }


    }

}
