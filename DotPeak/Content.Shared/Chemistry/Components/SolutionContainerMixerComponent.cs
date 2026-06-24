// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionContainerMixerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSolutionContainerMixerSystem)})]
public sealed class SolutionContainerMixerComponent : 
  Component,
  ISerializationGenerated<SolutionContainerMixerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ContainerId = "mixer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Mixing;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan MixDuration;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan MixTimeEnd;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? MixingSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<AudioComponent>? MixingSoundEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionContainerMixerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionContainerMixerComponent) component;
    if (serialization.TryCustomCopy<SolutionContainerMixerComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref str, hookCtx, false, context))
      str = this.ContainerId;
    target.ContainerId = str;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Mixing, ref flag, hookCtx, false, context))
      flag = this.Mixing;
    target.Mixing = flag;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MixDuration, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.MixDuration, hookCtx, context, false);
    target.MixDuration = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MixTimeEnd, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.MixTimeEnd, hookCtx, context, false);
    target.MixTimeEnd = timeSpan2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MixingSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.MixingSound, hookCtx, context, false);
    target.MixingSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionContainerMixerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SolutionContainerMixerComponent target1 = (SolutionContainerMixerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SolutionContainerMixerComponent target1 = (SolutionContainerMixerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SolutionContainerMixerComponent target1 = (SolutionContainerMixerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SolutionContainerMixerComponent Component.Instantiate()
  {
    return new SolutionContainerMixerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SolutionContainerMixerComponent_AutoState : IComponentState
  {
    public bool Mixing;
    public TimeSpan MixDuration;
    public TimeSpan MixTimeEnd;
    public SoundSpecifier? MixingSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionContainerMixerComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionContainerMixerComponent, ComponentGetState>(new ComponentEventRefHandler<SolutionContainerMixerComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionContainerMixerComponent, ComponentHandleState>(new ComponentEventRefHandler<SolutionContainerMixerComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      SolutionContainerMixerComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SolutionContainerMixerComponent.SolutionContainerMixerComponent_AutoState()
      {
        Mixing = component.Mixing,
        MixDuration = component.MixDuration,
        MixTimeEnd = component.MixTimeEnd,
        MixingSound = component.MixingSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SolutionContainerMixerComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SolutionContainerMixerComponent.SolutionContainerMixerComponent_AutoState current))
        return;
      component.Mixing = current.Mixing;
      component.MixDuration = current.MixDuration;
      component.MixTimeEnd = current.MixTimeEnd;
      component.MixingSound = current.MixingSound;
    }
  }
}
