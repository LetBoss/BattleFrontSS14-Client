// Decompiled with JetBrains decompiler
// Type: Content.Shared.FingerprintReader.FingerprintReaderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.FingerprintReader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (FingerprintReaderSystem)})]
public sealed class FingerprintReaderComponent : 
  Component,
  ISerializationGenerated<FingerprintReaderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> AllowedFingerprints = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreGloves;
  [DataField(null, false, 1, false, false, null)]
  public LocId? FailPopup;
  [DataField(null, false, 1, false, false, null)]
  public LocId? FailGlovesPopup;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FingerprintReaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FingerprintReaderComponent) target1;
    if (serialization.TryCustomCopy<FingerprintReaderComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<string> target2 = (HashSet<string>) null;
    if (this.AllowedFingerprints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.AllowedFingerprints, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<string>>(this.AllowedFingerprints, hookCtx, context);
    target.AllowedFingerprints = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreGloves, ref target3, hookCtx, false, context))
      target3 = this.IgnoreGloves;
    target.IgnoreGloves = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.FailPopup, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.FailPopup, hookCtx, context);
    target.FailPopup = target4;
    LocId? target5 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.FailGlovesPopup, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId?>(this.FailGlovesPopup, hookCtx, context);
    target.FailGlovesPopup = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FingerprintReaderComponent target,
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
    FingerprintReaderComponent target1 = (FingerprintReaderComponent) target;
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
    FingerprintReaderComponent target1 = (FingerprintReaderComponent) target;
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
    FingerprintReaderComponent target1 = (FingerprintReaderComponent) target;
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
  virtual FingerprintReaderComponent Component.Instantiate() => new FingerprintReaderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FingerprintReaderComponent_AutoState : IComponentState
  {
    public HashSet<string> AllowedFingerprints;
    public bool IgnoreGloves;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FingerprintReaderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FingerprintReaderComponent, ComponentGetState>(new ComponentEventRefHandler<FingerprintReaderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FingerprintReaderComponent, ComponentHandleState>(new ComponentEventRefHandler<FingerprintReaderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FingerprintReaderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FingerprintReaderComponent.FingerprintReaderComponent_AutoState()
      {
        AllowedFingerprints = component.AllowedFingerprints,
        IgnoreGloves = component.IgnoreGloves
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FingerprintReaderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FingerprintReaderComponent.FingerprintReaderComponent_AutoState current))
        return;
      component.AllowedFingerprints = current.AllowedFingerprints == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.AllowedFingerprints);
      component.IgnoreGloves = current.IgnoreGloves;
    }
  }
}
