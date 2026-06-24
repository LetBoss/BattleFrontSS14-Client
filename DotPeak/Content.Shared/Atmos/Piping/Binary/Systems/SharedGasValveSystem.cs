// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Binary.Systems.SharedGasValveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.Piping.Binary.Systems;

public abstract class SharedGasValveSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasValveComponent, ComponentStartup>(new EntityEventRefHandler<GasValveComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasValveComponent, ActivateInWorldEvent>(new EntityEventRefHandler<GasValveComponent, ActivateInWorldEvent>((object) this, __methodptr(OnActivate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasValveComponent, ExaminedEvent>(new EntityEventRefHandler<GasValveComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(Entity<GasValveComponent> ent, ref ComponentStartup args)
  {
    this.Set(ent.Owner, ent.Comp, ent.Comp.Open);
  }

  public virtual void Set(EntityUid uid, GasValveComponent component, bool value)
  {
    component.Open = value;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    AppearanceComponent appearanceComponent;
    if (!this.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
      return;
    this._appearance.SetData(uid, (Enum) FilterVisuals.Enabled, (object) component.Open, appearanceComponent);
  }

  public void Toggle(EntityUid uid, GasValveComponent component)
  {
    this.Set(uid, component, !component.Open);
  }

  private void OnActivate(Entity<GasValveComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    this.Toggle(ent.Owner, ent.Comp);
    this._audio.PlayPredicted(ent.Comp.ValveSound, ent.Owner, new EntityUid?(args.User), new AudioParams?(((AudioParams) ref AudioParams.Default).WithVariation(new float?(0.25f))));
    args.Handled = true;
  }

  private void OnExamined(Entity<GasValveComponent> ent, ref ExaminedEvent args)
  {
    GasValveComponent comp = ent.Comp;
    string markup;
    if (!this.Transform(Entity<GasValveComponent>.op_Implicit(ent)).Anchored || !this.Loc.TryGetString("gas-valve-system-examined", ref markup, ("statusColor", comp.Open ? (object) "green" : (object) "orange"), ("open", (object) comp.Open)))
      return;
    args.PushMarkup(markup);
  }
}
