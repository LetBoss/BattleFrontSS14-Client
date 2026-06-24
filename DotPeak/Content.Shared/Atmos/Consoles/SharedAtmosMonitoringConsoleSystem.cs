// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Consoles.SharedAtmosMonitoringConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Consoles;

public abstract class SharedAtmosMonitoringConsoleSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosMonitoringConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<AtmosMonitoringConsoleComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
  }

  private void OnGetState(
    EntityUid uid,
    AtmosMonitoringConsoleComponent component,
    ref ComponentGetState args)
  {
    if (GameTick.op_LessThanOrEqual(((ComponentGetState) ref args).FromTick, component.CreationTick) || component.ForceFullUpdate)
    {
      component.ForceFullUpdate = false;
      Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> chunks = new Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>>(component.AtmosPipeChunks.Count);
      foreach ((Vector2i key, AtmosPipeChunk atmosPipeChunk) in component.AtmosPipeChunks)
        chunks.Add(key, atmosPipeChunk.AtmosPipeData);
      ((ComponentGetState) ref args).State = (IComponentState) new SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState(chunks, component.AtmosDevices);
    }
    else
    {
      Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> modifiedChunks = new Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>>();
      foreach ((Vector2i key, AtmosPipeChunk atmosPipeChunk) in component.AtmosPipeChunks)
      {
        if (!GameTick.op_LessThan(atmosPipeChunk.LastUpdate, ((ComponentGetState) ref args).FromTick))
          modifiedChunks.Add(key, atmosPipeChunk.AtmosPipeData);
      }
      ((ComponentGetState) ref args).State = (IComponentState) new SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleDeltaState(modifiedChunks, component.AtmosDevices, new HashSet<Vector2i>((IEnumerable<Vector2i>) component.AtmosPipeChunks.Keys));
    }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class AtmosMonitoringConsoleState : ComponentState
  {
    public Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> Chunks;
    public Dictionary<NetEntity, AtmosDeviceNavMapData> AtmosDevices;

    public AtmosMonitoringConsoleState(
      Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> chunks,
      Dictionary<NetEntity, AtmosDeviceNavMapData> atmosDevices)
    {
      this.Chunks = chunks;
      this.AtmosDevices = atmosDevices;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class AtmosMonitoringConsoleDeltaState : 
    ComponentState,
    IComponentDeltaState<SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> ModifiedChunks;
    public Dictionary<NetEntity, AtmosDeviceNavMapData> AtmosDevices;
    public HashSet<Vector2i> AllChunks;

    public AtmosMonitoringConsoleDeltaState(
      Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> modifiedChunks,
      Dictionary<NetEntity, AtmosDeviceNavMapData> atmosDevices,
      HashSet<Vector2i> allChunks)
    {
      this.ModifiedChunks = modifiedChunks;
      this.AtmosDevices = atmosDevices;
      this.AllChunks = allChunks;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public void ApplyToFullState(
      SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState state)
    {
      foreach (Vector2i key in state.Chunks.Keys)
      {
        if (!this.AllChunks.Contains(key))
          state.Chunks.Remove(key);
      }
      foreach ((Vector2i key, Dictionary<AtmosMonitoringConsoleSubnet, ulong> dictionary) in this.ModifiedChunks)
        state.Chunks[key] = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>((IDictionary<AtmosMonitoringConsoleSubnet, ulong>) dictionary);
      state.AtmosDevices.Clear();
      foreach ((NetEntity key, AtmosDeviceNavMapData deviceNavMapData) in this.AtmosDevices)
        state.AtmosDevices.Add(key, deviceNavMapData);
    }

    public SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState CreateNewFullState(
      SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState state)
    {
      Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> chunks = new Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>>(state.Chunks.Count);
      foreach ((Vector2i key, Dictionary<AtmosMonitoringConsoleSubnet, ulong> _) in state.Chunks)
      {
        if (this.AllChunks.Contains(key))
          chunks[key] = !this.ModifiedChunks.ContainsKey(key) ? new Dictionary<AtmosMonitoringConsoleSubnet, ulong>((IDictionary<AtmosMonitoringConsoleSubnet, ulong>) state.Chunks[key]) : new Dictionary<AtmosMonitoringConsoleSubnet, ulong>((IDictionary<AtmosMonitoringConsoleSubnet, ulong>) this.ModifiedChunks[key]);
      }
      return new SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState(chunks, new Dictionary<NetEntity, AtmosDeviceNavMapData>((IDictionary<NetEntity, AtmosDeviceNavMapData>) this.AtmosDevices));
    }
  }
}
