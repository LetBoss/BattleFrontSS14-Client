// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Errors.ConError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System.Diagnostics;

#nullable enable
namespace Robust.Shared.Toolshed.Errors;

public abstract class ConError : IConError
{
  public abstract FormattedMessage DescribeInner();

  public string? Expression { get; set; }

  public Vector2i? IssueSpan { get; set; }

  public StackTrace? Trace { get; set; }
}
