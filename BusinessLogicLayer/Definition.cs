using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer;

public class Definition
{
    public string Name { get; set; }
    public string Meaning { get; set; }
    // Word in the text - it's definition
    [JsonIgnore]
    public Dictionary<string, Definition> UsedDefinitions { get; set; }
    [JsonIgnore]
    public List<string> MissingDefinitions { get; set; }
    [JsonIgnore]
    public readonly Regex DefinitionLinksRegex = new Regex(@"(.*?)<Link\s+def=(?:(?:'|"")(.+)(?:'|""))>(.+?)<\/Link>");

    public void GenerateLinks(List<Definition> defs)
    {
        MissingDefinitions = new List<string>();
        UsedDefinitions = new Dictionary<string, Definition>();
        foreach (Match match in DefinitionLinksRegex.Matches(Meaning))
        {
            string defName = match.Groups[2].Value;
            string text = match.Groups[3].Value;
            Definition? def = defs.FirstOrDefault(d => d.Name == defName);
            if (def == null)
            {
                MissingDefinitions.Add(defName);
                continue;
            }
            UsedDefinitions[defName] = def;
        }
    }

    public void OnDefinitionAdded(Definition def)
    {
        if (!MissingDefinitions.Contains(def.Name))
            return;
        MissingDefinitions.Remove(def.Name);
        UsedDefinitions[def.Name] = def;
    }

    public void OnDefinitionRemoved(Definition def)
    {
        if (!UsedDefinitions.ContainsKey(def.Name))
            return;
        UsedDefinitions.Remove(def.Name);
        MissingDefinitions.Add(def.Name);
    }

    public override bool Equals(object? obj)
    {
        return obj is Definition definition &&
                Name == definition.Name &&
                Meaning == definition.Meaning;
    }
}