// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Ghost.PubgGhostFollowTeammateOptionState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Ghost;

[NetSerializable]
[Serializable]
public sealed class PubgGhostFollowTeammateOptionState
{
  public NetEntity Entity { get; }

  public string Name { get; }

  public int SlotIndex { get; }

  public PubgGhostFollowTeammateOptionState(NetEntity entity, string name, int slotIndex)
  {
    this.Entity = entity;
    this.Name = name;
    this.SlotIndex = slotIndex;
  }
}
