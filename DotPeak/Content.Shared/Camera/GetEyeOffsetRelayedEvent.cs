// Decompiled with JetBrains decompiler
// Type: Content.Shared.Camera.GetEyeOffsetRelayedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System.Numerics;

#nullable disable
namespace Content.Shared.Camera;

[ByRefEvent]
public sealed class GetEyeOffsetRelayedEvent : EntityEventArgs, IInventoryRelayEvent
{
  public Vector2 Offset;

  public SlotFlags TargetSlots { get; } = SlotFlags.All;
}
