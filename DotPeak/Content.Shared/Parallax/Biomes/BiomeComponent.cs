// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.BiomeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Parallax.Biomes.Layers;
using Content.Shared.Parallax.Biomes.Markers;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Parallax.Biomes;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedBiomeSystem)})]
public sealed class BiomeComponent : 
  Component,
  ISerializationGenerated<BiomeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public bool Enabled = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("seed", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Seed = -1;
  [DataField("layers", false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<IBiomeLayer> Layers = new List<IBiomeLayer>();
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<BiomeTemplatePrototype>? Template;
  [DataField("modifiedTiles", false, 1, false, false, null)]
  public System.Collections.Generic.Dictionary<Vector2i, HashSet<Vector2i>> ModifiedTiles = new System.Collections.Generic.Dictionary<Vector2i, HashSet<Vector2i>>();
  [DataField("decals", false, 1, false, false, null)]
  public System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<uint, Vector2i>> LoadedDecals = new System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<uint, Vector2i>>();
  [DataField("entities", false, 1, false, false, null)]
  public System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<EntityUid, Vector2i>> LoadedEntities = new System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<EntityUid, Vector2i>>();
  [DataField("loadedChunks", false, 1, false, false, null)]
  public HashSet<Vector2i> LoadedChunks = new HashSet<Vector2i>();
  [DataField("pendingMarkers", false, 1, false, false, null)]
  public System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<string, List<Vector2i>>> PendingMarkers = new System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<string, List<Vector2i>>>();
  [DataField("loadedMarkers", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<HashSet<Vector2i>, BiomeMarkerLayerPrototype>))]
  public System.Collections.Generic.Dictionary<string, HashSet<Vector2i>> LoadedMarkers = new System.Collections.Generic.Dictionary<string, HashSet<Vector2i>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<BiomeMarkerLayerPrototype>> MarkerLayers = new HashSet<ProtoId<BiomeMarkerLayerPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<BiomeMarkerLayerPrototype>> ForcedMarkerLayers = new HashSet<ProtoId<BiomeMarkerLayerPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BiomeComponent) target1;
    if (serialization.TryCustomCopy<BiomeComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Seed, ref target3, hookCtx, false, context))
      target3 = this.Seed;
    target.Seed = target3;
    List<IBiomeLayer> target4 = (List<IBiomeLayer>) null;
    if (this.Layers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<IBiomeLayer>>(this.Layers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<IBiomeLayer>>(this.Layers, hookCtx, context);
    target.Layers = target4;
    ProtoId<BiomeTemplatePrototype>? target5 = new ProtoId<BiomeTemplatePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<BiomeTemplatePrototype>?>(this.Template, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<BiomeTemplatePrototype>?>(this.Template, hookCtx, context);
    target.Template = target5;
    System.Collections.Generic.Dictionary<Vector2i, HashSet<Vector2i>> target6 = (System.Collections.Generic.Dictionary<Vector2i, HashSet<Vector2i>>) null;
    if (this.ModifiedTiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<Vector2i, HashSet<Vector2i>>>(this.ModifiedTiles, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<System.Collections.Generic.Dictionary<Vector2i, HashSet<Vector2i>>>(this.ModifiedTiles, hookCtx, context);
    target.ModifiedTiles = target6;
    System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<uint, Vector2i>> target7 = (System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<uint, Vector2i>>) null;
    if (this.LoadedDecals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<uint, Vector2i>>>(this.LoadedDecals, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<uint, Vector2i>>>(this.LoadedDecals, hookCtx, context);
    target.LoadedDecals = target7;
    System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<EntityUid, Vector2i>> target8 = (System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<EntityUid, Vector2i>>) null;
    if (this.LoadedEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<EntityUid, Vector2i>>>(this.LoadedEntities, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<EntityUid, Vector2i>>>(this.LoadedEntities, hookCtx, context);
    target.LoadedEntities = target8;
    HashSet<Vector2i> target9 = (HashSet<Vector2i>) null;
    if (this.LoadedChunks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<Vector2i>>(this.LoadedChunks, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<HashSet<Vector2i>>(this.LoadedChunks, hookCtx, context);
    target.LoadedChunks = target9;
    System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<string, List<Vector2i>>> target10 = (System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<string, List<Vector2i>>>) null;
    if (this.PendingMarkers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<string, List<Vector2i>>>>(this.PendingMarkers, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<System.Collections.Generic.Dictionary<Vector2i, System.Collections.Generic.Dictionary<string, List<Vector2i>>>>(this.PendingMarkers, hookCtx, context);
    target.PendingMarkers = target10;
    System.Collections.Generic.Dictionary<string, HashSet<Vector2i>> target11 = (System.Collections.Generic.Dictionary<string, HashSet<Vector2i>>) null;
    if (this.LoadedMarkers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, HashSet<Vector2i>>>(this.LoadedMarkers, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, HashSet<Vector2i>>>(this.LoadedMarkers, hookCtx, context);
    target.LoadedMarkers = target11;
    HashSet<ProtoId<BiomeMarkerLayerPrototype>> target12 = (HashSet<ProtoId<BiomeMarkerLayerPrototype>>) null;
    if (this.MarkerLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<BiomeMarkerLayerPrototype>>>(this.MarkerLayers, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<HashSet<ProtoId<BiomeMarkerLayerPrototype>>>(this.MarkerLayers, hookCtx, context);
    target.MarkerLayers = target12;
    HashSet<ProtoId<BiomeMarkerLayerPrototype>> target13 = (HashSet<ProtoId<BiomeMarkerLayerPrototype>>) null;
    if (this.ForcedMarkerLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<BiomeMarkerLayerPrototype>>>(this.ForcedMarkerLayers, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<HashSet<ProtoId<BiomeMarkerLayerPrototype>>>(this.ForcedMarkerLayers, hookCtx, context);
    target.ForcedMarkerLayers = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeComponent target,
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
    BiomeComponent target1 = (BiomeComponent) target;
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
    BiomeComponent target1 = (BiomeComponent) target;
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
    BiomeComponent target1 = (BiomeComponent) target;
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
  virtual BiomeComponent Component.Instantiate() => new BiomeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BiomeComponent_AutoState : IComponentState
  {
    public int Seed;
    public List<IBiomeLayer> Layers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BiomeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BiomeComponent, ComponentGetState>(new ComponentEventRefHandler<BiomeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BiomeComponent, ComponentHandleState>(new ComponentEventRefHandler<BiomeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, BiomeComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new BiomeComponent.BiomeComponent_AutoState()
      {
        Seed = component.Seed,
        Layers = component.Layers
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BiomeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BiomeComponent.BiomeComponent_AutoState current))
        return;
      component.Seed = current.Seed;
      component.Layers = current.Layers == null ? (List<IBiomeLayer>) null : new List<IBiomeLayer>((IEnumerable<IBiomeLayer>) current.Layers);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, BiomeComponent>(uid, component, ref args1);
    }
  }
}
