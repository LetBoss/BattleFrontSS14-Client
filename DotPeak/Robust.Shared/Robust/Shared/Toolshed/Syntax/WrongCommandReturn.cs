// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.WrongCommandReturn
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public record struct WrongCommandReturn(Type Expected, Type Got) : IConError
{
  public Type Expected { get; set; } = Expected;

  public Type Got { get; set; } = Got;

  public FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted($"Expected an command run that returns type {this.Expected.PrettyName()}, but got {this.Got.PrettyName()}");
  }

  public string? Expression { get; set; } = (string) null;

  public Vector2i? IssueSpan { get; set; } = new Vector2i?();

  public StackTrace? Trace { get; set; } = (StackTrace) null;

  [CompilerGenerated]
  public readonly void Deconstruct(out Type Expected, out Type Got)
  {
    Expected = this.Expected;
    Got = this.Got;
  }
}
