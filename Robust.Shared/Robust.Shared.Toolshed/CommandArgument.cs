using System;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed;

public readonly record struct CommandArgument(string Name, Type Type, ITypeParser? Parser, bool IsOptional, object? DefaultValue, bool IsParamsCollection);
