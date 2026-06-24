// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.TileFireOnTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlammableSystem)})]
public sealed class TileFireOnTriggerComponent : 
  Component,
  ISerializationGenerated<TileFireOnTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "RMCTileFire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/hit_on_shattered_glass.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? Intensity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? Duration;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TileFireOnTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TileFireOnTriggerComponent) target1;
    if (serialization.TryCustomCopy<TileFireOnTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Intensity, ref target5, hookCtx, false, context))
      target5 = this.Intensity;
    target.Intensity = target5;
    int? target6 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Duration, ref target6, hookCtx, false, context))
      target6 = this.Duration;
    target.Duration = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TileFireOnTriggerComponent target,
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
    TileFireOnTriggerComponent target1 = (TileFireOnTriggerComponent) target;
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
    TileFireOnTriggerComponent target1 = (TileFireOnTriggerComponent) target;
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
    TileFireOnTriggerComponent target1 = (TileFireOnTriggerComponent) target;
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
  virtual TileFireOnTriggerComponent Component.Instantiate() => new TileFireOnTriggerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TileFireOnTriggerComponent_AutoState : IComponentState
  {
    public int Range;
    public EntProtoId Spawn;
    public SoundSpecifier? Sound;
    public int? Intensity;
    public int? Duration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TileFireOnTriggerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TileFireOnTriggerComponent, ComponentGetState>(new ComponentEventRefHandler<TileFireOnTriggerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TileFireOnTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<TileFireOnTriggerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TileFireOnTriggerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TileFireOnTriggerComponent.TileFireOnTriggerComponent_AutoState()
      {
        Range = component.Range,
        Spawn = component.Spawn,
        Sound = component.Sound,
        Intensity = component.Intensity,
        Duration = component.Duration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TileFireOnTriggerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TileFireOnTriggerComponent.TileFireOnTriggerComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.Spawn = current.Spawn;
      component.Sound = current.Sound;
      component.Intensity = current.Intensity;
      component.Duration = current.Duration;
    }
  }
}
