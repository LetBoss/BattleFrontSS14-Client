// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.Assorted.ParacusiaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Traits.Assorted;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedParacusiaSystem)})]
public sealed class ParacusiaComponent : 
  Component,
  ISerializationGenerated<ParacusiaComponent>,
  ISerializationGenerated
{
  [DataField("maxTimeBetweenIncidents", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float MaxTimeBetweenIncidents = 60f;
  [DataField("minTimeBetweenIncidents", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float MinTimeBetweenIncidents = 30f;
  [DataField("maxSoundDistance", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float MaxSoundDistance;
  [DataField("sounds", false, 1, true, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sounds;
  [DataField("timeBetweenIncidents", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan NextIncidentTime;
  public EntityUid? Stream;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParacusiaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParacusiaComponent) target1;
    if (serialization.TryCustomCopy<ParacusiaComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTimeBetweenIncidents, ref target2, hookCtx, false, context))
      target2 = this.MaxTimeBetweenIncidents;
    target.MaxTimeBetweenIncidents = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinTimeBetweenIncidents, ref target3, hookCtx, false, context))
      target3 = this.MinTimeBetweenIncidents;
    target.MinTimeBetweenIncidents = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxSoundDistance, ref target4, hookCtx, false, context))
      target4 = this.MaxSoundDistance;
    target.MaxSoundDistance = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sounds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sounds, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sounds, hookCtx, context);
    target.Sounds = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextIncidentTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextIncidentTime, hookCtx, context);
    target.NextIncidentTime = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParacusiaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ParacusiaComponent target1 = (ParacusiaComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ParacusiaComponent target1 = (ParacusiaComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ParacusiaComponent target1 = (ParacusiaComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ParacusiaComponent Component.Instantiate() => new ParacusiaComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ParacusiaComponent_AutoState : IComponentState
  {
    public float MaxTimeBetweenIncidents;
    public float MinTimeBetweenIncidents;
    public float MaxSoundDistance;
    public SoundSpecifier Sounds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ParacusiaComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ParacusiaComponent, ComponentGetState>(new ComponentEventRefHandler<ParacusiaComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ParacusiaComponent, ComponentHandleState>(new ComponentEventRefHandler<ParacusiaComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ParacusiaComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ParacusiaComponent.ParacusiaComponent_AutoState()
      {
        MaxTimeBetweenIncidents = component.MaxTimeBetweenIncidents,
        MinTimeBetweenIncidents = component.MinTimeBetweenIncidents,
        MaxSoundDistance = component.MaxSoundDistance,
        Sounds = component.Sounds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ParacusiaComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ParacusiaComponent.ParacusiaComponent_AutoState current))
        return;
      component.MaxTimeBetweenIncidents = current.MaxTimeBetweenIncidents;
      component.MinTimeBetweenIncidents = current.MinTimeBetweenIncidents;
      component.MaxSoundDistance = current.MaxSoundDistance;
      component.Sounds = current.Sounds;
    }
  }
}
