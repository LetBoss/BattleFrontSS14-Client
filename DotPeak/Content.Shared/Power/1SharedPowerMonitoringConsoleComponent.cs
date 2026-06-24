// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.PowerMonitoringDeviceMetaData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Power;

[NetSerializable]
[Serializable]
public struct PowerMonitoringDeviceMetaData(
  string name,
  NetCoordinates coordinates,
  PowerMonitoringConsoleGroup group,
  string spritePath,
  string spriteState)
{
  public string EntityName = name;
  public NetCoordinates Coordinates = coordinates;
  public PowerMonitoringConsoleGroup Group = group;
  public string SpritePath = spritePath;
  public string SpriteState = spriteState;
  public NetEntity? CollectionMaster = new NetEntity?();
}
