// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameStates.ComponentHandleState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Robust.Shared.GameStates;

[ByRefEvent]
[ComponentEvent]
public readonly struct ComponentHandleState(IComponentState? current, IComponentState? next)
{
  public IComponentState? Current { get; } = current;

  public IComponentState? Next { get; } = next;
}
