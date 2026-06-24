// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.BatteryWeaponFireModesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class BatteryWeaponFireModesSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private AccessReaderSystem _accessReaderSystem;
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<BatteryWeaponFireModesComponent, UseInHandEvent>(new ComponentEventHandler<BatteryWeaponFireModesComponent, UseInHandEvent>(this.OnUseInHandEvent));
    this.SubscribeLocalEvent<BatteryWeaponFireModesComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<BatteryWeaponFireModesComponent, GetVerbsEvent<Verb>>(this.OnGetVerb));
    this.SubscribeLocalEvent<BatteryWeaponFireModesComponent, ExaminedEvent>(new ComponentEventHandler<BatteryWeaponFireModesComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(
    EntityUid uid,
    BatteryWeaponFireModesComponent component,
    ExaminedEvent args)
  {
    EntityPrototype prototype;
    if (component.FireModes.Count < 2 || !this._prototypeManager.TryIndex<EntityPrototype>((string) this.GetMode(component).Prototype, out prototype))
      return;
    args.PushMarkup(this.Loc.GetString("gun-set-fire-mode", ("mode", (object) prototype.Name)));
  }

  private BatteryWeaponFireMode GetMode(BatteryWeaponFireModesComponent component)
  {
    return component.FireModes[component.CurrentFireMode];
  }

  private void OnGetVerb(
    EntityUid uid,
    BatteryWeaponFireModesComponent component,
    GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || component.FireModes.Count < 2 || !this._accessReaderSystem.IsAllowed(args.User, uid))
      return;
    for (int index1 = 0; index1 < component.FireModes.Count; ++index1)
    {
      EntityPrototype entityPrototype = this._prototypeManager.Index<EntityPrototype>((string) component.FireModes[index1].Prototype);
      int index = index1;
      Verb verb = new Verb()
      {
        Priority = 1,
        Category = VerbCategory.SelectType,
        Text = entityPrototype.Name,
        Disabled = index1 == component.CurrentFireMode,
        Impact = LogImpact.Medium,
        DoContactInteraction = new bool?(true),
        Act = (Action) (() => this.TrySetFireMode(uid, component, index, new EntityUid?(args.User)))
      };
      args.Verbs.Add(verb);
    }
  }

  private void OnUseInHandEvent(
    EntityUid uid,
    BatteryWeaponFireModesComponent component,
    UseInHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.TryCycleFireMode(uid, component, new EntityUid?(args.User));
  }

  public void TryCycleFireMode(
    EntityUid uid,
    BatteryWeaponFireModesComponent component,
    EntityUid? user = null)
  {
    if (component.FireModes.Count < 2)
      return;
    int index = (component.CurrentFireMode + 1) % component.FireModes.Count;
    this.TrySetFireMode(uid, component, index, user);
  }

  public bool TrySetFireMode(
    EntityUid uid,
    BatteryWeaponFireModesComponent component,
    int index,
    EntityUid? user = null)
  {
    if (index < 0 || index >= component.FireModes.Count || user.HasValue && !this._accessReaderSystem.IsAllowed(user.Value, uid))
      return false;
    this.SetFireMode(uid, component, index, user);
    return true;
  }

  private void SetFireMode(
    EntityUid uid,
    BatteryWeaponFireModesComponent component,
    int index,
    EntityUid? user = null)
  {
    BatteryWeaponFireMode fireMode = component.FireModes[index];
    component.CurrentFireMode = index;
    this.Dirty(uid, (IComponent) component);
    EntityPrototype prototype;
    if (this._prototypeManager.TryIndex<EntityPrototype>((string) fireMode.Prototype, out prototype))
    {
      AppearanceComponent comp;
      if (this.TryComp<AppearanceComponent>(uid, out comp))
        this._appearanceSystem.SetData(uid, (Enum) BatteryWeaponFireModeVisuals.State, (object) prototype.ID, comp);
      if (user.HasValue)
        this._popupSystem.PopupClient(this.Loc.GetString("gun-set-fire-mode", ("mode", (object) prototype.Name)), uid, new EntityUid?(user.Value));
    }
    ProjectileBatteryAmmoProviderComponent comp1;
    if (!this.TryComp<ProjectileBatteryAmmoProviderComponent>(uid, out comp1))
      return;
    float fireCost = comp1.FireCost;
    comp1.Prototype = (string) fireMode.Prototype;
    comp1.FireCost = fireMode.FireCost;
    float num = fireMode.FireCost / fireCost;
    comp1.Shots = (int) Math.Round((double) comp1.Shots / (double) num);
    comp1.Capacity = (int) Math.Round((double) comp1.Capacity / (double) num);
    this.Dirty(uid, (IComponent) comp1);
    UpdateClientAmmoEvent args = new UpdateClientAmmoEvent();
    this.RaiseLocalEvent<UpdateClientAmmoEvent>(uid, ref args);
  }
}
