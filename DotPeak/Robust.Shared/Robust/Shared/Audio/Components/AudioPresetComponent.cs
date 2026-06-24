// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Components.AudioPresetComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Audio.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[Access(new Type[] {typeof (SharedAudioSystem)})]
public sealed class AudioPresetComponent : 
  Robust.Shared.GameObjects.Component,
  ISerializationGenerated<AudioPresetComponent>,
  ISerializationGenerated,
  IComponentDelta,
  Robust.Shared.GameObjects.IComponent,
  ISerializationGenerated<Robust.Shared.GameObjects.IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [AutoNetworkedField]
  public string Preset = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AudioPresetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Robust.Shared.GameObjects.Component target1 = (Robust.Shared.GameObjects.Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AudioPresetComponent) target1;
    serialization.TryCustomCopy<AudioPresetComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AudioPresetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioPresetComponent target1 = (AudioPresetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioPresetComponent target1 = (AudioPresetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref Robust.Shared.GameObjects.IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioPresetComponent target1 = (AudioPresetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioPresetComponent target1 = (AudioPresetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AudioPresetComponent Robust.Shared.GameObjects.Component.Instantiate()
  {
    return new AudioPresetComponent();
  }

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AudioPresetComponent_AutoState : IComponentState
  {
    public string Preset;

    public AudioPresetComponent.AudioPresetComponent_AutoState ShallowClone()
    {
      return new AudioPresetComponent.AudioPresetComponent_AutoState()
      {
        Preset = this.Preset
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AudioPresetComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<AudioPresetComponent>("Preset");
      this.SubscribeLocalEvent<AudioPresetComponent, ComponentGetState>(new ComponentEventRefHandler<AudioPresetComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AudioPresetComponent, ComponentHandleState>(new ComponentEventRefHandler<AudioPresetComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AudioPresetComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick && this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick) == 1U)
        args.State = (IComponentState) new AudioPresetComponent.Preset_FieldComponentState()
        {
          Preset = component.Preset
        };
      else
        args.State = (IComponentState) new AudioPresetComponent.AudioPresetComponent_AutoState()
        {
          Preset = component.Preset
        };
    }

    private void OnHandleState(
      EntityUid uid,
      AudioPresetComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case AudioPresetComponent.Preset_FieldComponentState fieldComponentState:
          component.Preset = fieldComponentState.Preset;
          break;
        case AudioPresetComponent.AudioPresetComponent_AutoState componentAutoState:
          component.Preset = componentAutoState.Preset;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Preset_FieldComponentState : 
    IComponentDeltaState<AudioPresetComponent.AudioPresetComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string Preset;

    public void ApplyToFullState(
      AudioPresetComponent.AudioPresetComponent_AutoState fullState)
    {
      fullState.Preset = this.Preset;
    }

    public AudioPresetComponent.AudioPresetComponent_AutoState CreateNewFullState(
      AudioPresetComponent.AudioPresetComponent_AutoState fullState)
    {
      AudioPresetComponent.AudioPresetComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
