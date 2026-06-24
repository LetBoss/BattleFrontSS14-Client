// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.CustomCompletionParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public abstract class CustomCompletionParser<T> : CustomTypeParser<T> where T : notnull
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
  {
    return this.Toolshed.TryParse<T>(ctx, out result);
  }
}
