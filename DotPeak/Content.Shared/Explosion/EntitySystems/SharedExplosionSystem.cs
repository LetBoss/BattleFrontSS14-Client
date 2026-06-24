// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.EntitySystems.SharedExplosionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Armor;
using Content.Shared.Explosion.Components;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Explosion.EntitySystems;

public abstract class SharedExplosionSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ExplosionResistanceComponent, ArmorExamineEvent>(new EntityEventRefHandler<ExplosionResistanceComponent, ArmorExamineEvent>(this.OnArmorExamine));
  }

  private void OnArmorExamine(Entity<ExplosionResistanceComponent> ent, ref ArmorExamineEvent args)
  {
    float num = MathF.Round((float) ((1.0 - (double) ent.Comp.DamageCoefficient) * 100.0), 1);
    if ((double) num == 0.0)
      return;
    args.Msg.PushNewline();
    args.Msg.AddMarkupOrThrow(this.Loc.GetString((string) ent.Comp.Examine, ("value", (object) num)));
  }

  public virtual void TriggerExplosive(
    EntityUid uid,
    ExplosiveComponent? explosive = null,
    bool delete = true,
    float? totalIntensity = null,
    float? radius = null,
    EntityUid? user = null)
  {
  }

  public void SetExplosionResistance(
    EntityUid uid,
    float damageCoefficient,
    bool worn,
    ExplosionResistanceComponent? resistance = null)
  {
    if (resistance == null)
      resistance = this.EnsureComp<ExplosionResistanceComponent>(uid);
    resistance.DamageCoefficient = damageCoefficient;
    resistance.Worn = worn;
    this.Dirty(uid, (IComponent) resistance);
  }
}
