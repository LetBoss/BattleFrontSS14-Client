using System;

namespace Robust.Shared.Serialization.Markdown;

public sealed class DataParseException : Exception
{
	public DataParseException()
	{
	}

	public DataParseException(string message)
		: base(message)
	{
	}

	public DataParseException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
