// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.NotValidCommandError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class NotValidCommandError : ConError
{
  public Type? TargetType;

  public override FormattedMessage DescribeInner()
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddText("Ran into an invalid command, could not parse.");
    if ((object) this.TargetType != null && this.TargetType != typeof (void))
    {
      formattedMessage.PushNewline();
      formattedMessage.AddText($"The parser was trying to obtain a run of type {this.TargetType.PrettyName()}, make sure your run actually returns that value.");
    }
    return formattedMessage;
  }
}
