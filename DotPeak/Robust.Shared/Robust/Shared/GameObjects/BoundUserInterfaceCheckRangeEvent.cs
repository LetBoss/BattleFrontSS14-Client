// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.BoundUserInterfaceCheckRangeEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public struct BoundUserInterfaceCheckRangeEvent(
  Entity<TransformComponent> target,
  Enum uiKey,
  InterfaceData data,
  Entity<TransformComponent> actor)
{
  public readonly EntityUid Target = (EntityUid) target;
  public readonly Enum UiKey = uiKey;
  public readonly InterfaceData Data = data;
  public readonly Entity<TransformComponent> Actor = actor;
  public BoundUserInterfaceRangeResult Result = BoundUserInterfaceRangeResult.Default;
}
