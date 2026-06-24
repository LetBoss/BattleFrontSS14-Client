// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.SpanLikeTypeParser`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

public abstract class SpanLikeTypeParser<T, TElem> : TypeParser<T>
  where T : notnull
  where TElem : unmanaged
{
  public abstract int Elements { get; }

  public abstract T Create(Span<TElem> elements);

  public override unsafe bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
  {
    if (!ctx.EatMatch('['))
    {
      ctx.Error = (IConError) new ExpectedOpenBrace();
      result = default (T);
      return false;
    }
    ctx.ConsumeWhitespace();
    ctx.PushBlockTerminator(']');
    int elements1 = this.Elements;
    // ISSUE: untyped stack allocation
    Span<TElem> elements2 = new Span<TElem>((void*) __untypedstackalloc(checked (unchecked ((IntPtr) (uint) elements1) * sizeof (TElem))), elements1);
    for (int index1 = 0; index1 < this.Elements; ++index1)
    {
      ParserRestorePoint point = ctx.Save();
      TElem parsed;
      if (!this.Toolshed.TryParse<TElem>(ctx, out parsed))
      {
        ctx.Restore(point);
        int index2 = ctx.Index;
        if (ctx.EatBlockTerminator())
        {
          ctx.Error = (IConError) new UnexpectedCloseBrace();
          ctx.Error.Contextualize(ctx.Input, new Vector2i(index2, ctx.Index));
        }
        result = default (T);
        return false;
      }
      ctx.ConsumeWhitespace();
      if (index1 + 1 < this.Elements && ctx.EatBlockTerminator())
      {
        ctx.Error = (IConError) new UnexpectedCloseBrace();
        result = default (T);
        return false;
      }
      if (index1 + 1 < this.Elements && !ctx.EatMatch(','))
      {
        ctx.Error = (IConError) new ExpectedComma();
        result = default (T);
        return false;
      }
      elements2[index1] = parsed;
      ctx.ConsumeWhitespace();
    }
    if (!ctx.EatBlockTerminator())
    {
      ctx.Error = (IConError) new ExpectedCloseBrace();
      result = default (T);
      return false;
    }
    result = this.Create(elements2);
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg));
  }
}
