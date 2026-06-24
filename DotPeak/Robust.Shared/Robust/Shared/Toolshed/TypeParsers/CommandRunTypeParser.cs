// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.CommandRunTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class CommandRunTypeParser : TypeParser<CommandRun>
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out CommandRun? result)
  {
    return CommandRun.TryParse(ctx, (Type) null, (Type) null, out result);
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    CommandRun.TryParse(ctx, (Type) null, (Type) null, out CommandRun _);
    return ctx.Completions;
  }
}
