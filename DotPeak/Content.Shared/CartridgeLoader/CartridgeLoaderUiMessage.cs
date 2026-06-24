// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.CartridgeLoaderUiMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.CartridgeLoader;

[NetSerializable]
[Serializable]
public sealed class CartridgeLoaderUiMessage : BoundUserInterfaceMessage
{
  public readonly NetEntity CartridgeUid;
  public readonly CartridgeUiMessageAction Action;

  public CartridgeLoaderUiMessage(NetEntity cartridgeUid, CartridgeUiMessageAction action)
  {
    this.CartridgeUid = cartridgeUid;
    this.Action = action;
  }
}
