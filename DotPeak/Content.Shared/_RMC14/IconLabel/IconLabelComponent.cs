// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.IconLabel.IconLabelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.IconLabel;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class IconLabelComponent : 
  Component,
  ISerializationGenerated<IconLabelComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? LabelTextLocId;
  [AutoNetworkedField]
  public List<(string, object)> LabelTextParams = new List<(string, object)>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TextSize = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string TextColor = "Black";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i StoredOffset = new Vector2i(0, 0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LabelMaxSize = 3;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IconLabelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IconLabelComponent) target1;
    if (serialization.TryCustomCopy<IconLabelComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? target2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.LabelTextLocId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId?>(this.LabelTextLocId, hookCtx, context);
    target.LabelTextLocId = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.TextSize, ref target3, hookCtx, false, context))
      target3 = this.TextSize;
    target.TextSize = target3;
    string target4 = (string) null;
    if (this.TextColor == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TextColor, ref target4, hookCtx, false, context))
      target4 = this.TextColor;
    target.TextColor = target4;
    Vector2i target5 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.StoredOffset, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2i>(this.StoredOffset, hookCtx, context);
    target.StoredOffset = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.LabelMaxSize, ref target6, hookCtx, false, context))
      target6 = this.LabelMaxSize;
    target.LabelMaxSize = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IconLabelComponent target,
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
    IconLabelComponent target1 = (IconLabelComponent) target;
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
    IconLabelComponent target1 = (IconLabelComponent) target;
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
    IconLabelComponent target1 = (IconLabelComponent) target;
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
  virtual IconLabelComponent Component.Instantiate() => new IconLabelComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IconLabelComponent_AutoState : IComponentState
  {
    public LocId? LabelTextLocId;
    public List<(string, object)> LabelTextParams;
    public int TextSize;
    public string TextColor;
    public Vector2i StoredOffset;
    public int LabelMaxSize;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IconLabelComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IconLabelComponent, ComponentGetState>(new ComponentEventRefHandler<IconLabelComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IconLabelComponent, ComponentHandleState>(new ComponentEventRefHandler<IconLabelComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IconLabelComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IconLabelComponent.IconLabelComponent_AutoState()
      {
        LabelTextLocId = component.LabelTextLocId,
        LabelTextParams = component.LabelTextParams,
        TextSize = component.TextSize,
        TextColor = component.TextColor,
        StoredOffset = component.StoredOffset,
        LabelMaxSize = component.LabelMaxSize
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IconLabelComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IconLabelComponent.IconLabelComponent_AutoState current))
        return;
      component.LabelTextLocId = current.LabelTextLocId;
      component.LabelTextParams = current.LabelTextParams == null ? (List<(string, object)>) null : new List<(string, object)>((IEnumerable<(string, object)>) current.LabelTextParams);
      component.TextSize = current.TextSize;
      component.TextColor = current.TextColor;
      component.StoredOffset = current.StoredOffset;
      component.LabelMaxSize = current.LabelMaxSize;
    }
  }
}
