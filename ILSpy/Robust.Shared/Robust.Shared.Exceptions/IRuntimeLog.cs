using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Exceptions;

[NotContentImplementable]
public interface IRuntimeLog
{
	int ExceptionCount { get; }

	void LogException(Exception exception, string? catcher = null);

	string Display();
}
