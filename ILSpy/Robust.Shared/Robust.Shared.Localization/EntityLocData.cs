using System.Collections.Immutable;

namespace Robust.Shared.Localization;

public record EntityLocData(string Name, string Desc, string? Suffix, ImmutableDictionary<string, string> Attributes);
