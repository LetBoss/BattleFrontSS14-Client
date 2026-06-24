// Decompiled with JetBrains decompiler
// Type: Content.Client.Cuffs.CuffableSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Cuffs;

public sealed class CuffableSystem : SharedCuffableSystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, ComponentShutdown>(new ComponentEventHandler<CuffableComponent, ComponentShutdown>((object) this, __methodptr(OnCuffableShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, ComponentHandleState>(new ComponentEventRefHandler<CuffableComponent, ComponentHandleState>((object) this, __methodptr(OnCuffableHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnCuffableShutdown(
    EntityUid uid,
    CuffableComponent component,
    ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) HumanoidVisualLayers.Handcuffs, false);
  }

  private void OnCuffableHandleState(
    EntityUid uid,
    CuffableComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is CuffableComponentState current))
      return;
    component.CanStillInteract = current.CanStillInteract;
    this._actionBlocker.UpdateCanMove(uid);
    CuffedStateChangeEvent stateChangeEvent = new CuffedStateChangeEvent();
    this.RaiseLocalEvent<CuffedStateChangeEvent>(uid, ref stateChangeEvent, false);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    bool flag = current.NumHandsCuffed > 0;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) HumanoidVisualLayers.Handcuffs, flag);
    if (!flag)
      return;
    this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) HumanoidVisualLayers.Handcuffs, current.Color.Value);
    if (!object.Equals((object) component.CurrentRSI, (object) current.RSI) && current.RSI != null)
    {
      component.CurrentRSI = current.RSI;
      this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), this._sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) HumanoidVisualLayers.Handcuffs), new ResPath(component.CurrentRSI), new RSI.StateId?(RSI.StateId.op_Implicit(current.IconState)));
    }
    else
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) HumanoidVisualLayers.Handcuffs, RSI.StateId.op_Implicit(current.IconState));
  }
}
