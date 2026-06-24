// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Plasma.XenoPlasmaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
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
namespace Content.Shared._RMC14.Xenonids.Plasma;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoPlasmaSystem)})]
public sealed class XenoPlasmaComponent : 
  Component,
  ISerializationGenerated<XenoPlasmaComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Plasma;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public int MaxPlasma = 300;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PlasmaTransferDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier PlasmaTransferSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoDrool");
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaRegenOnWeeds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaRegenOffWeeds = (FixedPoint2) 0.05;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AlertPrototype> Alert = (ProtoId<AlertPrototype>) "XenoPlasma";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoPlasmaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoPlasmaComponent) target1;
    if (serialization.TryCustomCopy<XenoPlasmaComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Plasma, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Plasma, hookCtx, context);
    target.Plasma = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPlasma, ref target3, hookCtx, false, context))
      target3 = this.MaxPlasma;
    target.MaxPlasma = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PlasmaTransferDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.PlasmaTransferDelay, hookCtx, context);
    target.PlasmaTransferDelay = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.PlasmaTransferSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PlasmaTransferSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.PlasmaTransferSound, hookCtx, context);
    target.PlasmaTransferSound = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaRegenOnWeeds, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.PlasmaRegenOnWeeds, hookCtx, context);
    target.PlasmaRegenOnWeeds = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaRegenOffWeeds, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.PlasmaRegenOffWeeds, hookCtx, context);
    target.PlasmaRegenOffWeeds = target7;
    ProtoId<AlertPrototype> target8 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.Alert, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.Alert, hookCtx, context);
    target.Alert = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoPlasmaComponent target,
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
    XenoPlasmaComponent target1 = (XenoPlasmaComponent) target;
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
    XenoPlasmaComponent target1 = (XenoPlasmaComponent) target;
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
    XenoPlasmaComponent target1 = (XenoPlasmaComponent) target;
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
  virtual XenoPlasmaComponent Component.Instantiate() => new XenoPlasmaComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoPlasmaComponent_AutoState : IComponentState
  {
    public FixedPoint2 Plasma;
    public int MaxPlasma;
    public TimeSpan PlasmaTransferDelay;
    public SoundSpecifier PlasmaTransferSound;
    public FixedPoint2 PlasmaRegenOnWeeds;
    public FixedPoint2 PlasmaRegenOffWeeds;
    public ProtoId<AlertPrototype> Alert;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoPlasmaComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoPlasmaComponent, ComponentGetState>(new ComponentEventRefHandler<XenoPlasmaComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoPlasmaComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoPlasmaComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoPlasmaComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoPlasmaComponent.XenoPlasmaComponent_AutoState()
      {
        Plasma = component.Plasma,
        MaxPlasma = component.MaxPlasma,
        PlasmaTransferDelay = component.PlasmaTransferDelay,
        PlasmaTransferSound = component.PlasmaTransferSound,
        PlasmaRegenOnWeeds = component.PlasmaRegenOnWeeds,
        PlasmaRegenOffWeeds = component.PlasmaRegenOffWeeds,
        Alert = component.Alert
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoPlasmaComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoPlasmaComponent.XenoPlasmaComponent_AutoState current))
        return;
      component.Plasma = current.Plasma;
      component.MaxPlasma = current.MaxPlasma;
      component.PlasmaTransferDelay = current.PlasmaTransferDelay;
      component.PlasmaTransferSound = current.PlasmaTransferSound;
      component.PlasmaRegenOnWeeds = current.PlasmaRegenOnWeeds;
      component.PlasmaRegenOffWeeds = current.PlasmaRegenOffWeeds;
      component.Alert = current.Alert;
    }
  }
}
