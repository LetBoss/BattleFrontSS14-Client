// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.HandheldLightSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Client.Light.Components;
using Content.Client.Light.EntitySystems;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Light;

public sealed class HandheldLightSystem : SharedHandheldLightSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private LightBehaviorSystem _lightBehavior;

  public override void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<HandheldLightComponent>((Func<Entity<HandheldLightComponent>, Control>) (ent => (Control) new HandheldLightStatus(Entity<HandheldLightComponent>.op_Implicit(ent))));
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandheldLightComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<HandheldLightComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  public override bool TurnOff(Entity<HandheldLightComponent> ent, bool makeNoise = true) => true;

  public override bool TurnOn(EntityUid user, Entity<HandheldLightComponent> uid) => true;

  private void OnAppearanceChange(
    EntityUid uid,
    HandheldLightComponent? component,
    ref AppearanceChangeEvent args)
  {
    bool flag;
    HandheldLightPowerStates lightPowerStates;
    LightBehaviourComponent behaviourComponent;
    if (!this.Resolve<HandheldLightComponent>(uid, ref component, true) || !this._appearance.TryGetData<bool>(uid, (Enum) ToggleableVisuals.Enabled, ref flag, args.Component) || !this._appearance.TryGetData<HandheldLightPowerStates>(uid, (Enum) HandheldLightVisuals.Power, ref lightPowerStates, args.Component) || !this.TryComp<LightBehaviourComponent>(uid, ref behaviourComponent))
      return;
    if (this._lightBehavior.HasRunningBehaviours(Entity<LightBehaviourComponent>.op_Implicit((uid, behaviourComponent))))
      this._lightBehavior.StopLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, behaviourComponent)), resetToOriginalSettings: true);
    if (!flag)
      return;
    switch (lightPowerStates)
    {
      case HandheldLightPowerStates.LowPower:
        this._lightBehavior.StartLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, behaviourComponent)), component.RadiatingBehaviourId);
        break;
      case HandheldLightPowerStates.Dying:
        this._lightBehavior.StartLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, behaviourComponent)), component.BlinkingBehaviourId);
        break;
    }
  }
}
