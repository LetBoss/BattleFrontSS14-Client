// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.CatchableComponent
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
namespace Content.Shared.Throwing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class CatchableComponent : 
  Component,
  ISerializationGenerated<CatchableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequireCombatMode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CatchChance = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? CatcherWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? CatchSuccessSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CatchableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CatchableComponent) target1;
    if (serialization.TryCustomCopy<CatchableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireCombatMode, ref target2, hookCtx, false, context))
      target2 = this.RequireCombatMode;
    target.RequireCombatMode = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CatchChance, ref target3, hookCtx, false, context))
      target3 = this.CatchChance;
    target.CatchChance = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.CatcherWhitelist, ref target4, hookCtx, false, context))
    {
      if (this.CatcherWhitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.CatcherWhitelist, ref target4, hookCtx, context);
    }
    target.CatcherWhitelist = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CatchSuccessSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.CatchSuccessSound, hookCtx, context);
    target.CatchSuccessSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CatchableComponent target,
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
    CatchableComponent target1 = (CatchableComponent) target;
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
    CatchableComponent target1 = (CatchableComponent) target;
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
    CatchableComponent target1 = (CatchableComponent) target;
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
  virtual CatchableComponent Component.Instantiate() => new CatchableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CatchableComponent_AutoState : IComponentState
  {
    public bool RequireCombatMode;
    public float CatchChance;
    public EntityWhitelist? CatcherWhitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CatchableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CatchableComponent, ComponentGetState>(new ComponentEventRefHandler<CatchableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CatchableComponent, ComponentHandleState>(new ComponentEventRefHandler<CatchableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CatchableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CatchableComponent.CatchableComponent_AutoState()
      {
        RequireCombatMode = component.RequireCombatMode,
        CatchChance = component.CatchChance,
        CatcherWhitelist = component.CatcherWhitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CatchableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CatchableComponent.CatchableComponent_AutoState current))
        return;
      component.RequireCombatMode = current.RequireCombatMode;
      component.CatchChance = current.CatchChance;
      component.CatcherWhitelist = current.CatcherWhitelist;
    }
  }
}
