
using System.Collections;
using p42BaseLib.Interfaces;


namespace p42BaseLib;

public enum LogType
{
    Log,
    Error,
    Debug,
    Info
}

record P42LogObject
{
    LogType _logType;
    string _msg;
    DateTime _timeStamp;
    public P42LogObject(LogType logType, string message, DateTime timeStamp)
    {
        _logType = logType;
        _msg = message;
        _timeStamp = timeStamp;
    }

    public LogType GetLogType()
    {
        return _logType;
    }

    public string GetMessage()
    {
        return _msg;
    }

    public DateTime GetTimeStamp()
    {
        return _timeStamp;
    }
}

public class P42Logger : IP42Logger
{
 
    CancellationTokenSource _cts = new CancellationTokenSource();


    Thread? _wThread;
    object _wLocker = new object();
    SemaphoreSlim _wSignal = new SemaphoreSlim(0);
    
    bool _yearFolders = true;
    bool _monthFolders = true;
    bool _dayFolders = true;
    int _maxLogFiles = 999;
    int _maxErrorFiles = 999;
    bool _useUtc = false;

    int _logCounter = 1;
    int _errorCounter = 1;
    int _debugCounter = 1;
    int _infoCounter = 1;

    string _logPath = @".\Logs\Log.txt";
    string _errorPath = @".\Logs\Error.txt";
    string _debugPath = @".\Logs\Debug.txt";
    string _infoPath = @".\Logs\Info.txt";

    FixedSizeQueue<P42LogObject> _logQueue = new FixedSizeQueue<P42LogObject>(200);
    FixedSizeQueue<P42LogObject> _errorQueue = new FixedSizeQueue<P42LogObject>(500);


    bool Production { get; set; }
    void SetProduction(bool production) => Production = production;
    void SetLogQueueSize(int size) => _logQueue = new(size);
    void SetErrorQueueSize(int size) => _errorQueue = new(size);

    bool LogToFile { get; set; } = false;
    bool LogToConsole { get; set; } = true;

    public P42Logger(bool production = false) 
    {
        SetProduction(production);
        Start();
    }

    public P42Logger(bool production, bool logToFile, bool logToConsole) : this(production)
    {
        LogToFile = logToFile;
        LogToConsole = logToConsole;
    }

    public P42Logger(string logPath, string errorPath, bool production = false) : this(production)
    {
        SetLogPath(logPath);
        SetErrorPath(errorPath);
    }

    public P42Logger(string logPath, string errorPath, int logQueueSize, int errorQueueSize,
        bool production = false) : this()
    {
        SetLogPath(logPath);
        SetErrorPath(errorPath);
        SetLogQueueSize(logQueueSize);
        SetErrorQueueSize(errorQueueSize);
    }

    
    string GetLogFileName(DateTime timeStamp,LogType logType, string path)
    {
        string? dir = Path.GetDirectoryName(path);
        string? fn = Path.GetFileName(path);
        string? ext = Path.GetExtension(path);
        
        string year = _yearFolders ? timeStamp.Year.ToString("0000")+"\\" : "";
        string month = _monthFolders ? timeStamp.Month.ToString("00")+"\\" : "";
        string day = _dayFolders ? timeStamp.Day.ToString("00")+"\\" : "";
        string counter = "";
        switch (logType)
        {
            case LogType.Log:
                counter = _logCounter.ToString(@"0000");
                break;
            case LogType.Error:
                counter = _errorCounter.ToString(@"0000");
                break;
            case LogType.Debug:
                counter = _debugCounter.ToString(@"0000");
                break;
            default:
                counter = _infoCounter.ToString(@"0000");
                break;
        }
        
        string fileName = $"{dir}\\{year}{month}{day}{counter}_{fn}{ext}";
        return fileName;
    }

    void SetLogPath(string path)
    {
        _logPath = path;
    }

    void SetErrorPath(string path)
    {
        _errorPath = path;
    }

   


    public void Log(string message)
    {
        _logQueue.Enqueue(new P42LogObject(LogType.Log,message,GetTimeStamp()));
        _wSignal.Release();
    }

