// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.IFF.GetIFFFactionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

[ByRefEvent]
public record struct GetIFFFactionEvent(
  SlotFlags TargetSlots,
  HashSet<EntProtoId<IFFFactionComponent>> Factions) : IInventoryRelayEvent
;
