// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.BeforeRangedInteractEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;

#nullable disable
namespace Content.Shared.Interaction;

public sealed class BeforeRangedInteractEvent : HandledEntityEventArgs
{
  public EntityUid User { get; }

  public EntityUid Used { get; }

  public EntityUid? Target { get; }

  public EntityCoordinates ClickLocation { get; }

  public bool CanReach { get; }

  public BeforeRangedInteractEvent(
    EntityUid user,
    EntityUid used,
    EntityUid? target,
    EntityCoordinates clickLocation,
    bool canReach)
  {
    this.User = user;
    this.Used = used;
    this.Target = target;
    this.ClickLocation = clickLocation;
    this.CanReach = canReach;
  }
}
