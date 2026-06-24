// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ClawSharpness.XenoClawsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Doors.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Melee;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ClawSharpness;

public sealed class XenoClawsSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _protoManager;
  private Robust.Shared.GameObjects.EntityQuery<MeleeWeaponComponent> _meleeWeaponQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoClawsComponent> _xenoClawsQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoComponent> _xenoQuery;
  private readonly ProtoId<DamageGroupPrototype> _clawsDamageGroup = (ProtoId<DamageGroupPrototype>) "Brute";

  public override void Initialize()
  {
    this._meleeWeaponQuery = this.GetEntityQuery<MeleeWeaponComponent>();
    this._xenoClawsQuery = this.GetEntityQuery<XenoClawsComponent>();
    this._xenoQuery = this.GetEntityQuery<XenoComponent>();
    this.SubscribeLocalEvent<ReceiverXenoClawsComponent, DamageModifyEvent>(new EntityEventRefHandler<ReceiverXenoClawsComponent, DamageModifyEvent>(this.OnReceiverDamageModify));
    this.SubscribeLocalEvent<AirlockReceiverXenoClawsComponent, DamageModifyEvent>(new EntityEventRefHandler<AirlockReceiverXenoClawsComponent, DamageModifyEvent>(this.OnAirlockReceiverDamageModify));
  }

  private void OnReceiverDamageModify(
    Entity<ReceiverXenoClawsComponent> ent,
    ref DamageModifyEvent args)
  {
    EntityUid? tool = args.Tool;
    ReceiverXenoClawsComponent comp = ent.Comp;
    XenoClawsComponent component1;
    if (!this._meleeWeaponQuery.HasComp(tool) || !this._xenoClawsQuery.TryComp(tool, out component1))
      return;
    int num1 = component1.ClawType.CompareTo((object) comp.MinimumClawStrength) >= 0 ? 1 : 0;
    bool flag = false;
    if (comp.MinimumXenoTier.HasValue)
    {
      XenoComponent component2;
      int num2;
      if (this._xenoQuery.TryComp(tool, out component2))
      {
        int tier = component2.Tier;
        int? minimumXenoTier = comp.MinimumXenoTier;
        int valueOrDefault = minimumXenoTier.GetValueOrDefault();
        num2 = tier >= valueOrDefault & minimumXenoTier.HasValue ? 1 : 0;
      }
      else
        num2 = 0;
      flag = num2 != 0;
    }
    int num3 = flag ? 1 : 0;
    if ((num1 | num3) != 0)
      args.Damage = new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(this._clawsDamageGroup), (FixedPoint2) (comp.MaxHealth / (float) comp.HitsToDestroy));
    else
      args.Damage = new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(this._clawsDamageGroup), (FixedPoint2) 0);
  }

  private void OnAirlockReceiverDamageModify(
    Entity<AirlockReceiverXenoClawsComponent> ent,
    ref DamageModifyEvent args)
  {
    EntityUid? tool = args.Tool;
    AirlockReceiverXenoClawsComponent comp1 = ent.Comp;
    DoorComponent comp2;
    DoorBoltComponent comp3;
    if (!this._meleeWeaponQuery.HasComp(tool) || !this.TryComp<DoorComponent>((EntityUid) ent, out comp2) || !this.TryComp<DoorBoltComponent>((EntityUid) ent, out comp3))
      return;
    DamageSpecifier damageSpecifier = new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(this._clawsDamageGroup), (FixedPoint2) 0);
    XenoClawsComponent component;
    if (this._xenoClawsQuery.TryComp(tool, out component) && component.ClawType.CompareTo((object) comp1.MinimumClawStrength) >= 0)
    {
      if (comp3.BoltsDown)
        damageSpecifier = new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(this._clawsDamageGroup), (FixedPoint2) (comp1.MaxHealth / (float) comp1.HitsToDestroyBolted));
      if (comp2.State == DoorState.Welded)
        damageSpecifier = new DamageSpecifier(this._protoManager.Index<DamageGroupPrototype>(this._clawsDamageGroup), (FixedPoint2) (comp1.MaxHealth / (float) comp1.HitsToDestroyWelded));
    }
    args.Damage = damageSpecifier;
  }
}
