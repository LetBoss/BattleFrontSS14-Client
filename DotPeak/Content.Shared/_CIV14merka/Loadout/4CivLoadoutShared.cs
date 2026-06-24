// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutSetItemRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Loadout;

[NetSerializable]
[Serializable]
public sealed class CivLoadoutSetItemRequestEvent : EntityEventArgs
{
  public readonly string Faction;
  public readonly CivTdmClass Class;
  public readonly string ItemKey;
  public readonly bool Disabled;

  public CivLoadoutSetItemRequestEvent(
    string faction,
    CivTdmClass cls,
    string itemKey,
    bool disabled)
  {
    this.Faction = faction;
    this.Class = cls;
    this.ItemKey = itemKey;
    this.Disabled = disabled;
  }
}
