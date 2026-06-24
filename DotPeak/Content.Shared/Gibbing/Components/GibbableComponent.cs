// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gibbing.Components.GibbableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Gibbing.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.Gibbing.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (GibbingSystem)})]
public sealed class GibbableComponent : 
  Component,
  ISerializationGenerated<GibbableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> GibPrototypes = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int GibCount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? GibSound = (SoundSpecifier) new SoundCollectionSpecifier("gib", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.025f))));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float GibScatterRange = 0.3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GibbableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GibbableComponent) target1;
    if (serialization.TryCustomCopy<GibbableComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntProtoId> target2 = (List<EntProtoId>) null;
    if (this.GibPrototypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.GibPrototypes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntProtoId>>(this.GibPrototypes, hookCtx, context);
    target.GibPrototypes = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.GibCount, ref target3, hookCtx, false, context))
      target3 = this.GibCount;
    target.GibCount = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GibSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.GibSound, hookCtx, context);
    target.GibSound = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GibScatterRange, ref target5, hookCtx, false, context))
      target5 = this.GibScatterRange;
    target.GibScatterRange = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GibbableComponent target,
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
    GibbableComponent target1 = (GibbableComponent) target;
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
    GibbableComponent target1 = (GibbableComponent) target;
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
    GibbableComponent target1 = (GibbableComponent) target;
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
  virtual GibbableComponent Component.Instantiate() => new GibbableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GibbableComponent_AutoState : IComponentState
  {
    public List<EntProtoId> GibPrototypes;
    public int GibCount;
    public SoundSpecifier? GibSound;
    public float GibScatterRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GibbableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GibbableComponent, ComponentGetState>(new ComponentEventRefHandler<GibbableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GibbableComponent, ComponentHandleState>(new ComponentEventRefHandler<GibbableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, GibbableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new GibbableComponent.GibbableComponent_AutoState()
      {
        GibPrototypes = component.GibPrototypes,
        GibCount = component.GibCount,
        GibSound = component.GibSound,
        GibScatterRange = component.GibScatterRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GibbableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GibbableComponent.GibbableComponent_AutoState current))
        return;
      component.GibPrototypes = current.GibPrototypes == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.GibPrototypes);
      component.GibCount = current.GibCount;
      component.GibSound = current.GibSound;
      component.GibScatterRange = current.GibScatterRange;
    }
  }
}
