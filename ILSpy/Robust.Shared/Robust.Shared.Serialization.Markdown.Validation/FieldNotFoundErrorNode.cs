using System;
using Robust.Shared.Serialization.Markdown.Value;

namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class FieldNotFoundErrorNode(ValueDataNode key, Type type) : ErrorNode(key, $"Field \"{key.Value}\" not found in \"{type}\".", alwaysRelevant: false)
{
	public Type FieldType { get; } = type;
}
