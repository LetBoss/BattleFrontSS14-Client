// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cloning.CloningPodComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Content.Shared.Materials;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cloning;

[RegisterComponent]
public sealed class CloningPodComponent : 
  Component,
  ISerializationGenerated<CloningPodComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SinkPortPrototype> PodPort;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ContainerSlot BodyContainer;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float CloningProgress;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int UsedBiomass;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool FailedClone;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<MaterialPrototype> RequiredMaterial;
  [DataField(null, false, 1, false, false, null)]
  public float CloningTime;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MobSpawnId;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ScreamSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public CloningPodStatus Status;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? ConnectedConsole;

  public CloningPodComponent()
  {
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("ZombieScreams", new AudioParams?());
    ((SoundSpecifier) collectionSpecifier).Params = ((AudioParams) ref AudioParams.Default).WithVolume(4f);
    this.ScreamSound = (SoundSpecifier) collectionSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CloningPodComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CloningPodComponent) component;
    if (serialization.TryCustomCopy<CloningPodComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<SinkPortPrototype> protoId1 = new ProtoId<SinkPortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SinkPortPrototype>>(this.PodPort, ref protoId1, hookCtx, false, context))
      protoId1 = serialization.CreateCopy<ProtoId<SinkPortPrototype>>(this.PodPort, hookCtx, context, false);
    target.PodPort = protoId1;
    ProtoId<MaterialPrototype> protoId2 = new ProtoId<MaterialPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<MaterialPrototype>>(this.RequiredMaterial, ref protoId2, hookCtx, false, context))
      protoId2 = serialization.CreateCopy<ProtoId<MaterialPrototype>>(this.RequiredMaterial, hookCtx, context, false);
    target.RequiredMaterial = protoId2;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CloningTime, ref num, hookCtx, false, context))
      num = this.CloningTime;
    target.CloningTime = num;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MobSpawnId, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.MobSpawnId, hookCtx, context, false);
    target.MobSpawnId = entProtoId;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.ScreamSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScreamSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.ScreamSound, hookCtx, context, false);
    target.ScreamSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CloningPodComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CloningPodComponent target1 = (CloningPodComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CloningPodComponent target1 = (CloningPodComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CloningPodComponent target1 = (CloningPodComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CloningPodComponent Component.Instantiate() => new CloningPodComponent();
}
