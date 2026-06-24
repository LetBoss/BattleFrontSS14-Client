// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutSetSlotChoiceRequestEvent
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
public sealed class CivLoadoutSetSlotChoiceRequestEvent : EntityEventArgs
{
  public readonly string Faction;
  public readonly CivTdmClass Class;
  public readonly string Slot;
  public readonly string Proto;

  public CivLoadoutSetSlotChoiceRequestEvent(
    string faction,
    CivTdmClass cls,
    string slot,
    string proto)
  {
    this.Faction = faction;
    this.Class = cls;
    this.Slot = slot;
    this.Proto = proto;
  }
}
