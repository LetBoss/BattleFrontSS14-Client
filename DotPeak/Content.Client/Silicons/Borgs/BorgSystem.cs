// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.Borgs.BorgSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mobs;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Silicons.Borgs;

public sealed class BorgSystem : SharedBorgSystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BorgChassisComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<BorgChassisComponent, AppearanceChangeEvent>((object) this, __methodptr(OnBorgAppearanceChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MMIComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MMIComponent, AppearanceChangeEvent>((object) this, __methodptr(OnMMIAppearanceChanged)), (Type[]) null, (Type[]) null);
  }

  private void OnBorgAppearanceChanged(
    EntityUid uid,
    BorgChassisComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateBorgAppearance(uid, component, args.Component, args.Sprite);
  }

  protected override void OnInserted(
    EntityUid uid,
    BorgChassisComponent component,
    EntInsertedIntoContainerMessage args)
  {
    if (!component.Initialized)
      return;
    base.OnInserted(uid, component, args);
    this.UpdateBorgAppearance(uid, component);
  }

  protected override void OnRemoved(
    EntityUid uid,
    BorgChassisComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (!component.Initialized)
      return;
    base.OnRemoved(uid, component, args);
    this.UpdateBorgAppearance(uid, component);
  }

  private void UpdateBorgAppearance(
    EntityUid uid,
    BorgChassisComponent? component = null,
    AppearanceComponent? appearance = null,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<BorgChassisComponent, AppearanceComponent, SpriteComponent>(uid, ref component, ref appearance, ref sprite, true))
      return;
    MobState mobState;
    if (((SharedAppearanceSystem) this._appearance).TryGetData<MobState>(uid, (Enum) MobStateVisuals.State, ref mobState, appearance) && mobState != MobState.Alive)
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BorgVisualLayers.Light, false);
    }
    else
    {
      bool flag;
      if (!((SharedAppearanceSystem) this._appearance).TryGetData<bool>(uid, (Enum) BorgVisuals.HasPlayer, ref flag, appearance))
        flag = false;
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BorgVisualLayers.Light, component.BrainEntity.HasValue | flag);
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BorgVisualLayers.Light, RSI.StateId.op_Implicit(flag ? component.HasMindState : component.NoMindState));
    }
  }

  private void OnMMIAppearanceChanged(
    EntityUid uid,
    MMIComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    SpriteComponent sprite = args.Sprite;
    bool flag1;
    if (!((SharedAppearanceSystem) this._appearance).TryGetData<bool>(uid, (Enum) MMIVisuals.BrainPresent, ref flag1, (AppearanceComponent) null))
      flag1 = false;
    bool flag2;
    if (!((SharedAppearanceSystem) this._appearance).TryGetData<bool>(uid, (Enum) MMIVisuals.HasMind, ref flag2, (AppearanceComponent) null))
      flag2 = false;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) MMIVisualLayers.Brain, flag1);
    if (!flag1)
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) MMIVisualLayers.Base, RSI.StateId.op_Implicit(component.NoBrainState));
    }
    else
    {
      string str = flag2 ? component.HasMindState : component.NoMindState;
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) MMIVisualLayers.Base, RSI.StateId.op_Implicit(str));
    }
  }

  public void SetMindStates(
    Entity<BorgChassisComponent> borg,
    string hasMindState,
    string noMindState)
  {
    borg.Comp.HasMindState = hasMindState;
    borg.Comp.NoMindState = noMindState;
  }
}
