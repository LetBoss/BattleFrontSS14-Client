// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Mortar.CivMortarSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Mortar;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Mortar;

public sealed class CivMortarSystem : SharedCivMortarSystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  private const string AnimationKey = "civ_mortar_fire";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<CivMortarFiredEvent>(new EntityEventHandler<CivMortarFiredEvent>(this.OnMortarFiredEvent), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivMortarComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<CivMortarComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnMortarHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnMortarFiredEvent(CivMortarFiredEvent ev)
  {
    EntityUid? nullable;
    CivMortarComponent civMortarComponent;
    if (!this.TryGetEntity(ev.Mortar, ref nullable) || !this.TryComp<CivMortarComponent>(nullable, ref civMortarComponent) || this._animation.HasRunningAnimation(nullable.Value, "civ_mortar_fire"))
      return;
    this._animation.Play(nullable.Value, new Animation()
    {
      Length = civMortarComponent.AnimationTime,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) civMortarComponent.AnimationLayer,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(civMortarComponent.AnimationState), 0.0f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(civMortarComponent.DeployedState), 0.3f)
          }
        }
      }
    }, "civ_mortar_fire");
  }

  private void OnMortarHandleState(
    Entity<CivMortarComponent> mortar,
    ref AfterAutoHandleStateEvent args)
  {
    UserInterfaceComponent interfaceComponent;
    if (!this.TryComp<UserInterfaceComponent>(Entity<CivMortarComponent>.op_Implicit(mortar), ref interfaceComponent))
      return;
    foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
    {
      if (boundUserInterface is CivMortarBui civMortarBui)
        civMortarBui.Refresh();
    }
  }
}
