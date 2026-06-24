// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.DumpableComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class DumpableComponent : 
  Component,
  ISerializationGenerated<DumpableComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundDump", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DumpSound = (SoundSpecifier) new SoundCollectionSpecifier("storageRustle");
  [DataField("delayPerItem", false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DelayPerItem = TimeSpan.FromSeconds(0.075000002980232239);
  [DataField("multiplier", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Multiplier = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DumpableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DumpableComponent) target1;
    if (serialization.TryCustomCopy<DumpableComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DumpSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.DumpSound, hookCtx, context);
    target.DumpSound = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DelayPerItem, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DelayPerItem, hookCtx, context);
    target.DelayPerItem = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Multiplier, ref target4, hookCtx, false, context))
      target4 = this.Multiplier;
    target.Multiplier = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DumpableComponent target,
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
    DumpableComponent target1 = (DumpableComponent) target;
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
    DumpableComponent target1 = (DumpableComponent) target;
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
    DumpableComponent target1 = (DumpableComponent) target;
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
  virtual DumpableComponent Component.Instantiate() => new DumpableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DumpableComponent_AutoState : IComponentState
  {
    public SoundSpecifier? DumpSound;
    public TimeSpan DelayPerItem;
    public float Multiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DumpableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DumpableComponent, ComponentGetState>(new ComponentEventRefHandler<DumpableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DumpableComponent, ComponentHandleState>(new ComponentEventRefHandler<DumpableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, DumpableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DumpableComponent.DumpableComponent_AutoState()
      {
        DumpSound = component.DumpSound,
        DelayPerItem = component.DelayPerItem,
        Multiplier = component.Multiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DumpableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DumpableComponent.DumpableComponent_AutoState current))
        return;
      component.DumpSound = current.DumpSound;
      component.DelayPerItem = current.DelayPerItem;
      component.Multiplier = current.Multiplier;
    }
  }
}
