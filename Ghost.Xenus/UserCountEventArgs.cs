namespace Ghost.Xenus
{
    public class UserCountEventArgs
    {
        public int Count { get; }
        
        internal UserCountEventArgs(int count)
            => Count = count;
    }
}
