// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.UI.MapObjects.GridMapObject
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Shuttles.UI.MapObjects;

public record struct GridMapObject : IMapObject
{
  public EntityUid Entity;

  public string Name { get; set; }

  public bool HideButton { get; init; }
}
