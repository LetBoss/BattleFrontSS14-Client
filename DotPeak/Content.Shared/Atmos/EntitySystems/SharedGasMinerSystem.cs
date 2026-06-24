// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedGasMinerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Examine;
using Content.Shared.Temperature;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasMinerSystem : EntitySystem
{
  [Dependency]
  private SharedAtmosphereSystem _sharedAtmosphereSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasMinerComponent, ExaminedEvent>(new EntityEventRefHandler<GasMinerComponent, ExaminedEvent>((object) this, __methodptr(OnExamine)), (Type[]) null, (Type[]) null);
  }

  private void OnExamine(Entity<GasMinerComponent> ent, ref ExaminedEvent args)
  {
    GasMinerComponent comp = ent.Comp;
    using (args.PushGroup("GasMinerComponent"))
    {
      args.PushMarkup(this.Loc.GetString("gas-miner-mines-text", ("gas", (object) this.Loc.GetString(this._sharedAtmosphereSystem.GetGas(comp.SpawnGas).Name))));
      args.PushText(this.Loc.GetString("gas-miner-amount-text", ("moles", (object) $"{comp.SpawnAmount:0.#}")));
      args.PushText(this.Loc.GetString("gas-miner-temperature-text", ("tempK", (object) $"{comp.SpawnTemperature:0.#}"), ("tempC", (object) $"{TemperatureHelpers.KelvinToCelsius(comp.SpawnTemperature):0.#}")));
      if ((double) comp.MaxExternalAmount < double.PositiveInfinity)
        args.PushText(this.Loc.GetString("gas-miner-moles-cutoff-text", ("moles", (object) $"{comp.MaxExternalAmount:0.#}")));
      if ((double) comp.MaxExternalPressure < double.PositiveInfinity)
        args.PushText(this.Loc.GetString("gas-miner-pressure-cutoff-text", ("pressure", (object) $"{comp.MaxExternalPressure:0.#}")));
      ExaminedEvent examinedEvent = args;
      string markup;
      switch (comp.MinerState)
      {
        case GasMinerState.Disabled:
          markup = this.Loc.GetString("gas-miner-state-disabled-text");
          break;
        case GasMinerState.Idle:
          markup = this.Loc.GetString("gas-miner-state-idle-text");
          break;
        case GasMinerState.Working:
          markup = this.Loc.GetString("gas-miner-state-working-text");
          break;
        default:
          throw new IndexOutOfRangeException("MinerState");
      }
      examinedEvent.AddMarkup(markup);
    }
  }
}
