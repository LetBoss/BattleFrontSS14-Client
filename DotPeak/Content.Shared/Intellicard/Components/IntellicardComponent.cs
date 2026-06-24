// Decompiled with JetBrains decompiler
// Type: Content.Shared.Intellicard.IntellicardComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Intellicard;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class IntellicardComponent : 
  Component,
  ISerializationGenerated<IntellicardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DownloadTime = 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int UploadTime = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? WarningSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Misc/notice2.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WarningDelay = TimeSpan.FromSeconds(8L);
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextWarningAllowed = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntellicardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntellicardComponent) target1;
    if (serialization.TryCustomCopy<IntellicardComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.DownloadTime, ref target2, hookCtx, false, context))
      target2 = this.DownloadTime;
    target.DownloadTime = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.UploadTime, ref target3, hookCtx, false, context))
      target3 = this.UploadTime;
    target.UploadTime = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WarningSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.WarningSound, hookCtx, context);
    target.WarningSound = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarningDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.WarningDelay, hookCtx, context);
    target.WarningDelay = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntellicardComponent target,
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
    IntellicardComponent target1 = (IntellicardComponent) target;
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
    IntellicardComponent target1 = (IntellicardComponent) target;
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
    IntellicardComponent target1 = (IntellicardComponent) target;
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
  virtual IntellicardComponent Component.Instantiate() => new IntellicardComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IntellicardComponent_AutoState : IComponentState
  {
    public int DownloadTime;
    public int UploadTime;
    public SoundSpecifier? WarningSound;
    public TimeSpan WarningDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IntellicardComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IntellicardComponent, ComponentGetState>(new ComponentEventRefHandler<IntellicardComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IntellicardComponent, ComponentHandleState>(new ComponentEventRefHandler<IntellicardComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IntellicardComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IntellicardComponent.IntellicardComponent_AutoState()
      {
        DownloadTime = component.DownloadTime,
        UploadTime = component.UploadTime,
        WarningSound = component.WarningSound,
        WarningDelay = component.WarningDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IntellicardComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IntellicardComponent.IntellicardComponent_AutoState current))
        return;
      component.DownloadTime = current.DownloadTime;
      component.UploadTime = current.UploadTime;
      component.WarningSound = current.WarningSound;
      component.WarningDelay = current.WarningDelay;
    }
  }
}
