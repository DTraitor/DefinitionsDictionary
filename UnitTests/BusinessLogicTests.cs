namespace UnitTests;

public class BusinessLogicTests
{
    private const string TestFileName = "BusinessLogicTest.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(TestFileName);
    }

    [Test]
    public void LogicTests()
    {
        FileStream stream = new FileStream(TestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        stream.SetLength(0);
        InteractionLogic interaction = new InteractionLogic(stream);

        Definition definition = interaction.CreateDefinition("TestName", "TestMeaning");
        Assert.That(interaction.GetDefinitions(), Is.EqualTo(new List<Definition>{definition}));
        interaction.ChangeName(definition, "TestName2");
        Assert.That(definition.Name, Is.EqualTo("TestName2"));
        interaction.ChangeMeaning(definition, "TestMeaning2");
        Assert.That(definition.Meaning, Is.EqualTo("TestMeaning2"));

        interaction = new InteractionLogic(stream);
        Assert.That(interaction.GetDefinitions(), Is.EqualTo(new List<Definition>{definition}));
        Definition newDefinition = interaction.CreateDefinition("TestName", "TestMeaning");
        Assert.That(interaction.GetDefinitions(), Is.EqualTo(new List<Definition>{definition, newDefinition}));
        interaction.DeleteDefinition(interaction.GetDefinitions()[0]);
        Assert.That(interaction.GetDefinitions(), Is.EqualTo(new List<Definition>{newDefinition}));

        stream.Close();

        interaction = new InteractionLogic(TestFileName);
        Assert.That(interaction.GetDefinitions(), Is.EqualTo(new List<Definition>{newDefinition}));
        interaction.Dispose();
    }
}