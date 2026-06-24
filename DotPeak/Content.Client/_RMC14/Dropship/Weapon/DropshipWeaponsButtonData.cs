// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Weapon.DropshipWeaponsButtonData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship.Weapon;

public readonly record struct DropshipWeaponsButtonData(
  LocId Text,
  Action<BaseButton.ButtonEventArgs> OnPressed,
  NetEntity? Weapon = null)
;
