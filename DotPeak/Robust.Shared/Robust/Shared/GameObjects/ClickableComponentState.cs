// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ClickableComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
internal sealed class ClickableComponentState : ComponentState
{
  public Box2? LocalBounds { get; }

  public ClickableComponentState(Box2? localBounds) => this.LocalBounds = localBounds;
}
