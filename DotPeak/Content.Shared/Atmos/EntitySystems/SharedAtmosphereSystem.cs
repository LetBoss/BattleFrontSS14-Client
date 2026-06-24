// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedAtmosphereSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Clothing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosphereSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedInternalsSystem _internals;
  private EntityQuery<InternalsComponent> _internalsQuery;
  protected readonly GasPrototype[] GasPrototypes = new GasPrototype[9];

  private void InitializeBreathTool()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BreathToolComponent, ComponentShutdown>(new EntityEventRefHandler<BreathToolComponent, ComponentShutdown>((object) this, __methodptr(OnBreathToolShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BreathToolComponent, ItemMaskToggledEvent>(new EntityEventRefHandler<BreathToolComponent, ItemMaskToggledEvent>((object) this, __methodptr(OnMaskToggled)), (Type[]) null, (Type[]) null);
  }

  private void OnBreathToolShutdown(Entity<BreathToolComponent> entity, ref ComponentShutdown args)
  {
    this.DisconnectInternals(entity);
  }

  public void DisconnectInternals(Entity<BreathToolComponent> entity, bool forced = false)
  {
    EntityUid? connectedInternalsEntity = entity.Comp.ConnectedInternalsEntity;
    if (!connectedInternalsEntity.HasValue)
      return;
    entity.Comp.ConnectedInternalsEntity = new EntityUid?();
    InternalsComponent internalsComponent;
    if (this._internalsQuery.TryComp(connectedInternalsEntity, ref internalsComponent))
      this._internals.DisconnectBreathTool(Entity<InternalsComponent>.op_Implicit((connectedInternalsEntity.Value, internalsComponent)), entity.Owner, forced);
    this.Dirty<BreathToolComponent>(entity, (MetaDataComponent) null);
  }

  private void OnMaskToggled(Entity<BreathToolComponent> ent, ref ItemMaskToggledEvent args)
  {
    if (args.Mask.Comp.IsToggled)
    {
      this.DisconnectInternals(ent, true);
    }
    else
    {
      InternalsComponent internalsComponent;
      if (!this._internalsQuery.TryComp(args.Wearer, ref internalsComponent))
        return;
      this._internals.ConnectBreathTool(Entity<InternalsComponent>.op_Implicit((args.Wearer.Value, internalsComponent)), Entity<BreathToolComponent>.op_Implicit(ent));
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._internalsQuery = this.GetEntityQuery<InternalsComponent>();
    this.InitializeBreathTool();
    for (int index = 0; index < 9; ++index)
      this.GasPrototypes[index] = this._prototypeManager.Index<GasPrototype>(index.ToString());
  }

  public GasPrototype GetGas(int gasId) => this.GasPrototypes[gasId];

  public GasPrototype GetGas(Gas gasId) => this.GasPrototypes[(int) gasId];

  public IEnumerable<GasPrototype> Gases => (IEnumerable<GasPrototype>) this.GasPrototypes;
}
