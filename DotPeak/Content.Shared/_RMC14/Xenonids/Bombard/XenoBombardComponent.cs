// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Bombard.XenoBombardComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Xenonids.Bombard;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoBombardSystem)})]
public sealed class XenoBombardComponent : 
  Component,
  ISerializationGenerated<XenoBombardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 200;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(4.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Projectile = (EntProtoId) "XenoBombardAcidProjectile";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId[] Projectiles = new EntProtoId[2]
  {
    new EntProtoId("XenoBombardAcidProjectile"),
    new EntProtoId("XenoBombardNeurotoxinProjectile")
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier PrepareSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoDrool");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ShootSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/blobattack.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoBombardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoBombardComponent) target1;
    if (serialization.TryCustomCopy<XenoBombardComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Projectile, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Projectile, hookCtx, context);
    target.Projectile = target5;
    EntProtoId[] target6 = (EntProtoId[]) null;
    if (this.Projectiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntProtoId[]>(this.Projectiles, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<EntProtoId[]>(this.Projectiles, hookCtx, context);
    target.Projectiles = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.PrepareSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PrepareSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.PrepareSound, hookCtx, context);
    target.PrepareSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.ShootSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShootSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.ShootSound, hookCtx, context);
    target.ShootSound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoBombardComponent target,
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
    XenoBombardComponent target1 = (XenoBombardComponent) target;
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
    XenoBombardComponent target1 = (XenoBombardComponent) target;
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
    XenoBombardComponent target1 = (XenoBombardComponent) target;
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
  virtual XenoBombardComponent Component.Instantiate() => new XenoBombardComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoBombardComponent_AutoState : IComponentState
  {
    public int Range;
    public FixedPoint2 PlasmaCost;
    public TimeSpan Delay;
    public EntProtoId Projectile;
    public EntProtoId[] Projectiles;
    public SoundSpecifier PrepareSound;
    public SoundSpecifier ShootSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoBombardComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoBombardComponent, ComponentGetState>(new ComponentEventRefHandler<XenoBombardComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoBombardComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoBombardComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoBombardComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoBombardComponent.XenoBombardComponent_AutoState()
      {
        Range = component.Range,
        PlasmaCost = component.PlasmaCost,
        Delay = component.Delay,
        Projectile = component.Projectile,
        Projectiles = component.Projectiles,
        PrepareSound = component.PrepareSound,
        ShootSound = component.ShootSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoBombardComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoBombardComponent.XenoBombardComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.PlasmaCost = current.PlasmaCost;
      component.Delay = current.Delay;
      component.Projectile = current.Projectile;
      component.Projectiles = current.Projectiles;
      component.PrepareSound = current.PrepareSound;
      component.ShootSound = current.ShootSound;
    }
  }
}
