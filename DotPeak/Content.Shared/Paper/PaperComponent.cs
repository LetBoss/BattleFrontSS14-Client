// Decompiled with JetBrains decompiler
// Type: Content.Shared.Paper.PaperComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Paper;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PaperComponent : 
  Component,
  ISerializationGenerated<PaperComponent>,
  ISerializationGenerated
{
  public PaperComponent.PaperAction Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EditingDisabled;

  [DataField("content", false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Content { get; set; } = "";

  [DataField("contentSize", false, 1, false, false, null)]
  public int ContentSize { get; set; } = 10000;

  [DataField("stampedBy", false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<StampDisplayInfo> StampedBy { get; set; } = new List<StampDisplayInfo>();

  [DataField("stampState", false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? StampState { get; set; }

  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound { get; private set; } = (SoundSpecifier) new SoundCollectionSpecifier("PaperScribbles", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.1f))));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PaperComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PaperComponent) target1;
    if (serialization.TryCustomCopy<PaperComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Content == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Content, ref target2, hookCtx, false, context))
      target2 = this.Content;
    target.Content = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ContentSize, ref target3, hookCtx, false, context))
      target3 = this.ContentSize;
    target.ContentSize = target3;
    List<StampDisplayInfo> target4 = (List<StampDisplayInfo>) null;
    if (this.StampedBy == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<StampDisplayInfo>>(this.StampedBy, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<StampDisplayInfo>>(this.StampedBy, hookCtx, context);
    target.StampedBy = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StampState, ref target5, hookCtx, false, context))
      target5 = this.StampState;
    target.StampState = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.EditingDisabled, ref target6, hookCtx, false, context))
      target6 = this.EditingDisabled;
    target.EditingDisabled = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PaperComponent target,
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
    PaperComponent target1 = (PaperComponent) target;
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
    PaperComponent target1 = (PaperComponent) target;
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
    PaperComponent target1 = (PaperComponent) target;
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
  virtual PaperComponent Component.Instantiate() => new PaperComponent();

  [NetSerializable]
  [Serializable]
  public sealed class PaperBoundUserInterfaceState : BoundUserInterfaceState
  {
    public readonly string Text;
    public readonly List<StampDisplayInfo> StampedBy;
    public readonly PaperComponent.PaperAction Mode;

    public PaperBoundUserInterfaceState(
      string text,
      List<StampDisplayInfo> stampedBy,
      PaperComponent.PaperAction mode = PaperComponent.PaperAction.Read)
    {
      this.Text = text;
      this.StampedBy = stampedBy;
      this.Mode = mode;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PaperInputTextMessage : BoundUserInterfaceMessage
  {
    public readonly string Text;

    public PaperInputTextMessage(string text) => this.Text = text;
  }

  [NetSerializable]
  [Serializable]
  public sealed class PaperSignatureRequestMessage : BoundUserInterfaceMessage
  {
    public readonly int SignatureIndex;

    public PaperSignatureRequestMessage(int signatureIndex) => this.SignatureIndex = signatureIndex;
  }

  [NetSerializable]
  [Serializable]
  public enum PaperUiKey
  {
    Key,
  }

  [NetSerializable]
  [Serializable]
  public enum PaperAction
  {
    Read,
    Write,
  }

  [NetSerializable]
  [Serializable]
  public enum PaperVisuals : byte
  {
    Status,
    Stamp,
  }

  [NetSerializable]
  [Serializable]
  public enum PaperStatus : byte
  {
    Blank,
    Written,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PaperComponent_AutoState : IComponentState
  {
    public string Content;
    public List<StampDisplayInfo> StampedBy;
    public string? StampState;
    public bool EditingDisabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PaperComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PaperComponent, ComponentGetState>(new ComponentEventRefHandler<PaperComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PaperComponent, ComponentHandleState>(new ComponentEventRefHandler<PaperComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, PaperComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new PaperComponent.PaperComponent_AutoState()
      {
        Content = component.Content,
        StampedBy = component.StampedBy,
        StampState = component.StampState,
        EditingDisabled = component.EditingDisabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PaperComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PaperComponent.PaperComponent_AutoState current))
        return;
      component.Content = current.Content;
      component.StampedBy = current.StampedBy == null ? (List<StampDisplayInfo>) null : new List<StampDisplayInfo>((IEnumerable<StampDisplayInfo>) current.StampedBy);
      component.StampState = current.StampState;
      component.EditingDisabled = current.EditingDisabled;
    }
  }
}
