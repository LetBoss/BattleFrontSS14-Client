// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.UnknownComponentError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public record struct UnknownComponentError(string Component) : IConError
{
  public string Component { get; set; } = Component;

  public FormattedMessage DescribeInner()
  {
    FormattedMessage formattedMessage1 = FormattedMessage.FromUnformatted($"Unknown component {this.Component}. For a list of all components, try types:components.");
    if (this.Component.EndsWith("component", true, CultureInfo.InvariantCulture))
    {
      formattedMessage1.PushNewline();
      FormattedMessage formattedMessage2 = formattedMessage1;
      string component = this.Component;
      int length = "component".Length;
      string text = $"Do not specify the word `Component` in the argument. Maybe try {component.Substring(0, component.Length - length)}?";
      formattedMessage2.AddText(text);
    }
    return formattedMessage1;
  }

  public string? Expression { get; set; } = (string) null;

  public Vector2i? IssueSpan { get; set; } = new Vector2i?();

  public StackTrace? Trace { get; set; } = (StackTrace) null;

  [CompilerGenerated]
  public readonly void Deconstruct(out string Component) => Component = this.Component;
}
