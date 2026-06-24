// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Burrow.XenoBurrowComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Burrow;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoBurrowComponent : 
  Component,
  ISerializationGenerated<XenoBurrowComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Tunneling;
  [DataField(null, false, 1, false, false, null)]
  public float MaxTunnelingDistance = 15f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BurrowLength = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BurrowCooldown = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TunnelCooldown = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? NextTunnelAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? NextBurrowAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BurrowMaxDuration = TimeSpan.FromSeconds(9L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? ForcedUnburrowAt;
  [DataField(null, false, 1, false, false, null)]
  public float UnburrowStunRange = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnburrowStunLength = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MinimumTunnelTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BurrowDownSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/burrowing_b.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BurrowUpSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/burrowoff.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoBurrowComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoBurrowComponent) target1;
    if (serialization.TryCustomCopy<XenoBurrowComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Tunneling, ref target3, hookCtx, false, context))
      target3 = this.Tunneling;
    target.Tunneling = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTunnelingDistance, ref target4, hookCtx, false, context))
      target4 = this.MaxTunnelingDistance;
    target.MaxTunnelingDistance = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurrowLength, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.BurrowLength, hookCtx, context);
    target.BurrowLength = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurrowCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.BurrowCooldown, hookCtx, context);
    target.BurrowCooldown = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TunnelCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.TunnelCooldown, hookCtx, context);
    target.TunnelCooldown = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextTunnelAt, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.NextTunnelAt, hookCtx, context);
    target.NextTunnelAt = target8;
    TimeSpan? target9 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextBurrowAt, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan?>(this.NextBurrowAt, hookCtx, context);
    target.NextBurrowAt = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BurrowMaxDuration, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.BurrowMaxDuration, hookCtx, context);
    target.BurrowMaxDuration = target10;
    TimeSpan? target11 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ForcedUnburrowAt, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan?>(this.ForcedUnburrowAt, hookCtx, context);
    target.ForcedUnburrowAt = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UnburrowStunRange, ref target12, hookCtx, false, context))
      target12 = this.UnburrowStunRange;
    target.UnburrowStunRange = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnburrowStunLength, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.UnburrowStunLength, hookCtx, context);
    target.UnburrowStunLength = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinimumTunnelTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.MinimumTunnelTime, hookCtx, context);
    target.MinimumTunnelTime = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (this.BurrowDownSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BurrowDownSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.BurrowDownSound, hookCtx, context);
    target.BurrowDownSound = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (this.BurrowUpSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BurrowUpSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.BurrowUpSound, hookCtx, context);
    target.BurrowUpSound = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoBurrowComponent target,
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
    XenoBurrowComponent target1 = (XenoBurrowComponent) target;
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
    XenoBurrowComponent target1 = (XenoBurrowComponent) target;
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
    XenoBurrowComponent target1 = (XenoBurrowComponent) target;
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
  virtual XenoBurrowComponent Component.Instantiate() => new XenoBurrowComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoBurrowComponent_AutoState : IComponentState
  {
    public bool Active;
    public bool Tunneling;
    public TimeSpan? NextTunnelAt;
    public TimeSpan? NextBurrowAt;
    public TimeSpan? ForcedUnburrowAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoBurrowComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoBurrowComponent, ComponentGetState>(new ComponentEventRefHandler<XenoBurrowComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoBurrowComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoBurrowComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoBurrowComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoBurrowComponent.XenoBurrowComponent_AutoState()
      {
        Active = component.Active,
        Tunneling = component.Tunneling,
        NextTunnelAt = component.NextTunnelAt,
        NextBurrowAt = component.NextBurrowAt,
        ForcedUnburrowAt = component.ForcedUnburrowAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoBurrowComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoBurrowComponent.XenoBurrowComponent_AutoState current))
        return;
      component.Active = current.Active;
      component.Tunneling = current.Tunneling;
      component.NextTunnelAt = current.NextTunnelAt;
      component.NextBurrowAt = current.NextBurrowAt;
      component.ForcedUnburrowAt = current.ForcedUnburrowAt;
    }
  }
}
