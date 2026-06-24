// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Components.MapComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.Map.Components;

[NetSerializable]
[Serializable]
public sealed class MapComponentState(MapId mapId, bool lightingEnabled, bool paused, bool init) : 
  ComponentState
{
  public MapId MapId = mapId;
  public bool LightingEnabled = lightingEnabled;
  public bool MapPaused = paused;
  public bool Initialized = init;
}
