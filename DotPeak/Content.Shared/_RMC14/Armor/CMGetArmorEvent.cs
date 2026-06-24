// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.CMGetArmorEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared._RMC14.Armor;

[ByRefEvent]
public record struct CMGetArmorEvent(
  SlotFlags TargetSlots,
  int XenoArmor = 0,
  int Melee = 0,
  int Bullet = 0,
  int Bio = 0,
  int FrontalArmor = 0,
  int SideArmor = 0,
  double ArmorModifier = 1.0,
  int ExplosionArmor = 0) : IInventoryRelayEvent
;
