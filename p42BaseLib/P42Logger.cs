using p42BaseLib.Interfaces;


namespace p42BaseLib;

public class P42Logger() : IP42Logger
{
    bool Production { get; set; } 
    FixedSizeQueue<string> _logQueue = new(200);
    FixedSizeQueue<string> _errorQueue = new(500);

    public void Log(string message)
    {
        Console.WriteLine(message);
        _logQueue.Enqueue(message);
    }

    public void Debug(string message)
    {
        if (Production) return;
        Console.WriteLine(message);
    }

    public void Error(string message)
    {
        Console.WriteLine(message);
        _errorQueue.Enqueue(message);
        // should be stored somewhere at least or better someone gets notified ;)
    }

    public void Info(string message)
    {
        Console.WriteLine(message);
    }
}