// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Reagent.RMCReagentSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Reagent;

public sealed class RMCReagentSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private ISerializationManager _serialization;
  private FrozenDictionary<string, Content.Shared._RMC14.Chemistry.Reagent.Reagent> _reagents = FrozenDictionary<string, Content.Shared._RMC14.Chemistry.Reagent.Reagent>.Empty;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.ReloadPrototypes();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<ReagentPrototype>())
      return;
    this.ReloadPrototypes();
  }

  private void ReloadPrototypes()
  {
    Dictionary<string, Content.Shared._RMC14.Chemistry.Reagent.Reagent> source = new Dictionary<string, Content.Shared._RMC14.Chemistry.Reagent.Reagent>();
    foreach (ReagentPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<ReagentPrototype>())
    {
      object target = (object) new Content.Shared._RMC14.Chemistry.Reagent.Reagent();
      this._serialization.CopyTo((object) enumeratePrototype, ref target);
      if (target is Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent)
        source[enumeratePrototype.ID] = reagent;
    }
    this._reagents = source.ToFrozenDictionary<string, Content.Shared._RMC14.Chemistry.Reagent.Reagent>();
  }

  public Content.Shared._RMC14.Chemistry.Reagent.Reagent Index(ProtoId<ReagentPrototype> id)
  {
    return this._reagents[(string) id];
  }

  public bool TryIndex(ProtoId<ReagentPrototype> id, [NotNullWhen(true)] out Content.Shared._RMC14.Chemistry.Reagent.Reagent? reagent)
  {
    return this._reagents.TryGetValue((string) id, out reagent);
  }

  public bool TryIndex(ReagentId id, [NotNullWhen(true)] out Content.Shared._RMC14.Chemistry.Reagent.Reagent? reagent)
  {
    return this._reagents.TryGetValue(id.Prototype, out reagent);
  }
}
