namespace ORBIT9000.Core.Environment
{
    public static class AppEnvironment
    {
        public static bool IsDebug { get; private set; }

        static AppEnvironment()
        {
            #if DEBUG
            IsDebug = true;
            #else
            IsDebug = false;
            #endif
        }

        public static void SetDebug(bool value)
        {
            IsDebug = value;
        }
    }
}
