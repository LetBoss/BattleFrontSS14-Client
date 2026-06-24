// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.RMCExplosiveDeleteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCExplosionSystem)})]
public sealed class RMCExplosiveDeleteComponent : 
  Component,
  ISerializationGenerated<RMCExplosiveDeleteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Delay = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BeepInterval = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? InitialBeepDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BeepSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DeleteWalls = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCExplosiveDeleteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCExplosiveDeleteComponent) target1;
    if (serialization.TryCustomCopy<RMCExplosiveDeleteComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target3, hookCtx, false, context))
      target3 = this.Delay;
    target.Delay = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BeepInterval, ref target4, hookCtx, false, context))
      target4 = this.BeepInterval;
    target.BeepInterval = target4;
    float? target5 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.InitialBeepDelay, ref target5, hookCtx, false, context))
      target5 = this.InitialBeepDelay;
    target.InitialBeepDelay = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BeepSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.BeepSound, hookCtx, context);
    target.BeepSound = target6;
    EntityWhitelist target7 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target7 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, context);
    }
    target.Whitelist = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteWalls, ref target8, hookCtx, false, context))
      target8 = this.DeleteWalls;
    target.DeleteWalls = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCExplosiveDeleteComponent target,
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
    RMCExplosiveDeleteComponent target1 = (RMCExplosiveDeleteComponent) target;
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
    RMCExplosiveDeleteComponent target1 = (RMCExplosiveDeleteComponent) target;
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
    RMCExplosiveDeleteComponent target1 = (RMCExplosiveDeleteComponent) target;
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
  virtual RMCExplosiveDeleteComponent Component.Instantiate() => new RMCExplosiveDeleteComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCExplosiveDeleteComponent_AutoState : IComponentState
  {
    public int Range;
    public float Delay;
    public float BeepInterval;
    public float? InitialBeepDelay;
    public SoundSpecifier? BeepSound;
    public EntityWhitelist? Whitelist;
    public bool DeleteWalls;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCExplosiveDeleteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCExplosiveDeleteComponent, ComponentGetState>(new ComponentEventRefHandler<RMCExplosiveDeleteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCExplosiveDeleteComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCExplosiveDeleteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCExplosiveDeleteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCExplosiveDeleteComponent.RMCExplosiveDeleteComponent_AutoState()
      {
        Range = component.Range,
        Delay = component.Delay,
        BeepInterval = component.BeepInterval,
        InitialBeepDelay = component.InitialBeepDelay,
        BeepSound = component.BeepSound,
        Whitelist = component.Whitelist,
        DeleteWalls = component.DeleteWalls
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCExplosiveDeleteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCExplosiveDeleteComponent.RMCExplosiveDeleteComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.Delay = current.Delay;
      component.BeepInterval = current.BeepInterval;
      component.InitialBeepDelay = current.InitialBeepDelay;
      component.BeepSound = current.BeepSound;
      component.Whitelist = current.Whitelist;
      component.DeleteWalls = current.DeleteWalls;
    }
  }
}
