// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Squads.CivSquadRadialOption
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._CIV14merka.Squads;

public sealed record CivSquadRadialOption(
  CivSquadRadialAction Action,
  string Title,
  string Description,
  string Tooltip,
  Color AccentColor)
;
