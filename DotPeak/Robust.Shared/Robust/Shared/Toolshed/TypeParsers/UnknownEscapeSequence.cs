// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.UnknownEscapeSequence
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class UnknownEscapeSequence(Rune c) : ConError
{
  public override FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted($"Unknown escape sequence: \\{c}");
  }
}
