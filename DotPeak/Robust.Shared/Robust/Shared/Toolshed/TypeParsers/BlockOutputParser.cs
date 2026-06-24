// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.BlockOutputParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class BlockOutputParser : CustomTypeParser<Type>
{
  public override bool ShowTypeArgSignature => false;

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
  {
    result = (Type) null;
    ParserRestorePoint point = ctx.Save();
    int index = ctx.Index;
    CommandRun run;
    if (!Block.TryParseBlock(ctx, (Type) null, (Type) null, out run))
    {
      ctx.Error?.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
      return false;
    }
    ctx.Restore(point);
    if (run.ReturnType == (Type) null)
      return false;
    result = run.ReturnType;
    return true;
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    Block.TryParseBlock(ctx, (Type) null, (Type) null, out CommandRun _);
    return ctx.Completions;
  }
}
