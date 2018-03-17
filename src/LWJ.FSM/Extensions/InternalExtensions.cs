namespace LWJ.FSM
{
    internal static class InternalExtensions
    {
         
        public static string FormatArgs(this string source, params object[] args)
        {
            return string.Format(source, args);
        }
        
    }
}
