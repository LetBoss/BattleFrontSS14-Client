// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.UnknownType
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public record struct UnknownType(string T) : IConError
{
  public string T { get; set; } = T;

  public FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted($"The type {this.T} is not known and cannot be used.");
  }

  public string? Expression { get; set; } = (string) null;

  public Vector2i? IssueSpan { get; set; } = new Vector2i?();

  public StackTrace? Trace { get; set; } = (StackTrace) null;

  [CompilerGenerated]
  public readonly void Deconstruct(out string T) => T = this.T;
}
