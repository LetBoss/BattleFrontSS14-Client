// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.AcidSlash.XenoAcidSlashSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.AcidSlash;

public sealed class XenoAcidSlashSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoAcidSlashComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoAcidSlashComponent, MeleeHitEvent>(this.OnMeleeHit));
  }

  private void OnMeleeHit(Entity<XenoAcidSlashComponent> xeno, ref MeleeHitEvent args)
  {
    if (!args.IsHit)
      return;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget(xeno.Owner, hitEntity) && !this.HasComp<XenoComponent>(hitEntity))
      {
        ComponentRegistry acid = xeno.Comp.Acid;
        if (acid == null)
          break;
        this.EntityManager.AddComponents(hitEntity, acid, true);
        break;
      }
    }
  }
}
