namespace Edwards.Kevin.Budgeteer.Utils;

public interface IUtils
{
    string[] LoadFile(string filePath);
    string[] GetFiles(string path);
}

public class Utils : IUtils
{
    public string[] LoadFile(string filePath)
    {
        return File.ReadAllLines(filePath);
    }

    public string[] GetFiles(string path) => Directory.GetFiles(path);

}