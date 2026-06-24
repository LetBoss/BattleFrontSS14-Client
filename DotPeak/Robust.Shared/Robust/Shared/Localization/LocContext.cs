// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Linguini.Bundle;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Localization;

public readonly struct LocContext
{
  internal readonly FluentBundle Bundle;

  public CultureInfo Culture => this.Bundle.Culture;

  internal LocContext(FluentBundle bundle) => this.Bundle = bundle;
}
