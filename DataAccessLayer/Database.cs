using System.Text.Json;

namespace DataAccessLayer;

public class Database<T> : IDisposable
{
    public Database(string file)
    {
        stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        Read();
    }

    public Database(FileStream stream)
    {
        streamOwned = false;
        this.stream = stream;
        Read();
    }

    public void Dispose()
    {
        Save();
        if (streamOwned)
            stream.Close();
    }

    private void Read()
    {
        stream.Position = 0;
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        try
        {
            DefinitionsList = JsonSerializer.Deserialize<List<T>>(reader.ReadToEnd()) ?? new List<T>();
        }
        catch (JsonException e)
        {
            DefinitionsList = new List<T>();
        }
    }

    public void Save()
    {
        stream.SetLength(0);
        using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
        writer.Write(JsonSerializer.Serialize(DefinitionsList));
    }

    private bool streamOwned = true;
    public List<T> DefinitionsList { get; private set; }
    private readonly FileStream stream;
}