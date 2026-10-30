using System.Collections.Concurrent;

namespace p42BaseLib;

public class LogWorker
{
    CancellationTokenSource _cts = new CancellationTokenSource();
    Thread? _thread;
    ConcurrentQueue<string> _logQueue;
    public void Start()
    {
        if (_thread == null)
        {
            _thread = new Thread(Run);
            _thread.IsBackground = false;
            _thread.Name = nameof(LogWorker);
            _thread.Start();
        }
    }
    
    public void Stop()
    {
        _cts.Cancel();
        _thread?.Join();
    }

    
    void Run()
    {
        CancellationToken toke = _cts.Token;
        while (!toke.IsCancellationRequested)
        {
            Thread.Sleep(100);
        }
    }
}