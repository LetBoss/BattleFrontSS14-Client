// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableSilencerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableSilencerSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableSilencerComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>(new EntityEventRefHandler<AttachableSilencerComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>(this.OnSilencerRefreshModifiers));
    this.SubscribeLocalEvent<AttachableSilencerComponent, AttachableRelayedEvent<GunMuzzleFlashAttemptEvent>>(new EntityEventRefHandler<AttachableSilencerComponent, AttachableRelayedEvent<GunMuzzleFlashAttemptEvent>>(this.OnSilencerMuzzleFlash));
  }

  private void OnSilencerRefreshModifiers(
    Entity<AttachableSilencerComponent> ent,
    ref AttachableRelayedEvent<GunRefreshModifiersEvent> args)
  {
    args.Args.SoundGunshot = ent.Comp.Sound;
  }

  private void OnSilencerMuzzleFlash(
    Entity<AttachableSilencerComponent> ent,
    ref AttachableRelayedEvent<GunMuzzleFlashAttemptEvent> args)
  {
    args.Args.Cancelled = true;
  }
}
