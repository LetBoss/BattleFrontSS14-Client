// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.ExpectedGeneric
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Diagnostics;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public record struct ExpectedGeneric : IConError
{
  public ExpectedGeneric()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CExpression\u003Ek__BackingField = (string) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CIssueSpan\u003Ek__BackingField = new Vector2i?();
    // ISSUE: reference to a compiler-generated field
    this.\u003CTrace\u003Ek__BackingField = (StackTrace) null;
  }

  public FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted("Expected a generic type, did you forget the angle brackets?");
  }

  public string? Expression { get; set; }

  public Vector2i? IssueSpan { get; set; }

  public StackTrace? Trace { get; set; }
}
