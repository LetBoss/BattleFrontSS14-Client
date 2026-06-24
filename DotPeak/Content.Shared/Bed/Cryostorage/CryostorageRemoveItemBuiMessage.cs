// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Cryostorage.CryostorageRemoveItemBuiMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Bed.Cryostorage;

[NetSerializable]
[Serializable]
public sealed class CryostorageRemoveItemBuiMessage : BoundUserInterfaceMessage
{
  public NetEntity StoredEntity;
  public string Key;
  public CryostorageRemoveItemBuiMessage.RemovalType Type;

  public CryostorageRemoveItemBuiMessage(
    NetEntity storedEntity,
    string key,
    CryostorageRemoveItemBuiMessage.RemovalType type)
  {
    this.StoredEntity = storedEntity;
    this.Key = key;
    this.Type = type;
  }

  public enum RemovalType : byte
  {
    Hand,
    Inventory,
  }
}
