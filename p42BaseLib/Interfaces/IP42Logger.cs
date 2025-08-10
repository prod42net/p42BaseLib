namespace p42BaseLib.Interfaces;

public interface IP42Logger
{
    void Log(string message);
    void Debug(string message);
    void Error(string message);
    void Info(string message);
}