
using p42BaseLib.Interfaces;


namespace p42BaseLib;

public enum LogType
{
    Log,
    Error,
    Debug,
    Info
}

public class P42Logger : IP42Logger
{
 

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

    FixedSizeQueue<string> _logQueue = new(200);
    FixedSizeQueue<string> _errorQueue = new(500);

    bool Production { get; set; }
    void SetProduction(bool production) => Production = production;
    void SetLogQueueSize(int size) => _logQueue = new(size);
    void SetErrorQueueSize(int size) => _errorQueue = new(size);

    bool LogToFile { get; set; } = true;
    bool ErrorToFile { get; set; } = true;
    bool DebugToFile { get; set; } = false;
    bool InfoToFile { get; set; } = false;

    public P42Logger(bool production = false) 
    {
        SetProduction(production);
    }

    public P42Logger(string logPath, string errorPath, bool production = false) : this()
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
        WriteEntry(LogType.Log, message);
        _logQueue.Enqueue(message);
    }

    public void Debug(string message)
    {
        if (Production) return;
        WriteEntry(LogType.Debug, message);
    }

    public void Error(string message)
    {
        WriteEntry(LogType.Error, message);
        _errorQueue.Enqueue(message);
        // should be stored somewhere at least or better someone gets notified ;)
    }

    public void Info(string message)
    {
        WriteEntry(LogType.Info, message);
    }


    public virtual string FormatMessage(DateTime timeStamp,LogType logType,string message)
    {
       
        string timePrefix = (_yearFolders ? "" : timeStamp.Year.ToString(@"0000")) +
                            (_monthFolders ? "" : timeStamp.Month.ToString(@"00")) +
                            (_monthFolders ? "" : timeStamp.Month.ToString(@"00 "));
        return $"[{timePrefix}{timeStamp:HH:mm:ss.fff}] [{logType.ToString("")}] {message}"; 

    }
    
    protected virtual void WriteEntry(LogType logType, string message)
    {
        DateTime ts = _useUtc ? DateTime.UtcNow : DateTime.Now;
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

        message = FormatMessage(ts,logType,message);
        string fileName = GetLogFileName(ts,logType, path);
        
        WriteToConsole(message);
        WriteToFile(ts,logType,message, fileName);
    }

    void WriteToConsole(string message)
    {
        Console.WriteLine(message);
    }

    void WriteToFile(DateTime timeStamp, LogType logType,string message, string fileName)
    {
        try
        {
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
}