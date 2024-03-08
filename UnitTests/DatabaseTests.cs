namespace UnitTests;

public class DatabaseTests
{
    private const string TestFileName = "DatabaseTest.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(TestFileName);
    }

    [Test]
    public void ReadWriteTest()
    {
        FileStream stream = new FileStream(TestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        Definition definition = new Definition()
        {
            Name = "TestName",
            Meaning = "This is a test meaning with <Link def='Other'>link</Link> to other definition",
        };
        stream.SetLength(0);

        Database<Definition> database = new (stream);
        Assert.That(database.DefinitionsList, Is.EqualTo(new List<Definition>()));

        database.DefinitionsList.Add(definition);
        database.Dispose();
        stream.Close();

        database = new Database<Definition>(TestFileName);
        Assert.That(database.DefinitionsList, Is.EqualTo(new List<Definition>{definition}));
        database.Dispose();
    }
}