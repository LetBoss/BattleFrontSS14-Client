// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Areas.AreaGridComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Areas;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AreaSystem)})]
public sealed class AreaGridComponent : 
  Component,
  ISerializationGenerated<AreaGridComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Vector2i, EntProtoId<AreaComponent>> Areas = new Dictionary<Vector2i, EntProtoId<AreaComponent>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Vector2i, Color> Colors = new Dictionary<Vector2i, Color>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<AreaComponent>, EntityUid> AreaEntities = new Dictionary<EntProtoId<AreaComponent>, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Vector2i, string> Labels = new Dictionary<Vector2i, string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AreaGridComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AreaGridComponent) target1;
    if (serialization.TryCustomCopy<AreaGridComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Vector2i, EntProtoId<AreaComponent>> target2 = (Dictionary<Vector2i, EntProtoId<AreaComponent>>) null;
    if (this.Areas == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, EntProtoId<AreaComponent>>>(this.Areas, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Vector2i, EntProtoId<AreaComponent>>>(this.Areas, hookCtx, context);
    target.Areas = target2;
    Dictionary<Vector2i, Color> target3 = (Dictionary<Vector2i, Color>) null;
    if (this.Colors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, Color>>(this.Colors, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<Vector2i, Color>>(this.Colors, hookCtx, context);
    target.Colors = target3;
    Dictionary<EntProtoId<AreaComponent>, EntityUid> target4 = (Dictionary<EntProtoId<AreaComponent>, EntityUid>) null;
    if (this.AreaEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<AreaComponent>, EntityUid>>(this.AreaEntities, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntProtoId<AreaComponent>, EntityUid>>(this.AreaEntities, hookCtx, context);
    target.AreaEntities = target4;
    Dictionary<Vector2i, string> target5 = (Dictionary<Vector2i, string>) null;
    if (this.Labels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, string>>(this.Labels, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<Vector2i, string>>(this.Labels, hookCtx, context);
    target.Labels = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AreaGridComponent target,
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
    AreaGridComponent target1 = (AreaGridComponent) target;
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
    AreaGridComponent target1 = (AreaGridComponent) target;
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
    AreaGridComponent target1 = (AreaGridComponent) target;
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
  virtual AreaGridComponent Component.Instantiate() => new AreaGridComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AreaGridComponent_AutoState : IComponentState
  {
    public Dictionary<Vector2i, EntProtoId<AreaComponent>> Areas;
    public Dictionary<Vector2i, Color> Colors;
    public Dictionary<EntProtoId<AreaComponent>, NetEntity> AreaEntities;
    public Dictionary<Vector2i, string> Labels;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AreaGridComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AreaGridComponent, ComponentGetState>(new ComponentEventRefHandler<AreaGridComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AreaGridComponent, ComponentHandleState>(new ComponentEventRefHandler<AreaGridComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, AreaGridComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new AreaGridComponent.AreaGridComponent_AutoState()
      {
        Areas = component.Areas,
        Colors = component.Colors,
        AreaEntities = this.GetNetEntityDictionary<EntProtoId<AreaComponent>>(component.AreaEntities),
        Labels = component.Labels
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AreaGridComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AreaGridComponent.AreaGridComponent_AutoState current))
        return;
      component.Areas = current.Areas == null ? (Dictionary<Vector2i, EntProtoId<AreaComponent>>) null : new Dictionary<Vector2i, EntProtoId<AreaComponent>>((IDictionary<Vector2i, EntProtoId<AreaComponent>>) current.Areas);
      component.Colors = current.Colors == null ? (Dictionary<Vector2i, Color>) null : new Dictionary<Vector2i, Color>((IDictionary<Vector2i, Color>) current.Colors);
      this.EnsureEntityDictionary<AreaGridComponent, EntProtoId<AreaComponent>>(current.AreaEntities, uid, component.AreaEntities);
      component.Labels = current.Labels == null ? (Dictionary<Vector2i, string>) null : new Dictionary<Vector2i, string>((IDictionary<Vector2i, string>) current.Labels);
    }
  }
}
