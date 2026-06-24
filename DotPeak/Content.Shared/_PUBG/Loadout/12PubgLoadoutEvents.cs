// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgLoadoutWeaponSlotState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[NetSerializable]
[Serializable]
public sealed class PubgLoadoutWeaponSlotState
{
  public string SlotId { get; }

  public PubgModuleSlotType SlotType { get; }

  public PubgModuleUiAnchor UiAnchor { get; }

  public string DisplayNameLocKey { get; }

  public List<string> AllowedModulePrototypeIds { get; }

  public NetEntity AttachedModule { get; }

  public string? AttachedModuleName { get; }

  public PubgLoadoutWeaponSlotState(
    string slotId,
    PubgModuleSlotType slotType,
    PubgModuleUiAnchor uiAnchor,
    string displayNameLocKey,
    List<string> allowedModulePrototypeIds,
    NetEntity attachedModule,
    string? attachedModuleName)
  {
    this.SlotId = slotId;
    this.SlotType = slotType;
    this.UiAnchor = uiAnchor;
    this.DisplayNameLocKey = displayNameLocKey;
    this.AllowedModulePrototypeIds = allowedModulePrototypeIds;
    this.AttachedModule = attachedModule;
    this.AttachedModuleName = attachedModuleName;
  }
}
