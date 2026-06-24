// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.UnpoweredFlashlightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Decals;
using Content.Shared.Emag.Systems;
using Content.Shared.Light.Components;
using Content.Shared.Mind.Components;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public sealed class UnpoweredFlashlightSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedPointLightSystem _light;
  [Dependency]
  private EmagSystem _emag;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<UnpoweredFlashlightComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<UnpoweredFlashlightComponent, GetVerbsEvent<ActivationVerb>>(this.AddToggleLightVerbs));
    this.SubscribeLocalEvent<UnpoweredFlashlightComponent, GetItemActionsEvent>(new ComponentEventHandler<UnpoweredFlashlightComponent, GetItemActionsEvent>(this.OnGetActions));
    this.SubscribeLocalEvent<UnpoweredFlashlightComponent, ToggleActionEvent>(new ComponentEventHandler<UnpoweredFlashlightComponent, ToggleActionEvent>(this.OnToggleAction));
    this.SubscribeLocalEvent<UnpoweredFlashlightComponent, MindAddedMessage>(new ComponentEventHandler<UnpoweredFlashlightComponent, MindAddedMessage>(this.OnMindAdded));
    this.SubscribeLocalEvent<UnpoweredFlashlightComponent, GotEmaggedEvent>(new ComponentEventRefHandler<UnpoweredFlashlightComponent, GotEmaggedEvent>(this.OnGotEmagged));
    this.SubscribeLocalEvent<UnpoweredFlashlightComponent, MapInitEvent>(new ComponentEventHandler<UnpoweredFlashlightComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnMapInit(EntityUid uid, UnpoweredFlashlightComponent component, MapInitEvent args)
  {
    this._actionContainer.EnsureAction(uid, ref component.ToggleActionEntity, (string) component.ToggleAction);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnToggleAction(
    EntityUid uid,
    UnpoweredFlashlightComponent component,
    ToggleActionEvent args)
  {
    if (args.Handled)
      return;
    this.TryToggleLight((Entity<UnpoweredFlashlightComponent>) (uid, component), new EntityUid?(args.Performer));
    args.Handled = true;
  }

  private void OnGetActions(
    EntityUid uid,
    UnpoweredFlashlightComponent component,
    GetItemActionsEvent args)
  {
    args.AddAction(component.ToggleActionEntity);
  }

  private void AddToggleLightVerbs(
    EntityUid uid,
    UnpoweredFlashlightComponent component,
    GetVerbsEvent<ActivationVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Text = this.Loc.GetString("toggle-flashlight-verb-get-data-text");
    activationVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/light.svg.192dpi.png"));
    activationVerb1.Act = (Action) (() => this.TryToggleLight((Entity<UnpoweredFlashlightComponent>) (uid, component), new EntityUid?(args.User)));
    activationVerb1.Priority = -1;
    ActivationVerb activationVerb2 = activationVerb1;
    args.Verbs.Add(activationVerb2);
  }

  private void OnMindAdded(
    EntityUid uid,
    UnpoweredFlashlightComponent component,
    MindAddedMessage args)
  {
    this._actionsSystem.AddAction(uid, ref component.ToggleActionEntity, (string) component.ToggleAction);
  }

  private void OnGotEmagged(
    EntityUid uid,
    UnpoweredFlashlightComponent component,
    ref GotEmaggedEvent args)
  {
    SharedPointLightComponent component1;
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || !this._light.TryGetLight(uid, out component1))
      return;
    ColorPalettePrototype prototype;
    if (this._prototypeManager.TryIndex<ColorPalettePrototype>(component.EmaggedColorsPrototype, out prototype))
    {
      Color color = this._random.Pick<Color>((IReadOnlyCollection<Color>) prototype.Colors.Values);
      this._light.SetColor(uid, color, component1);
    }
    args.Repeatable = true;
    args.Handled = true;
  }

  public void TryToggleLight(Entity<UnpoweredFlashlightComponent?> ent, EntityUid? user = null, bool quiet = false)
  {
    if (!this.Resolve<UnpoweredFlashlightComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    this.SetLight(ent, !ent.Comp.LightOn, user, quiet);
  }

  public void SetLight(
    Entity<UnpoweredFlashlightComponent?> ent,
    bool value,
    EntityUid? user = null,
    bool quiet = false)
  {
    SharedPointLightComponent component;
    if (!this.Resolve<UnpoweredFlashlightComponent>((EntityUid) ent, ref ent.Comp) || ent.Comp.LightOn == value || !this._light.TryGetLight((EntityUid) ent, out component))
      return;
    this.Dirty<UnpoweredFlashlightComponent>(ent);
    ent.Comp.LightOn = value;
    this._light.SetEnabled((EntityUid) ent, value, component);
    this._appearance.SetData((EntityUid) ent, (Enum) UnpoweredFlashlightVisuals.LightOn, (object) value);
    if (!quiet)
      this._audioSystem.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, user);
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? toggleActionEntity = ent.Comp.ToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = value ? 1 : 0;
    actionsSystem.SetToggled(action, num != 0);
    this.RaiseLocalEvent<LightToggleEvent>((EntityUid) ent, new LightToggleEvent(value));
  }
}
