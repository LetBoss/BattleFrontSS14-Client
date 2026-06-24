// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.WiresPanelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Wires;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] {typeof (SharedWiresSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class WiresPanelComponent : 
  Component,
  ISerializationGenerated<WiresPanelComponent>,
  ISerializationGenerated
{
  [DataField("open", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Open;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool Visible = true;
  [DataField("screwdriverOpenSound", false, 1, false, false, null)]
  public SoundSpecifier ScrewdriverOpenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/screwdriveropen.ogg");
  [DataField("screwdriverCloseSound", false, 1, false, false, null)]
  public SoundSpecifier ScrewdriverCloseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/screwdriverclose.ogg");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan OpenDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> OpeningTool = (ProtoId<ToolQualityPrototype>) "Screwing";
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExamineTextClosed = (LocId?) "wires-panel-component-on-examine-closed";
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExamineTextOpen = (LocId?) "wires-panel-component-on-examine-open";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WiresPanelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WiresPanelComponent) target1;
    if (serialization.TryCustomCopy<WiresPanelComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Open, ref target2, hookCtx, false, context))
      target2 = this.Open;
    target.Open = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.ScrewdriverOpenSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScrewdriverOpenSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.ScrewdriverOpenSound, hookCtx, context);
    target.ScrewdriverOpenSound = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.ScrewdriverCloseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScrewdriverCloseSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.ScrewdriverCloseSound, hookCtx, context);
    target.ScrewdriverCloseSound = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OpenDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.OpenDelay, hookCtx, context);
    target.OpenDelay = target5;
    ProtoId<ToolQualityPrototype> target6 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.OpeningTool, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.OpeningTool, hookCtx, context);
    target.OpeningTool = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExamineTextClosed, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.ExamineTextClosed, hookCtx, context);
    target.ExamineTextClosed = target7;
    LocId? target8 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExamineTextOpen, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId?>(this.ExamineTextOpen, hookCtx, context);
    target.ExamineTextOpen = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WiresPanelComponent target,
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
    WiresPanelComponent target1 = (WiresPanelComponent) target;
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
    WiresPanelComponent target1 = (WiresPanelComponent) target;
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
    WiresPanelComponent target1 = (WiresPanelComponent) target;
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
  virtual WiresPanelComponent Component.Instantiate() => new WiresPanelComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WiresPanelComponent_AutoState : IComponentState
  {
    public bool Open;
    public bool Visible;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WiresPanelComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WiresPanelComponent, ComponentGetState>(new ComponentEventRefHandler<WiresPanelComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WiresPanelComponent, ComponentHandleState>(new ComponentEventRefHandler<WiresPanelComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WiresPanelComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WiresPanelComponent.WiresPanelComponent_AutoState()
      {
        Open = component.Open,
        Visible = component.Visible
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WiresPanelComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WiresPanelComponent.WiresPanelComponent_AutoState current))
        return;
      component.Open = current.Open;
      component.Visible = current.Visible;
    }
  }
}
