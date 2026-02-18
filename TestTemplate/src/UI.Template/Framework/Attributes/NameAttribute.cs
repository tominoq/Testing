namespace UI.Template.Framework.Attributes;

/// <summary>
/// Attribute to use for enum values to provide a name/alternative name for the value.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class NameAttribute(string name) : Attribute
{
    /// <summary>
    /// Value of the attribute
    /// </summary>
    public string Name { get; } = name;
}
