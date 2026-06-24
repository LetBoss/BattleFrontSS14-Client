// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.Ghillie.GhillieSuitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Armor.Ghillie;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedGhillieSuitSystem)})]
public sealed class GhillieSuitComponent : 
  Component,
  ISerializationGenerated<GhillieSuitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Opacity = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AddedOpacityOnShoot = 0.04f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UseDelay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan InvisibilityDelay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan InvisibilityBreakDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockFriendlyFire = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist Whitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId CloakEffect = (EntProtoId) "RMCEffectCloak";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionGhilliePreparePosition";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GhillieSuitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GhillieSuitComponent) target1;
    if (serialization.TryCustomCopy<GhillieSuitComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Opacity, ref target3, hookCtx, false, context))
      target3 = this.Opacity;
    target.Opacity = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AddedOpacityOnShoot, ref target4, hookCtx, false, context))
      target4 = this.AddedOpacityOnShoot;
    target.AddedOpacityOnShoot = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UseDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.UseDelay, hookCtx, context);
    target.UseDelay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.InvisibilityDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.InvisibilityDelay, hookCtx, context);
    target.InvisibilityDelay = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.InvisibilityBreakDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.InvisibilityBreakDelay, hookCtx, context);
    target.InvisibilityBreakDelay = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockFriendlyFire, ref target8, hookCtx, false, context))
      target8 = this.BlockFriendlyFire;
    target.BlockFriendlyFire = target8;
    EntityWhitelist target9 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target9, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target9 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target9, hookCtx, context, true);
    }
    target.Whitelist = target9;
    EntProtoId target10 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.CloakEffect, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId>(this.CloakEffect, hookCtx, context);
    target.CloakEffect = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GhillieSuitComponent target,
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
    GhillieSuitComponent target1 = (GhillieSuitComponent) target;
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
    GhillieSuitComponent target1 = (GhillieSuitComponent) target;
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
    GhillieSuitComponent target1 = (GhillieSuitComponent) target;
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
  virtual GhillieSuitComponent Component.Instantiate() => new GhillieSuitComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GhillieSuitComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Opacity;
    public float AddedOpacityOnShoot;
    public TimeSpan UseDelay;
    public TimeSpan InvisibilityDelay;
    public TimeSpan InvisibilityBreakDelay;
    public bool BlockFriendlyFire;
    public EntityWhitelist Whitelist;
    public EntProtoId CloakEffect;
    public EntProtoId ActionId;
    public NetEntity? Action;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GhillieSuitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GhillieSuitComponent, ComponentGetState>(new ComponentEventRefHandler<GhillieSuitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GhillieSuitComponent, ComponentHandleState>(new ComponentEventRefHandler<GhillieSuitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GhillieSuitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GhillieSuitComponent.GhillieSuitComponent_AutoState()
      {
        Enabled = component.Enabled,
        Opacity = component.Opacity,
        AddedOpacityOnShoot = component.AddedOpacityOnShoot,
        UseDelay = component.UseDelay,
        InvisibilityDelay = component.InvisibilityDelay,
        InvisibilityBreakDelay = component.InvisibilityBreakDelay,
        BlockFriendlyFire = component.BlockFriendlyFire,
        Whitelist = component.Whitelist,
        CloakEffect = component.CloakEffect,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GhillieSuitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GhillieSuitComponent.GhillieSuitComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Opacity = current.Opacity;
      component.AddedOpacityOnShoot = current.AddedOpacityOnShoot;
      component.UseDelay = current.UseDelay;
      component.InvisibilityDelay = current.InvisibilityDelay;
      component.InvisibilityBreakDelay = current.InvisibilityBreakDelay;
      component.BlockFriendlyFire = current.BlockFriendlyFire;
      component.Whitelist = current.Whitelist;
      component.CloakEffect = current.CloakEffect;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<GhillieSuitComponent>(current.Action, uid);
    }
  }
}
