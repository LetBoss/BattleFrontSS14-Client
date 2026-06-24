// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.IntelCluesComponent
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
namespace Content.Shared._RMC14.Intel;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (IntelSystem)})]
public sealed class IntelCluesComponent : 
  Component,
  ISerializationGenerated<IntelCluesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string InitialArea = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Clues;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Clue = (LocId) "rmc-intel-clue-paper-scrap";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? Category;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntelCluesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntelCluesComponent) target1;
    if (serialization.TryCustomCopy<IntelCluesComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.InitialArea == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InitialArea, ref target2, hookCtx, false, context))
      target2 = this.InitialArea;
    target.InitialArea = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Clues, ref target3, hookCtx, false, context))
      target3 = this.Clues;
    target.Clues = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Clue, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.Clue, hookCtx, context);
    target.Clue = target4;
    LocId? target5 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Category, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId?>(this.Category, hookCtx, context);
    target.Category = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntelCluesComponent target,
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
    IntelCluesComponent target1 = (IntelCluesComponent) target;
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
    IntelCluesComponent target1 = (IntelCluesComponent) target;
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
    IntelCluesComponent target1 = (IntelCluesComponent) target;
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
  virtual IntelCluesComponent Component.Instantiate() => new IntelCluesComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IntelCluesComponent_AutoState : IComponentState
  {
    public string InitialArea;
    public int Clues;
    public LocId Clue;
    public LocId? Category;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IntelCluesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IntelCluesComponent, ComponentGetState>(new ComponentEventRefHandler<IntelCluesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IntelCluesComponent, ComponentHandleState>(new ComponentEventRefHandler<IntelCluesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IntelCluesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IntelCluesComponent.IntelCluesComponent_AutoState()
      {
        InitialArea = component.InitialArea,
        Clues = component.Clues,
        Clue = component.Clue,
        Category = component.Category
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IntelCluesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IntelCluesComponent.IntelCluesComponent_AutoState current))
        return;
      component.InitialArea = current.InitialArea;
      component.Clues = current.Clues;
      component.Clue = current.Clue;
      component.Category = current.Category;
    }
  }
}
