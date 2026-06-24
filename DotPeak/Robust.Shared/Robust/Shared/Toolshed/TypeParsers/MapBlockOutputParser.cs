// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.MapBlockOutputParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class MapBlockOutputParser : CustomTypeParser<Type>
{
  public override bool ShowTypeArgSignature => false;

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
  {
    result = (Type) null;
    Type type1 = ctx.Bundle.PipedType;
    if (type1 != (Type) null)
    {
      if (type1.IsGenericType(typeof (IEnumerable<>)))
        type1 = type1.GetGenericArguments()[0];
      else if (type1.IsGenericType(typeof (List<>)))
        type1 = type1.GetGenericArguments()[0];
      else if (type1.IsArray)
      {
        type1 = type1.GetElementType();
      }
      else
      {
        Type type2 = ((IEnumerable<Type>) type1.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (x => x.IsGenericType(typeof (IEnumerable<>))))?.GetGenericArguments()[0];
        if ((object) type2 == null)
          type2 = type1;
        type1 = type2;
      }
    }
    ParserRestorePoint point = ctx.Save();
    int index = ctx.Index;
    CommandRun run;
    if (!Block.TryParseBlock(ctx, type1, (Type) null, out run))
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
    Type type = ctx.Bundle.PipedType;
    if (type != (Type) null && type.IsGenericType(typeof (IEnumerable<>)))
      type = type.GetGenericArguments()[0];
    Block.TryParseBlock(ctx, type, (Type) null, out CommandRun _);
    return ctx.Completions;
  }
}
