namespace Logic.Exceptions
{
    internal class NoAccessException : Exception
    {
        public NoAccessException() : base("This user has no rights to do this")
        {
        }
    }
}
