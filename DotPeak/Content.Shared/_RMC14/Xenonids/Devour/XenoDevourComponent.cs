// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Devour.XenoDevourComponent
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
namespace Content.Shared._RMC14.Xenonids.Devour;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoDevourSystem)})]
public sealed class XenoDevourComponent : 
  Component,
  ISerializationGenerated<XenoDevourComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DevourDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DevourContainerId = "cm_xeno_devour";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier RegurgitateSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoDrool");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WarnAfter = TimeSpan.FromSeconds(50L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RegurgitateAfter = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RegurgitationStun = TimeSpan.FromSeconds(4L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDevourComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDevourComponent) target1;
    if (serialization.TryCustomCopy<XenoDevourComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DevourDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DevourDelay, hookCtx, context);
    target.DevourDelay = target2;
    string target3 = (string) null;
    if (this.DevourContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DevourContainerId, ref target3, hookCtx, false, context))
      target3 = this.DevourContainerId;
    target.DevourContainerId = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.RegurgitateSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RegurgitateSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.RegurgitateSound, hookCtx, context);
    target.RegurgitateSound = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarnAfter, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.WarnAfter, hookCtx, context);
    target.WarnAfter = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RegurgitateAfter, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.RegurgitateAfter, hookCtx, context);
    target.RegurgitateAfter = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RegurgitationStun, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.RegurgitationStun, hookCtx, context);
    target.RegurgitationStun = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDevourComponent target,
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
    XenoDevourComponent target1 = (XenoDevourComponent) target;
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
    XenoDevourComponent target1 = (XenoDevourComponent) target;
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
    XenoDevourComponent target1 = (XenoDevourComponent) target;
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
  virtual XenoDevourComponent Component.Instantiate() => new XenoDevourComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoDevourComponent_AutoState : IComponentState
  {
    public TimeSpan DevourDelay;
    public string DevourContainerId;
    public SoundSpecifier RegurgitateSound;
    public TimeSpan WarnAfter;
    public TimeSpan RegurgitateAfter;
    public TimeSpan RegurgitationStun;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoDevourComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoDevourComponent, ComponentGetState>(new ComponentEventRefHandler<XenoDevourComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoDevourComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoDevourComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoDevourComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoDevourComponent.XenoDevourComponent_AutoState()
      {
        DevourDelay = component.DevourDelay,
        DevourContainerId = component.DevourContainerId,
        RegurgitateSound = component.RegurgitateSound,
        WarnAfter = component.WarnAfter,
        RegurgitateAfter = component.RegurgitateAfter,
        RegurgitationStun = component.RegurgitationStun
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoDevourComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoDevourComponent.XenoDevourComponent_AutoState current))
        return;
      component.DevourDelay = current.DevourDelay;
      component.DevourContainerId = current.DevourContainerId;
      component.RegurgitateSound = current.RegurgitateSound;
      component.WarnAfter = current.WarnAfter;
      component.RegurgitateAfter = current.RegurgitateAfter;
      component.RegurgitationStun = current.RegurgitationStun;
    }
  }
}