    public void Debug(string message)
    {
        if (Production) return;
        _logQueue.Enqueue(new P42LogObject(LogType.Debug,message,GetTimeStamp()));
        _wSignal.Release();
    }

    public void Error(string message)
    {
        _logQueue.Enqueue(new P42LogObject(LogType.Error,message,GetTimeStamp()));
        _wSignal.Release();
    }

    public void Info(string message)
    {
        _logQueue.Enqueue(new P42LogObject(LogType.Info,message,GetTimeStamp()));
        _wSignal.Release();
    }

    

    public virtual string FormatMessage(DateTime ts,LogType logType,string message)
    {
        return $"{GetTimeStampLabel(ts)} {GetLogLabel(logType)} {message}"; 
    }

    protected string GetLogLabel(LogType logType)
    {
        return $"[{logType.ToString("")}]"; 
    }
    protected string GetTimeStampLabel(DateTime ts)
    {
        //ts = _useUtc ? DateTime.UtcNow : DateTime.Now;
        string timePrefix = (_yearFolders ? "" : ts.Year.ToString(@"0000")) +
                            (_monthFolders ? "" : ts.Month.ToString(@"00")) +
                            (_monthFolders ? "" : ts.Month.ToString(@"00 "));
        return $"[{timePrefix}{ts:HH:mm:ss.fff}]"; 
    }

    protected DateTime GetTimeStamp()
    {
        return _useUtc ? DateTime.UtcNow : DateTime.Now;
    }

    protected virtual void WriteEntry(LogType logType, string message,DateTime timestamp)
    {
        message = FormatMessage(timestamp,logType,message);
        
        if (LogToConsole)
            WriteToConsole(message);
        if (LogToFile)
        {
            string path = "";
            switch (logType)
            {
                case LogType.Log:
                    path = _logPath;
                    break;
                case LogType.Error:
                    path = _errorPath;
                    break;
                case LogType.Debug:
                    path = _debugPath;
                    break;
                case LogType.Info:
                    path = _infoPath;
                    break;
            }
            string fileName = GetLogFileName(timestamp,logType, path);
            WriteToFile(timestamp,logType,message, fileName);
        }
    }

    void WriteToConsole(string message)
    {
        Console.WriteLine(message);
    }

    void WriteToFile(DateTime timeStamp, LogType logType,string message, string fileName)
    {
        try
        {
            fileName = CheckAndRotateLogFile(timeStamp, logType, fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? String.Empty);

            using var sw = new StreamWriter(fileName, true);
            sw.WriteLine(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    string CheckAndRotateLogFile(DateTime timeStamp, LogType logType, string fileName)
    {
        if (File.Exists(fileName))
        {
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Length > 1024 * 1024) // 1MB
            {
                switch (logType)
                {
                    case LogType.Log:
                        _logCounter = _logCounter >= _maxLogFiles ? 1 : _logCounter + 1;
                        break;
                    case LogType.Error:
                        _errorCounter = _errorCounter >= _maxErrorFiles ? 1 : _errorCounter + 1;
                        break;
                    case LogType.Debug:
                        _debugCounter = _debugCounter >= _maxLogFiles ? 1 : _debugCounter + 1;
                        break;
                    case LogType.Info:
                        _infoCounter = _infoCounter >= _maxLogFiles ? 1 : _infoCounter + 1;
                        break;
                }
                fileName = GetLogFileName(timeStamp, logType, fileName);
            }
        }

        return fileName;
    }
    
    public void Start()
    {
        if (_wThread == null)
        {
            _wThread = new Thread(Run);
            _wThread.IsBackground = false;
            _wThread.Name = nameof(LogWorker);
            _wThread.Start();
        }
    }
    
    public void Stop()
    {
        _cts.Cancel();
        _wSignal.Release();
        _wThread?.Join();
    }

    
    void Run()
    {
        CancellationToken token = _cts.Token;
        while (!token.IsCancellationRequested)
        {
            _wSignal.Wait(token);
            P42LogObject obj;
            lock (_wLocker)
            {
                obj = _logQueue.Dequeue();
            }
            WriteEntry(obj.GetLogType(), obj.GetMessage(),obj.GetTimeStamp());
        }
    }
}