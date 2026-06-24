// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Squads.CivSquadRadialMenuState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Squads;

public readonly record struct CivSquadRadialMenuState(
  int TeamId,
  string Title,
  string Description,
  IReadOnlyList<CivSquadRadialOption> Options)
;
