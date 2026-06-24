// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Bioscan.BioscanComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Bioscan;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (BioscanSystem)})]
public sealed class BioscanComponent : 
  Component,
  ISerializationGenerated<BioscanComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextCheck;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxMarinesAlive;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxXenoAlive;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastMarine;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier MarineSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/ARES/bioscan.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6f)));
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastXeno;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier XenoSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoQueenCommand", new AudioParams?(AudioParams.Default.WithVolume(-6f)));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BioscanComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BioscanComponent) target1;
    if (serialization.TryCustomCopy<BioscanComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextCheck, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.NextCheck, hookCtx, context);
    target.NextCheck = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxMarinesAlive, ref target3, hookCtx, false, context))
      target3 = this.MaxMarinesAlive;
    target.MaxMarinesAlive = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxXenoAlive, ref target4, hookCtx, false, context))
      target4 = this.MaxXenoAlive;
    target.MaxXenoAlive = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastMarine, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.LastMarine, hookCtx, context);
    target.LastMarine = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.MarineSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MarineSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.MarineSound, hookCtx, context);
    target.MarineSound = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastXeno, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.LastXeno, hookCtx, context);
    target.LastXeno = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.XenoSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.XenoSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.XenoSound, hookCtx, context);
    target.XenoSound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BioscanComponent target,
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
    BioscanComponent target1 = (BioscanComponent) target;
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
    BioscanComponent target1 = (BioscanComponent) target;
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
    BioscanComponent target1 = (BioscanComponent) target;
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
  virtual BioscanComponent Component.Instantiate() => new BioscanComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BioscanComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BioscanComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<BioscanComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      BioscanComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextCheck += args.PausedTime;
      component.LastMarine += args.PausedTime;
      component.LastXeno += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BioscanComponent_AutoState : IComponentState
  {
    public TimeSpan NextCheck;
    public int MaxMarinesAlive;
    public int MaxXenoAlive;
    public TimeSpan LastMarine;
    public 
    #nullable enable
    SoundSpecifier MarineSound;
    public TimeSpan LastXeno;
    public SoundSpecifier XenoSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BioscanComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BioscanComponent, ComponentGetState>(new ComponentEventRefHandler<BioscanComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BioscanComponent, ComponentHandleState>(new ComponentEventRefHandler<BioscanComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, BioscanComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new BioscanComponent.BioscanComponent_AutoState()
      {
        NextCheck = component.NextCheck,
        MaxMarinesAlive = component.MaxMarinesAlive,
        MaxXenoAlive = component.MaxXenoAlive,
        LastMarine = component.LastMarine,
        MarineSound = component.MarineSound,
        LastXeno = component.LastXeno,
        XenoSound = component.XenoSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BioscanComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BioscanComponent.BioscanComponent_AutoState current))
        return;
      component.NextCheck = current.NextCheck;
      component.MaxMarinesAlive = current.MaxMarinesAlive;
      component.MaxXenoAlive = current.MaxXenoAlive;
      component.LastMarine = current.LastMarine;
      component.MarineSound = current.MarineSound;
      component.LastXeno = current.LastXeno;
      component.XenoSound = current.XenoSound;
    }
  }
}
