using System;
using Robust.Shared.Serialization.Markdown.Validation;

namespace Robust.Shared.Serialization.Manager.Exceptions;

public sealed class InvalidValidationNodeReturnedException<T> : Exception where T : ValidationNode
{
	public Type ActualType;

	public override string Message => $"{"ValidationNode"} of type {ActualType} provided, but {typeof(T)} expected.";

	public InvalidValidationNodeReturnedException(ValidationNode validationNode)
	{
		ActualType = validationNode.GetType();
	}
}
