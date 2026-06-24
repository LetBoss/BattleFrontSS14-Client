// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableIFFSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableIFFSystem : EntitySystem
{
  [Dependency]
  private AttachableHolderSystem _holder;
  [Dependency]
  private GunIFFSystem _gunIFF;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableIFFComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableIFFComponent, AttachableAlteredEvent>(this.OnAttachableIFFAltered));
    this.SubscribeLocalEvent<AttachableIFFComponent, AttachableRelayedEvent<AttachableGrantIFFEvent>>(new EntityEventRefHandler<AttachableIFFComponent, AttachableRelayedEvent<AttachableGrantIFFEvent>>(this.OnAttachableIFFGrant));
    this.SubscribeLocalEvent<GunAttachableIFFComponent, AmmoShotEvent>(new EntityEventRefHandler<GunAttachableIFFComponent, AmmoShotEvent>(this.OnGunAttachableIFFAmmoShot));
    this.SubscribeLocalEvent<GunAttachableIFFComponent, ExaminedEvent>(new EntityEventRefHandler<GunAttachableIFFComponent, ExaminedEvent>(this.OnGunAttachableIFFExamined));
  }

  private void OnAttachableIFFAltered(
    Entity<AttachableIFFComponent> ent,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.Attached:
        this.UpdateGunIFF(args.Holder);
        break;
      case AttachableAlteredType.Detached:
        this.UpdateGunIFF(args.Holder);
        break;
    }
  }

  private void OnAttachableIFFGrant(
    Entity<AttachableIFFComponent> ent,
    ref AttachableRelayedEvent<AttachableGrantIFFEvent> args)
  {
    args.Args.Grants = true;
  }

  private void OnGunAttachableIFFAmmoShot(
    Entity<GunAttachableIFFComponent> ent,
    ref AmmoShotEvent args)
  {
    this._gunIFF.GiveAmmoIFF((EntityUid) ent, ref args, false, true);
  }

  private void OnGunAttachableIFFExamined(
    Entity<GunAttachableIFFComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("GunAttachableIFFComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-iff"));
  }

  private void UpdateGunIFF(EntityUid gun)
  {
    AttachableHolderComponent comp;
    if (!this.TryComp<AttachableHolderComponent>(gun, out comp))
      return;
    AttachableGrantIFFEvent args = new AttachableGrantIFFEvent();
    this._holder.RelayEvent<AttachableGrantIFFEvent>((Entity<AttachableHolderComponent>) (gun, comp), ref args);
    if (this._timing.ApplyingState)
      return;
    if (args.Grants)
      this.EnsureComp<GunAttachableIFFComponent>(gun);
    else
      this.RemCompDeferred<GunAttachableIFFComponent>(gun);
  }
}
