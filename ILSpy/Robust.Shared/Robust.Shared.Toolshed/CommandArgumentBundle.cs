using System;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed;

public struct CommandArgumentBundle
{
	public string? Command;

	public string? SubCommand;

	public Dictionary<string, object?>? Arguments;

	public Type[]? TypeArguments;

	public required bool Inverted;

	public required Type? PipedType;

	public int NameStart;

	public int NameEnd;
}
