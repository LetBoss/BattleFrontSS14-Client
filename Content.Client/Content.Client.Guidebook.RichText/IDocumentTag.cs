using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;

namespace Content.Client.Guidebook.Richtext;

public interface IDocumentTag
{
	bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control);
}
