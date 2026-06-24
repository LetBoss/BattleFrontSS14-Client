// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vents.VentCrawlerComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vents;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VentCrawlerComponent : 
  Component,
  ISerializationGenerated<VentCrawlerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VentCrawlDelay = TimeSpan.FromMilliseconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VentEnterDelay = TimeSpan.FromSeconds(4.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VentExitDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VentCrawlSoundDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string VentCrawlIcon = "unknown";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId VentCrawlExamine = (LocId) "rmc-vent-crawling-entrance-xeno";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VentCrawlerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VentCrawlerComponent) target1;
    if (serialization.TryCustomCopy<VentCrawlerComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VentCrawlDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.VentCrawlDelay, hookCtx, context);
    target.VentCrawlDelay = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VentEnterDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.VentEnterDelay, hookCtx, context);
    target.VentEnterDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VentExitDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.VentExitDelay, hookCtx, context);
    target.VentExitDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VentCrawlSoundDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.VentCrawlSoundDelay, hookCtx, context);
    target.VentCrawlSoundDelay = target5;
    string target6 = (string) null;
    if (this.VentCrawlIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VentCrawlIcon, ref target6, hookCtx, false, context))
      target6 = this.VentCrawlIcon;
    target.VentCrawlIcon = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VentCrawlExamine, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.VentCrawlExamine, hookCtx, context);
    target.VentCrawlExamine = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VentCrawlerComponent target,
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
    VentCrawlerComponent target1 = (VentCrawlerComponent) target;
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
    VentCrawlerComponent target1 = (VentCrawlerComponent) target;
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
    VentCrawlerComponent target1 = (VentCrawlerComponent) target;
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
  virtual VentCrawlerComponent Component.Instantiate() => new VentCrawlerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VentCrawlerComponent_AutoState : IComponentState
  {
    public TimeSpan VentCrawlDelay;
    public TimeSpan VentEnterDelay;
    public TimeSpan VentExitDelay;
    public TimeSpan VentCrawlSoundDelay;
    public string VentCrawlIcon;
    public LocId VentCrawlExamine;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VentCrawlerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VentCrawlerComponent, ComponentGetState>(new ComponentEventRefHandler<VentCrawlerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VentCrawlerComponent, ComponentHandleState>(new ComponentEventRefHandler<VentCrawlerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VentCrawlerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VentCrawlerComponent.VentCrawlerComponent_AutoState()
      {
        VentCrawlDelay = component.VentCrawlDelay,
        VentEnterDelay = component.VentEnterDelay,
        VentExitDelay = component.VentExitDelay,
        VentCrawlSoundDelay = component.VentCrawlSoundDelay,
        VentCrawlIcon = component.VentCrawlIcon,
        VentCrawlExamine = component.VentCrawlExamine
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VentCrawlerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VentCrawlerComponent.VentCrawlerComponent_AutoState current))
        return;
      component.VentCrawlDelay = current.VentCrawlDelay;
      component.VentEnterDelay = current.VentEnterDelay;
      component.VentExitDelay = current.VentExitDelay;
      component.VentCrawlSoundDelay = current.VentCrawlSoundDelay;
      component.VentCrawlIcon = current.VentCrawlIcon;
      component.VentCrawlExamine = current.VentCrawlExamine;
    }
  }
}
