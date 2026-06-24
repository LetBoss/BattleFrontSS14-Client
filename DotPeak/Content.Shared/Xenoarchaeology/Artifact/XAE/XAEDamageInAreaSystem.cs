// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.XAEDamageInAreaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Whitelist;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEDamageInAreaSystem : BaseXAESystem<XAEDamageInAreaComponent>
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private IGameTiming _timing;
  private readonly HashSet<EntityUid> _entitiesInRange = new HashSet<EntityUid>();

  protected override void OnActivated(
    Entity<XAEDamageInAreaComponent> ent,
    ref XenoArtifactNodeActivatedEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    XAEDamageInAreaComponent comp = ent.Comp;
    this._entitiesInRange.Clear();
    this._lookup.GetEntitiesInRange(ent.Owner, comp.Radius, this._entitiesInRange);
    foreach (EntityUid uid in this._entitiesInRange)
    {
      if (this._random.Prob(comp.DamageChance) && !this._whitelistSystem.IsWhitelistFail(comp.Whitelist, uid))
        this._damageable.TryChangeDamage(new EntityUid?(uid), comp.Damage, comp.IgnoreResistances);
    }
  }
}
