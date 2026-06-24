// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.MultipleToolComponent
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
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class MultipleToolComponent : 
  Component,
  ISerializationGenerated<MultipleToolComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public uint CurrentEntry;
  [Robust.Shared.ViewVariables.ViewVariables]
  public string CurrentQualityName = string.Empty;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool UiUpdateNeeded;
  [DataField(null, false, 1, false, false, null)]
  public bool StatusShowBehavior = true;

  [DataField(null, false, 1, true, false, null)]
  public MultipleToolComponent.ToolEntry[] Entries { get; private set; } = Array.Empty<MultipleToolComponent.ToolEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MultipleToolComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MultipleToolComponent) target1;
    if (serialization.TryCustomCopy<MultipleToolComponent>(this, ref target, hookCtx, false, context))
      return;
    MultipleToolComponent.ToolEntry[] target2 = (MultipleToolComponent.ToolEntry[]) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<MultipleToolComponent.ToolEntry[]>(this.Entries, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<MultipleToolComponent.ToolEntry[]>(this.Entries, hookCtx, context);
    target.Entries = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.StatusShowBehavior, ref target3, hookCtx, false, context))
      target3 = this.StatusShowBehavior;
    target.StatusShowBehavior = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MultipleToolComponent target,
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
    MultipleToolComponent target1 = (MultipleToolComponent) target;
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
    MultipleToolComponent target1 = (MultipleToolComponent) target;
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
    MultipleToolComponent target1 = (MultipleToolComponent) target;
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
  virtual MultipleToolComponent Component.Instantiate() => new MultipleToolComponent();

  [DataDefinition]
  public sealed class ToolEntry : 
    ISerializationGenerated<MultipleToolComponent.ToolEntry>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, true, false, null)]
    public PrototypeFlags<ToolQualityPrototype> Behavior = new PrototypeFlags<ToolQualityPrototype>();
    [DataField(null, false, 1, false, false, null)]
    public SoundSpecifier? UseSound;
    [DataField(null, false, 1, false, false, null)]
    public SoundSpecifier? ChangeSound;
    [DataField(null, false, 1, false, false, null)]
    public SpriteSpecifier? Sprite;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref MultipleToolComponent.ToolEntry target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<MultipleToolComponent.ToolEntry>(this, ref target, hookCtx, false, context))
        return;
      PrototypeFlags<ToolQualityPrototype> target1 = (PrototypeFlags<ToolQualityPrototype>) null;
      if (this.Behavior == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<PrototypeFlags<ToolQualityPrototype>>(this.Behavior, ref target1, hookCtx, false, context))
        target1 = serialization.CreateCopy<PrototypeFlags<ToolQualityPrototype>>(this.Behavior, hookCtx, context);
      target.Behavior = target1;
      SoundSpecifier target2 = (SoundSpecifier) null;
      if (!serialization.TryCustomCopy<SoundSpecifier>(this.UseSound, ref target2, hookCtx, true, context))
        target2 = serialization.CreateCopy<SoundSpecifier>(this.UseSound, hookCtx, context);
      target.UseSound = target2;
      SoundSpecifier target3 = (SoundSpecifier) null;
      if (!serialization.TryCustomCopy<SoundSpecifier>(this.ChangeSound, ref target3, hookCtx, true, context))
        target3 = serialization.CreateCopy<SoundSpecifier>(this.ChangeSound, hookCtx, context);
      target.ChangeSound = target3;
      SpriteSpecifier target4 = (SpriteSpecifier) null;
      if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Sprite, ref target4, hookCtx, true, context))
        target4 = serialization.CreateCopy<SpriteSpecifier>(this.Sprite, hookCtx, context);
      target.Sprite = target4;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref MultipleToolComponent.ToolEntry target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      MultipleToolComponent.ToolEntry target1 = (MultipleToolComponent.ToolEntry) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public MultipleToolComponent.ToolEntry Instantiate() => new MultipleToolComponent.ToolEntry();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MultipleToolComponent_AutoState : IComponentState
  {
    public uint CurrentEntry;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MultipleToolComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MultipleToolComponent, ComponentGetState>(new ComponentEventRefHandler<MultipleToolComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MultipleToolComponent, ComponentHandleState>(new ComponentEventRefHandler<MultipleToolComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MultipleToolComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MultipleToolComponent.MultipleToolComponent_AutoState()
      {
        CurrentEntry = component.CurrentEntry
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MultipleToolComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MultipleToolComponent.MultipleToolComponent_AutoState current))
        return;
      component.CurrentEntry = current.CurrentEntry;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, MultipleToolComponent>(uid, component, ref args1);
    }
  }
}
