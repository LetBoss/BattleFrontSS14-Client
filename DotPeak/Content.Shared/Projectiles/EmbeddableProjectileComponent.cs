// Decompiled with JetBrains decompiler
// Type: Content.Shared.Projectiles.EmbeddableProjectileComponent
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
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Projectiles;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EmbeddableProjectileComponent : 
  Component,
  ISerializationGenerated<EmbeddableProjectileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinimumSpeed = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DeleteOnRemove;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? RemovalTime = new float?(3f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EmbedOnThrow = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Offset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? EmbeddedIntoUid;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmbeddableProjectileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmbeddableProjectileComponent) target1;
    if (serialization.TryCustomCopy<EmbeddableProjectileComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumSpeed, ref target2, hookCtx, false, context))
      target2 = this.MinimumSpeed;
    target.MinimumSpeed = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnRemove, ref target3, hookCtx, false, context))
      target3 = this.DeleteOnRemove;
    target.DeleteOnRemove = target3;
    float? target4 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.RemovalTime, ref target4, hookCtx, false, context))
      target4 = this.RemovalTime;
    target.RemovalTime = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.EmbedOnThrow, ref target5, hookCtx, false, context))
      target5 = this.EmbedOnThrow;
    target.EmbedOnThrow = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.EmbeddedIntoUid, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.EmbeddedIntoUid, hookCtx, context);
    target.EmbeddedIntoUid = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmbeddableProjectileComponent target,
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
    EmbeddableProjectileComponent target1 = (EmbeddableProjectileComponent) target;
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
    EmbeddableProjectileComponent target1 = (EmbeddableProjectileComponent) target;
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
    EmbeddableProjectileComponent target1 = (EmbeddableProjectileComponent) target;
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
  virtual EmbeddableProjectileComponent Component.Instantiate()
  {
    return new EmbeddableProjectileComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EmbeddableProjectileComponent_AutoState : IComponentState
  {
    public float MinimumSpeed;
    public bool DeleteOnRemove;
    public float? RemovalTime;
    public bool EmbedOnThrow;
    public Vector2 Offset;
    public SoundSpecifier? Sound;
    public NetEntity? EmbeddedIntoUid;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EmbeddableProjectileComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EmbeddableProjectileComponent, ComponentGetState>(new ComponentEventRefHandler<EmbeddableProjectileComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EmbeddableProjectileComponent, ComponentHandleState>(new ComponentEventRefHandler<EmbeddableProjectileComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EmbeddableProjectileComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EmbeddableProjectileComponent.EmbeddableProjectileComponent_AutoState()
      {
        MinimumSpeed = component.MinimumSpeed,
        DeleteOnRemove = component.DeleteOnRemove,
        RemovalTime = component.RemovalTime,
        EmbedOnThrow = component.EmbedOnThrow,
        Offset = component.Offset,
        Sound = component.Sound,
        EmbeddedIntoUid = this.GetNetEntity(component.EmbeddedIntoUid)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EmbeddableProjectileComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EmbeddableProjectileComponent.EmbeddableProjectileComponent_AutoState current))
        return;
      component.MinimumSpeed = current.MinimumSpeed;
      component.DeleteOnRemove = current.DeleteOnRemove;
      component.RemovalTime = current.RemovalTime;
      component.EmbedOnThrow = current.EmbedOnThrow;
      component.Offset = current.Offset;
      component.Sound = current.Sound;
      component.EmbeddedIntoUid = this.EnsureEntity<EmbeddableProjectileComponent>(current.EmbeddedIntoUid, uid);
    }
  }
}
