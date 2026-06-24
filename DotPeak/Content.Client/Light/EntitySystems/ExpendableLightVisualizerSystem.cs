// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.ExpendableLightVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Light.Components;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class ExpendableLightVisualizerSystem : VisualizerSystem<ExpendableLightComponent>
{
  [Dependency]
  private PointLightSystem _pointLightSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private LightBehaviorSystem _lightBehavior;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ExpendableLightComponent, ComponentShutdown>(new ComponentEventHandler<ExpendableLightComponent, ComponentShutdown>((object) this, __methodptr(OnLightShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnLightShutdown(
    EntityUid uid,
    ExpendableLightComponent component,
    ComponentShutdown args)
  {
    component.PlayingStream = this._audioSystem.Stop(component.PlayingStream, (AudioComponent) null);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ExpendableLightComponent comp,
    ref AppearanceChangeEvent args)
  {
    string id;
    LightBehaviourComponent behaviourComponent;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) ExpendableLightVisuals.Behavior, ref id, args.Component) && ((EntitySystem) this).TryComp<LightBehaviourComponent>(uid, ref behaviourComponent))
    {
      this._lightBehavior.StopLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, behaviourComponent)));
      if (!string.IsNullOrEmpty(id))
      {
        this._lightBehavior.StartLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, behaviourComponent)), id);
      }
      else
      {
        PointLightComponent pointLightComponent;
        if (((EntitySystem) this).TryComp<PointLightComponent>(uid, ref pointLightComponent))
          ((SharedPointLightSystem) this._pointLightSystem).SetEnabled(uid, false, (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
      }
    }
    ExpendableLightState expendableLightState;
    if (args.Sprite == null || !this.SpriteSystem.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ExpendableLightVisualLayers.Overlay) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<ExpendableLightState>(uid, (Enum) ExpendableLightVisuals.State, ref expendableLightState, args.Component))
      return;
    switch (expendableLightState)
    {
      case ExpendableLightState.Lit:
        this._audioSystem.Stop(comp.PlayingStream, (AudioComponent) null);
        ExpendableLightComponent expendableLightComponent = comp;
        (EntityUid, AudioComponent)? nullable1 = this._audioSystem.PlayPvs(comp.LoopedSound, uid, new AudioParams?());
        ref (EntityUid, AudioComponent)? local = ref nullable1;
        EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
        expendableLightComponent.PlayingStream = nullable2;
        int num1;
        if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ExpendableLightVisualLayers.Overlay, ref num1, true))
        {
          if (!string.IsNullOrWhiteSpace(comp.IconStateLit))
            this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, RSI.StateId.op_Implicit(comp.IconStateLit));
          if (!string.IsNullOrWhiteSpace(comp.SpriteShaderLit))
            args.Sprite.LayerSetShader(num1, comp.SpriteShaderLit);
          else
            args.Sprite.LayerSetShader(num1, (ShaderInstance) null, (string) null);
          if (comp.GlowColorLit.HasValue)
            this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, comp.GlowColorLit.Value);
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, true);
        }
        if (comp.GlowColorLit.HasValue)
          this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ExpendableLightVisualLayers.Glow, comp.GlowColorLit.Value);
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ExpendableLightVisualLayers.Glow, true);
        break;
      case ExpendableLightState.Dead:
        comp.PlayingStream = this._audioSystem.Stop(comp.PlayingStream, (AudioComponent) null);
        int num2;
        if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ExpendableLightVisualLayers.Overlay, ref num2, true))
        {
          if (!string.IsNullOrWhiteSpace(comp.IconStateSpent))
            this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(comp.IconStateSpent));
          if (!string.IsNullOrWhiteSpace(comp.SpriteShaderSpent))
            args.Sprite.LayerSetShader(num2, comp.SpriteShaderSpent);
          else
            args.Sprite.LayerSetShader(num2, (ShaderInstance) null, (string) null);
        }
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ExpendableLightVisualLayers.Glow, false);
        break;
    }
  }
}
