// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Mortar.MortarSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Mortar;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Mortar;

public sealed class MortarSystem : SharedMortarSystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  private const string AnimationKey = "rmc_mortar_fire";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<MortarFiredEvent>(new EntityEventHandler<MortarFiredEvent>(this.OnMortarFiredEvent), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MortarComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MortarComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnMortarHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnMortarFiredEvent(MortarFiredEvent ev)
  {
    EntityUid? nullable;
    MortarComponent mortarComponent;
    if (!this.TryGetEntity(ev.Mortar, ref nullable) || !this.TryComp<MortarComponent>(nullable, ref mortarComponent) || this._animation.HasRunningAnimation(nullable.Value, "rmc_mortar_fire"))
      return;
    this._animation.Play(nullable.Value, new Animation()
    {
      Length = mortarComponent.AnimationTime,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) mortarComponent.AnimationLayer,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(mortarComponent.AnimationState), 0.0f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(mortarComponent.DeployedState), 0.3f)
          }
        }
      }
    }, "rmc_mortar_fire");
  }

  private void OnMortarHandleState(
    Entity<MortarComponent> mortar,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<MortarComponent>.op_Implicit(mortar), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is MortarBui mortarBui)
          mortarBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"MortarBui"}:\n{ex}");
    }
  }
}
