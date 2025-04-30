namespace Orbit9000.EngineTerminal
{
    internal class MaxValueAttribute : Attribute
    {
        public MaxValueAttribute(int max)
        {
            Max = max;
        }

        public int Max { get; }  
    }
}