// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.BUIStates.DockingPortState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Shuttles.BUIStates;

[NetSerializable]
[Serializable]
public sealed class DockingPortState
{
  public string Name = string.Empty;
  public NetCoordinates Coordinates;
  public Angle Angle;
  public NetEntity Entity;
  public NetEntity? GridDockedWith;

  public bool Connected => this.GridDockedWith.HasValue;
}
