// Decompiled with JetBrains decompiler
// Type: Content.Shared.NameModifier.EntitySystems.RefreshNameModifiersEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.NameModifier.EntitySystems;

[ByRefEvent]
public sealed class RefreshNameModifiersEvent : IInventoryRelayEvent
{
  public readonly string BaseName;
  private readonly List<(LocId LocId, int Priority, (string, object)[] ExtraArgs)> _modifiers = new List<(LocId, int, (string, object)[])>();

  public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;

  public int ModifierCount => this._modifiers.Count;

  public RefreshNameModifiersEvent(string baseName) => this.BaseName = baseName;

  public void AddModifier(LocId locId, int priority = 0, params (string, object)[] extraArgs)
  {
    this._modifiers.Add((locId, priority, extraArgs));
  }

  public string GetModifiedName()
  {
    string baseName = this.BaseName;
    foreach ((LocId, int, (string, object)[]) tuple in (IEnumerable<(LocId, int, (string, object)[])>) this._modifiers.OrderBy<(LocId, int, (string, object)[]), int>((Func<(LocId, int, (string, object)[]), int>) (n => n.Priority)))
    {
      (string, object)[] array = tuple.Item3;
      Array.Resize<(string, object)>(ref array, array.Length + 1);
      (string, object)[] valueTupleArray = array;
      valueTupleArray[valueTupleArray.Length - 1] = ("baseName", (object) baseName);
      baseName = Loc.GetString((string) tuple.Item1, array);
    }
    return baseName;
  }
}
