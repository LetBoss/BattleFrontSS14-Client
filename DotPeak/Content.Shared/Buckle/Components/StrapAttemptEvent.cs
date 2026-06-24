// Decompiled with JetBrains decompiler
// Type: Content.Shared.Buckle.Components.StrapAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Buckle.Components;

[ByRefEvent]
public record struct StrapAttemptEvent(
  Entity<StrapComponent> Strap,
  Entity<BuckleComponent> Buckle,
  EntityUid? User,
  bool Popup)
{
  public bool Cancelled = false;

  public Entity<StrapComponent> Strap { get; set; } = Strap;

  public Entity<BuckleComponent> Buckle { get; set; } = Buckle;

  public EntityUid? User { get; set; } = User;

  public bool Popup { get; set; } = Popup;
}
