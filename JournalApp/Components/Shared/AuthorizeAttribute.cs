namespace JournalApp.Components.Shared;

/// <summary>
/// Simple authorization attribute for Blazor pages
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AuthorizeAttribute : Attribute
{
}
