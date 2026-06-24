// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.ExpectedArgumentError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Toolshed;

public sealed class ExpectedArgumentError(Type type) : ConError
{
  public override FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted($"Expected command argument of type {type.PrettyName()}, but ran out of input");
  }
}
