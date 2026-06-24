// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATDamageThresholdReachedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATDamageThresholdReachedSystem : 
  BaseXATSystem<XATDamageThresholdReachedComponent>
{
  [Dependency]
  private IPrototypeManager _prototype;

  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<DamageChangedEvent>(new BaseXATSystem<XATDamageThresholdReachedComponent>.XATEventHandler<DamageChangedEvent>(this.OnDamageChanged));
  }

  private void OnDamageChanged(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATDamageThresholdReachedComponent, XenoArtifactNodeComponent> node,
    ref DamageChangedEvent args)
  {
    if (!args.DamageIncreased || args.DamageDelta == null)
      return;
    EntityUid? origin = args.Origin;
    EntityUid owner = artifact.Owner;
    if ((origin.HasValue ? (origin.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
      return;
    XATDamageThresholdReachedComponent comp1 = node.Comp1;
    if (this.Timing.IsFirstTimePredicted)
      comp1.AccumulatedDamage += args.DamageDelta;
    foreach ((ProtoId<DamageTypePrototype> key, FixedPoint2 fixedPoint2_3) in comp1.TypesNeeded)
    {
      FixedPoint2 fixedPoint2_2 = fixedPoint2_3;
      if (comp1.AccumulatedDamage.DamageDict.GetValueOrDefault<string, FixedPoint2>((string) key) >= fixedPoint2_2)
      {
        this.InvokeTrigger(artifact, node);
        return;
      }
    }
    ProtoId<DamageGroupPrototype> protoId2;
    foreach ((protoId2, fixedPoint2_3) in comp1.GroupsNeeded)
    {
      FixedPoint2 fixedPoint2_4 = fixedPoint2_3;
      DamageGroupPrototype group = this._prototype.Index<DamageGroupPrototype>(protoId2);
      FixedPoint2 total;
      if (comp1.AccumulatedDamage.TryGetDamageInGroup(group, out total) && total >= fixedPoint2_4)
      {
        this.InvokeTrigger(artifact, node);
        break;
      }
    }
  }

  private void InvokeTrigger(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATDamageThresholdReachedComponent, XenoArtifactNodeComponent> node)
  {
    XATDamageThresholdReachedComponent comp1 = node.Comp1;
    comp1.AccumulatedDamage.DamageDict.Clear();
    this.Dirty((EntityUid) node, (IComponent) comp1);
    this.Trigger(artifact, node);
  }
}
