// Decompiled with JetBrains decompiler
// Type: Content.Shared.Toolshed.TypeParsers.FixedPoint2TypeParser
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

#nullable enable
namespace Content.Shared.Toolshed.TypeParsers;

public sealed class FixedPoint2TypeParser : TypeParser<FixedPoint2>
{
  public override bool TryParse(ParserContext ctx, out FixedPoint2 result)
  {
    int? parsed1;
    if (this.Toolshed.TryParse<int?>(ctx, out parsed1))
    {
      result = FixedPoint2.New(parsed1.Value);
      return true;
    }
    float? parsed2;
    if (this.Toolshed.TryParse<float?>(ctx, out parsed2))
    {
      result = FixedPoint2.New(parsed2.Value);
      return true;
    }
    result = FixedPoint2.Zero;
    return false;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg));
  }
}
