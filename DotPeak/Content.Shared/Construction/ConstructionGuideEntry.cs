// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.ConstructionGuideEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Construction;

[NetSerializable]
[Serializable]
public sealed class ConstructionGuideEntry
{
  public int? EntryNumber { get; set; }

  public int Padding { get; set; }

  public string Localization { get; set; } = string.Empty;

  public (string, object)[]? Arguments { get; set; }

  public SpriteSpecifier? Icon { get; set; }
}
