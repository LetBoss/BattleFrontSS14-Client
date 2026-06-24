// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Errors.IConError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System.Diagnostics;

#nullable enable
namespace Robust.Shared.Toolshed.Errors;

public interface IConError
{
  FormattedMessage Describe()
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    string expression = this.Expression;
    if (expression != null)
    {
      Vector2i? issueSpan = this.IssueSpan;
      if (issueSpan.HasValue)
      {
        Vector2i valueOrDefault = issueSpan.GetValueOrDefault();
        formattedMessage.AddMessage(ConHelpers.HighlightSpan(expression, valueOrDefault, Color.Red));
        formattedMessage.PushNewline();
        formattedMessage.AddMessage(ConHelpers.ArrowSpan(valueOrDefault));
        formattedMessage.PushNewline();
      }
    }
    formattedMessage.AddMessage(this.DescribeInner());
    return formattedMessage;
  }

  protected FormattedMessage DescribeInner();

  string? Expression { get; protected set; }

  Vector2i? IssueSpan { get; protected set; }

  StackTrace? Trace { get; protected set; }

  void Contextualize(string expression, Vector2i issueSpan)
  {
    if (this.Expression != null && this.IssueSpan.HasValue)
      return;
    this.Expression = expression;
    this.IssueSpan = new Vector2i?(issueSpan);
  }
}
