namespace Swerva
{
    public delegate void HttpLogEvent(object message);

    public static class HttpLog
    {
        public static event HttpLogEvent Log;

        public static void WriteLine(object message)
        {
            if(Log != null)
            {
                Log(message);
            }
            else
            {
                System.Console.ForegroundColor = System.ConsoleColor.DarkGreen;
                System.Console.Write(System.DateTime.Now + " ");
                System.Console.ForegroundColor = System.ConsoleColor.White;
                System.Console.WriteLine(message);
            }
        }
    }
}