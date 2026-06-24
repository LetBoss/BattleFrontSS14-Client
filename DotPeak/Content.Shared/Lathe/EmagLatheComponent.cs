// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lathe.EmagLatheRecipesComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Lathe.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.Lathe;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EmagLatheRecipesComponent : 
  Component,
  ISerializationGenerated<EmagLatheRecipesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<LatheRecipePackPrototype>> EmagDynamicPacks = new List<ProtoId<LatheRecipePackPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<LatheRecipePackPrototype>> EmagStaticPacks = new List<ProtoId<LatheRecipePackPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmagLatheRecipesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmagLatheRecipesComponent) target1;
    if (serialization.TryCustomCopy<EmagLatheRecipesComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<LatheRecipePackPrototype>> target2 = (List<ProtoId<LatheRecipePackPrototype>>) null;
    if (this.EmagDynamicPacks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.EmagDynamicPacks, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.EmagDynamicPacks, hookCtx, context);
    target.EmagDynamicPacks = target2;
    List<ProtoId<LatheRecipePackPrototype>> target3 = (List<ProtoId<LatheRecipePackPrototype>>) null;
    if (this.EmagStaticPacks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.EmagStaticPacks, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.EmagStaticPacks, hookCtx, context);
    target.EmagStaticPacks = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmagLatheRecipesComponent target,
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
    EmagLatheRecipesComponent target1 = (EmagLatheRecipesComponent) target;
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
    EmagLatheRecipesComponent target1 = (EmagLatheRecipesComponent) target;
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
    EmagLatheRecipesComponent target1 = (EmagLatheRecipesComponent) target;
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
  virtual EmagLatheRecipesComponent Component.Instantiate() => new EmagLatheRecipesComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EmagLatheRecipesComponent_AutoState : IComponentState
  {
    public List<ProtoId<LatheRecipePackPrototype>> EmagDynamicPacks;
    public List<ProtoId<LatheRecipePackPrototype>> EmagStaticPacks;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EmagLatheRecipesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EmagLatheRecipesComponent, ComponentGetState>(new ComponentEventRefHandler<EmagLatheRecipesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EmagLatheRecipesComponent, ComponentHandleState>(new ComponentEventRefHandler<EmagLatheRecipesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EmagLatheRecipesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EmagLatheRecipesComponent.EmagLatheRecipesComponent_AutoState()
      {
        EmagDynamicPacks = component.EmagDynamicPacks,
        EmagStaticPacks = component.EmagStaticPacks
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EmagLatheRecipesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EmagLatheRecipesComponent.EmagLatheRecipesComponent_AutoState current))
        return;
      component.EmagDynamicPacks = current.EmagDynamicPacks == null ? (List<ProtoId<LatheRecipePackPrototype>>) null : new List<ProtoId<LatheRecipePackPrototype>>((IEnumerable<ProtoId<LatheRecipePackPrototype>>) current.EmagDynamicPacks);
      component.EmagStaticPacks = current.EmagStaticPacks == null ? (List<ProtoId<LatheRecipePackPrototype>>) null : new List<ProtoId<LatheRecipePackPrototype>>((IEnumerable<ProtoId<LatheRecipePackPrototype>>) current.EmagStaticPacks);
    }
  }
}
