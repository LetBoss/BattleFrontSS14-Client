// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedGasTileOverlaySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasTileOverlaySystem : EntitySystem
{
  public const byte ChunkSize = 8;
  protected float AccumulatedFrameTime;
  protected bool PvsEnabled;
  [Dependency]
  protected IPrototypeManager ProtoMan;
  public int[] VisibleGasId;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTileOverlayComponent, ComponentGetState>(new ComponentEventRefHandler<GasTileOverlayComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
    List<int> intList = new List<int>();
    for (int index = 0; index < 9; ++index)
    {
      GasPrototype gasPrototype = this.ProtoMan.Index<GasPrototype>(index.ToString());
      if (!string.IsNullOrEmpty(gasPrototype.GasOverlayTexture) || !string.IsNullOrEmpty(gasPrototype.GasOverlaySprite) && !string.IsNullOrEmpty(gasPrototype.GasOverlayState))
        intList.Add(index);
    }
    this.VisibleGasId = intList.ToArray();
  }

  private void OnGetState(
    EntityUid uid,
    GasTileOverlayComponent component,
    ref ComponentGetState args)
  {
    if (this.PvsEnabled && !((ComponentGetState) ref args).ReplayState)
      return;
    if (GameTick.op_LessThanOrEqual(((ComponentGetState) ref args).FromTick, component.CreationTick) || GameTick.op_LessThanOrEqual(((ComponentGetState) ref args).FromTick, component.ForceTick))
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasTileOverlayState(component.Chunks);
    }
    else
    {
      Dictionary<Vector2i, GasOverlayChunk> modifiedChunks = new Dictionary<Vector2i, GasOverlayChunk>();
      foreach ((Vector2i key, GasOverlayChunk gasOverlayChunk) in component.Chunks)
      {
        if (GameTick.op_GreaterThanOrEqual(gasOverlayChunk.LastUpdate, ((ComponentGetState) ref args).FromTick))
          modifiedChunks[key] = gasOverlayChunk;
      }
      ((ComponentGetState) ref args).State = (IComponentState) new GasTileOverlayDeltaState(modifiedChunks, new HashSet<Vector2i>((IEnumerable<Vector2i>) component.Chunks.Keys));
    }
  }

  public static Vector2i GetGasChunkIndices(Vector2i indices)
  {
    return new Vector2i((int) MathF.Floor((float) indices.X / 8f), (int) MathF.Floor((float) indices.Y / 8f));
  }

  [NetSerializable]
  [Serializable]
  public readonly struct GasOverlayData(byte fireState, byte[] opacity) : 
    IEquatable<SharedGasTileOverlaySystem.GasOverlayData>
  {
    [Robust.Shared.ViewVariables.ViewVariables]
    public readonly byte FireState = fireState;
    [Robust.Shared.ViewVariables.ViewVariables]
    public readonly byte[] Opacity = opacity;

    public bool Equals(SharedGasTileOverlaySystem.GasOverlayData other)
    {
      if ((int) this.FireState != (int) other.FireState)
        return false;
      int? length1 = this.Opacity?.Length;
      int? length2 = other.Opacity?.Length;
      if (!(length1.GetValueOrDefault() == length2.GetValueOrDefault() & length1.HasValue == length2.HasValue))
        return false;
      if (this.Opacity != null && other.Opacity != null)
      {
        for (int index = 0; index < this.Opacity.Length; ++index)
        {
          if ((int) this.Opacity[index] != (int) other.Opacity[index])
            return false;
        }
      }
      return true;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class GasOverlayUpdateEvent : EntityEventArgs
  {
    public Dictionary<NetEntity, List<GasOverlayChunk>> UpdatedChunks = new Dictionary<NetEntity, List<GasOverlayChunk>>();
    public Dictionary<NetEntity, HashSet<Vector2i>> RemovedChunks = new Dictionary<NetEntity, HashSet<Vector2i>>();
  }
}
