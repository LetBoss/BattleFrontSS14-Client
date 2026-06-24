// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.SharedHandheldLightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Light;

public abstract class SharedHandheldLightSystem : EntitySystem
{
  [Dependency]
  private SharedItemSystem _itemSys;
  [Dependency]
  private ClothingSystem _clothingSys;
  [Dependency]
  private SharedActionsSystem _actionSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HandheldLightComponent, ComponentInit>(new ComponentEventHandler<HandheldLightComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<HandheldLightComponent, ComponentHandleState>(new ComponentEventRefHandler<HandheldLightComponent, ComponentHandleState>(this.OnHandleState));
    this.SubscribeLocalEvent<HandheldLightComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<HandheldLightComponent, GetVerbsEvent<ActivationVerb>>(this.AddToggleLightVerb));
  }

  private void OnInit(EntityUid uid, HandheldLightComponent component, ComponentInit args)
  {
    this.UpdateVisuals(uid, component);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnHandleState(
    EntityUid uid,
    HandheldLightComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is HandheldLightComponent.HandheldLightComponentState current))
      return;
    component.Level = current.Charge;
    this.SetActivated(uid, current.Activated, component, false);
  }

  public void SetActivated(
    EntityUid uid,
    bool activated,
    HandheldLightComponent? component = null,
    bool makeNoise = true)
  {
    if (!this.Resolve<HandheldLightComponent>(uid, ref component) || component.Activated == activated)
      return;
    component.Activated = activated;
    if (makeNoise)
      this._audio.PlayPvs(component.Activated ? component.TurnOnSound : component.TurnOffSound, uid);
    this.Dirty(uid, (IComponent) component);
    this.UpdateVisuals(uid, component);
    LightToggleEvent args = new LightToggleEvent(activated);
    this.RaiseLocalEvent<LightToggleEvent>(uid, args);
  }

  public void UpdateVisuals(
    EntityUid uid,
    HandheldLightComponent? component = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<HandheldLightComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
      return;
    if (component.AddPrefix)
    {
      string str = component.Activated ? "on" : "off";
      this._itemSys.SetHeldPrefix(uid, str);
      this._clothingSys.SetEquippedPrefix(uid, str);
    }
    if (component.ToggleActionEntity.HasValue)
    {
      SharedActionsSystem actionSystem = this._actionSystem;
      EntityUid? toggleActionEntity = component.ToggleActionEntity;
      Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
      int num = component.Activated ? 1 : 0;
      actionSystem.SetToggled(action, num != 0);
    }
    this._appearance.SetData(uid, (Enum) ToggleableVisuals.Enabled, (object) component.Activated, appearance);
  }

  private void AddToggleLightVerb(
    Entity<HandheldLightComponent> ent,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !ent.Comp.ToggleOnInteract)
      return;
    GetVerbsEvent<ActivationVerb> @event = args;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Text = this.Loc.GetString("verb-common-toggle-light");
    activationVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/light.svg.192dpi.png"));
    activationVerb1.Act = ent.Comp.Activated ? (Action) (() => this.TurnOff(ent)) : (Action) (() => this.TurnOn(@event.User, ent));
    ActivationVerb activationVerb2 = activationVerb1;
    args.Verbs.Add(activationVerb2);
  }

  public abstract bool TurnOff(Entity<HandheldLightComponent> ent, bool makeNoise = true);

  public abstract bool TurnOn(EntityUid user, Entity<HandheldLightComponent> uid);
}
