// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Consoles.AtmosMonitoringConsoleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Consoles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Atmos.Consoles;

public sealed class AtmosMonitoringConsoleSystem : SharedAtmosMonitoringConsoleSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AtmosMonitoringConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<AtmosMonitoringConsoleComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    AtmosMonitoringConsoleComponent component,
    ref ComponentHandleState args)
  {
    Dictionary<Vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong>> dictionary1;
    Dictionary<NetEntity, AtmosDeviceNavMapData> atmosDevices;
    switch (((ComponentHandleState) ref args).Current)
    {
      case SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleDeltaState consoleDeltaState:
        dictionary1 = consoleDeltaState.ModifiedChunks;
        atmosDevices = consoleDeltaState.AtmosDevices;
        using (Dictionary<Vector2i, AtmosPipeChunk>.KeyCollection.Enumerator enumerator = component.AtmosPipeChunks.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!consoleDeltaState.AllChunks.Contains(current))
              component.AtmosPipeChunks.Remove(current);
          }
          break;
        }
      case SharedAtmosMonitoringConsoleSystem.AtmosMonitoringConsoleState monitoringConsoleState:
        dictionary1 = monitoringConsoleState.Chunks;
        atmosDevices = monitoringConsoleState.AtmosDevices;
        using (Dictionary<Vector2i, AtmosPipeChunk>.KeyCollection.Enumerator enumerator = component.AtmosPipeChunks.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!monitoringConsoleState.Chunks.ContainsKey(current))
              component.AtmosPipeChunks.Remove(current);
          }
          break;
        }
      default:
        return;
    }
    foreach ((Vector2i vector2i, Dictionary<AtmosMonitoringConsoleSubnet, ulong> dictionary2) in dictionary1)
      component.AtmosPipeChunks[vector2i] = new AtmosPipeChunk(vector2i)
      {
        AtmosPipeData = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>((IDictionary<AtmosMonitoringConsoleSubnet, ulong>) dictionary2)
      };
    component.AtmosDevices.Clear();
    foreach ((NetEntity key, AtmosDeviceNavMapData deviceNavMapData) in atmosDevices)
      component.AtmosDevices[key] = deviceNavMapData;
  }
}
