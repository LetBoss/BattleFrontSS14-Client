// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.DirectionalTileFireOnTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
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
public sealed class DirectionalTileFireOnTriggerComponent : 
  Component,
  ISerializationGenerated<DirectionalTileFireOnTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DiagonalRange = 1;
  [DataField(null, false, 1, false, false, null)]
  public int Width = 1;
  [DataField(null, false, 1, false, false, null)]
  public bool InitialSpread;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction Direction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Rebounded;
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
    ref DirectionalTileFireOnTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DirectionalTileFireOnTriggerComponent) target1;
    if (serialization.TryCustomCopy<DirectionalTileFireOnTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.DiagonalRange, ref target3, hookCtx, false, context))
      target3 = this.DiagonalRange;
    target.DiagonalRange = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Width, ref target4, hookCtx, false, context))
      target4 = this.Width;
    target.Width = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.InitialSpread, ref target5, hookCtx, false, context))
      target5 = this.InitialSpread;
    target.InitialSpread = target5;
    Direction target6 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.Direction, ref target6, hookCtx, false, context))
      target6 = this.Direction;
    target.Direction = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Rebounded, ref target7, hookCtx, false, context))
      target7 = this.Rebounded;
    target.Rebounded = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target9;
    int? target10 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Intensity, ref target10, hookCtx, false, context))
      target10 = this.Intensity;
    target.Intensity = target10;
    int? target11 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Duration, ref target11, hookCtx, false, context))
      target11 = this.Duration;
    target.Duration = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DirectionalTileFireOnTriggerComponent target,
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
    DirectionalTileFireOnTriggerComponent target1 = (DirectionalTileFireOnTriggerComponent) target;
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
    DirectionalTileFireOnTriggerComponent target1 = (DirectionalTileFireOnTriggerComponent) target;
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
    DirectionalTileFireOnTriggerComponent target1 = (DirectionalTileFireOnTriggerComponent) target;
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
  virtual DirectionalTileFireOnTriggerComponent Component.Instantiate()
  {
    return new DirectionalTileFireOnTriggerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DirectionalTileFireOnTriggerComponent_AutoState : IComponentState
  {
    public int Range;
    public int DiagonalRange;
    public Direction Direction;
    public bool Rebounded;
    public EntProtoId Spawn;
    public SoundSpecifier? Sound;
    public int? Intensity;
    public int? Duration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DirectionalTileFireOnTriggerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DirectionalTileFireOnTriggerComponent, ComponentGetState>(new ComponentEventRefHandler<DirectionalTileFireOnTriggerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DirectionalTileFireOnTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<DirectionalTileFireOnTriggerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DirectionalTileFireOnTriggerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DirectionalTileFireOnTriggerComponent.DirectionalTileFireOnTriggerComponent_AutoState()
      {
        Range = component.Range,
        DiagonalRange = component.DiagonalRange,
        Direction = component.Direction,
        Rebounded = component.Rebounded,
        Spawn = component.Spawn,
        Sound = component.Sound,
        Intensity = component.Intensity,
        Duration = component.Duration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DirectionalTileFireOnTriggerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DirectionalTileFireOnTriggerComponent.DirectionalTileFireOnTriggerComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.DiagonalRange = current.DiagonalRange;
      component.Direction = current.Direction;
      component.Rebounded = current.Rebounded;
      component.Spawn = current.Spawn;
      component.Sound = current.Sound;
      component.Intensity = current.Intensity;
      component.Duration = current.Duration;
    }
  }
}
