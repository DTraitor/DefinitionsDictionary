using DataAccessLayer;

namespace BusinessLogicLayer;

public class InteractionLogic : IDisposable
{
    public InteractionLogic(string fileName)
    {
        _database = new Database<Definition>(fileName);
        foreach (Definition def in _database.DefinitionsList)
            def.GenerateLinks(_database.DefinitionsList);
    }

    public InteractionLogic(FileStream stream)
    {
        _database = new Database<Definition>(stream);
        foreach (Definition def in _database.DefinitionsList)
            def.GenerateLinks(_database.DefinitionsList);
    }

    public void Dispose()
    {
        _database.Dispose();
    }

    public List<Definition> GetDefinitions()
    {
        return _database.DefinitionsList;
    }

    public Definition CreateDefinition(string name, string meaning)
    {
        Definition definition = new Definition()
        {
            Name = name,
            Meaning = meaning
        };
        _database.DefinitionsList.Add(definition);
        definition.GenerateLinks(_database.DefinitionsList);
        foreach (Definition def in _database.DefinitionsList)
            def.OnDefinitionAdded(definition);
        _database.Save();
        return definition;
    }

    public void DeleteDefinition(Definition definition)
    {
        _database.DefinitionsList.Remove(definition);
        foreach (Definition def in _database.DefinitionsList)
            def.OnDefinitionRemoved(definition);
        _database.Save();
    }

    public void ChangeName(Definition definition, string newName)
    {
        foreach (Definition def in _database.DefinitionsList)
        {
            def.OnDefinitionRemoved(definition);
        }
        definition.Name = newName;
        foreach (Definition def in _database.DefinitionsList)
        {
            def.OnDefinitionAdded(definition);
        }
        _database.Save();
    }

    public void ChangeMeaning(Definition definition, string newMeaning)
    {
        definition.Meaning = newMeaning;
        definition.GenerateLinks(_database.DefinitionsList);
        _database.Save();
    }

    private readonly Database<Definition> _database;
}