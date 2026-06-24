// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.EjectMaterialMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Materials;

[NetSerializable]
[Serializable]
public sealed class EjectMaterialMessage : EntityEventArgs
{
  public NetEntity Entity;
  public string Material;
  public int SheetsToExtract;

  public EjectMaterialMessage(NetEntity entity, string material, int sheetsToExtract)
  {
    this.Entity = entity;
    this.Material = material;
    this.SheetsToExtract = sheetsToExtract;
  }
}
