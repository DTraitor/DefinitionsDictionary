namespace UnitTests;

public class LinkTests
{
    [Test]
    public void DefinitionLinkTest()
    {
        Definition definition = new Definition()
        {
            Name = "TestName",
            Meaning = "This is a test meaning with <Link def='Other'>link</Link> to other definition",
        };
        Definition otherDefinition = new Definition()
        {
            Name = "Other",
            Meaning = "This is a other definition",
        };

        definition.GenerateLinks(new List<Definition>{});
        Assert.That(definition.MissingDefinitions, Is.EqualTo(new List<string>{"Other"}));
        Assert.That(definition.UsedDefinitions, Is.EqualTo(new Dictionary<string, Definition>{}));
        definition.OnDefinitionAdded(otherDefinition);
        definition.OnDefinitionAdded(otherDefinition);
        Assert.That(definition.MissingDefinitions, Is.EqualTo(new List<string>{}));
        Assert.That(definition.UsedDefinitions, Is.EqualTo(new Dictionary<string, Definition>{{"Other", otherDefinition}}));
        definition.OnDefinitionRemoved(otherDefinition);
        definition.OnDefinitionRemoved(otherDefinition);
        Assert.That(definition.MissingDefinitions, Is.EqualTo(new List<string>{"Other"}));
        Assert.That(definition.UsedDefinitions, Is.EqualTo(new Dictionary<string, Definition>{}));
        definition.GenerateLinks(new List<Definition>{otherDefinition});
        Assert.That(definition.MissingDefinitions, Is.EqualTo(new List<string>{}));
        Assert.That(definition.UsedDefinitions, Is.EqualTo(new Dictionary<string, Definition>{{"Other", otherDefinition}}));
    }
}