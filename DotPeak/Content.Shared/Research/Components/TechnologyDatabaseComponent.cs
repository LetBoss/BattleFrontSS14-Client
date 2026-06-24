// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Components.TechnologyDatabaseComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Lathe;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Research.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedResearchSystem), typeof (SharedLatheSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class TechnologyDatabaseComponent : 
  Component,
  ISerializationGenerated<TechnologyDatabaseComponent>,
  ISerializationGenerated
{
  [AutoNetworkedField]
  [DataField("mainDiscipline", false, 1, false, false, typeof (PrototypeIdSerializer<TechDisciplinePrototype>))]
  public string? MainDiscipline;
  [AutoNetworkedField]
  [DataField("currentTechnologyCards", false, 1, false, false, null)]
  public System.Collections.Generic.List<string> CurrentTechnologyCards = new System.Collections.Generic.List<string>();
  [AutoNetworkedField]
  [DataField("supportedDisciplines", false, 1, false, false, typeof (PrototypeIdListSerializer<TechDisciplinePrototype>))]
  public System.Collections.Generic.List<string> SupportedDisciplines = new System.Collections.Generic.List<string>();
  [AutoNetworkedField]
  [DataField("unlockedTechnologies", false, 1, false, false, typeof (PrototypeIdListSerializer<TechnologyPrototype>))]
  public System.Collections.Generic.List<string> UnlockedTechnologies = new System.Collections.Generic.List<string>();
  [AutoNetworkedField]
  [DataField("unlockedRecipes", false, 1, false, false, typeof (PrototypeIdListSerializer<LatheRecipePrototype>))]
  public System.Collections.Generic.List<string> UnlockedRecipes = new System.Collections.Generic.List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TechnologyDatabaseComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TechnologyDatabaseComponent) target1;
    if (serialization.TryCustomCopy<TechnologyDatabaseComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MainDiscipline, ref target2, hookCtx, false, context))
      target2 = this.MainDiscipline;
    target.MainDiscipline = target2;
    System.Collections.Generic.List<string> target3 = (System.Collections.Generic.List<string>) null;
    if (this.CurrentTechnologyCards == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.CurrentTechnologyCards, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.CurrentTechnologyCards, hookCtx, context);
    target.CurrentTechnologyCards = target3;
    System.Collections.Generic.List<string> target4 = (System.Collections.Generic.List<string>) null;
    if (this.SupportedDisciplines == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.SupportedDisciplines, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.SupportedDisciplines, hookCtx, context);
    target.SupportedDisciplines = target4;
    System.Collections.Generic.List<string> target5 = (System.Collections.Generic.List<string>) null;
    if (this.UnlockedTechnologies == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.UnlockedTechnologies, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.UnlockedTechnologies, hookCtx, context);
    target.UnlockedTechnologies = target5;
    System.Collections.Generic.List<string> target6 = (System.Collections.Generic.List<string>) null;
    if (this.UnlockedRecipes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.UnlockedRecipes, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.UnlockedRecipes, hookCtx, context);
    target.UnlockedRecipes = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TechnologyDatabaseComponent target,
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
    TechnologyDatabaseComponent target1 = (TechnologyDatabaseComponent) target;
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
    TechnologyDatabaseComponent target1 = (TechnologyDatabaseComponent) target;
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
    TechnologyDatabaseComponent target1 = (TechnologyDatabaseComponent) target;
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
  virtual TechnologyDatabaseComponent Component.Instantiate() => new TechnologyDatabaseComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TechnologyDatabaseComponent_AutoState : IComponentState
  {
    public string? MainDiscipline;
    public System.Collections.Generic.List<string> CurrentTechnologyCards;
    public System.Collections.Generic.List<string> SupportedDisciplines;
    public System.Collections.Generic.List<string> UnlockedTechnologies;
    public System.Collections.Generic.List<string> UnlockedRecipes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TechnologyDatabaseComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TechnologyDatabaseComponent, ComponentGetState>(new ComponentEventRefHandler<TechnologyDatabaseComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TechnologyDatabaseComponent, ComponentHandleState>(new ComponentEventRefHandler<TechnologyDatabaseComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TechnologyDatabaseComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TechnologyDatabaseComponent.TechnologyDatabaseComponent_AutoState()
      {
        MainDiscipline = component.MainDiscipline,
        CurrentTechnologyCards = component.CurrentTechnologyCards,
        SupportedDisciplines = component.SupportedDisciplines,
        UnlockedTechnologies = component.UnlockedTechnologies,
        UnlockedRecipes = component.UnlockedRecipes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TechnologyDatabaseComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TechnologyDatabaseComponent.TechnologyDatabaseComponent_AutoState current))
        return;
      component.MainDiscipline = current.MainDiscipline;
      component.CurrentTechnologyCards = current.CurrentTechnologyCards == null ? (System.Collections.Generic.List<string>) null : new System.Collections.Generic.List<string>((IEnumerable<string>) current.CurrentTechnologyCards);
      component.SupportedDisciplines = current.SupportedDisciplines == null ? (System.Collections.Generic.List<string>) null : new System.Collections.Generic.List<string>((IEnumerable<string>) current.SupportedDisciplines);
      component.UnlockedTechnologies = current.UnlockedTechnologies == null ? (System.Collections.Generic.List<string>) null : new System.Collections.Generic.List<string>((IEnumerable<string>) current.UnlockedTechnologies);
      component.UnlockedRecipes = current.UnlockedRecipes == null ? (System.Collections.Generic.List<string>) null : new System.Collections.Generic.List<string>((IEnumerable<string>) current.UnlockedRecipes);
    }
  }
}
