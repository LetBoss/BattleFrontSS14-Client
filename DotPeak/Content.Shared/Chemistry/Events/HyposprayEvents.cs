// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Hypospray.Events.BeforeHyposprayInjectsTargetEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Chemistry.Hypospray.Events;

public abstract class BeforeHyposprayInjectsTargetEvent : 
  CancellableEntityEventArgs,
  IInventoryRelayEvent
{
  public EntityUid EntityUsingHypospray;
  public readonly EntityUid Hypospray;
  public EntityUid TargetGettingInjected;
  public string? InjectMessageOverride;

  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public BeforeHyposprayInjectsTargetEvent(EntityUid user, EntityUid hypospray, EntityUid target)
  {
    this.EntityUsingHypospray = user;
    this.Hypospray = hypospray;
    this.TargetGettingInjected = target;
    this.InjectMessageOverride = (string) null;
  }
}
