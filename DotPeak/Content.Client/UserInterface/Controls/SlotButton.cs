// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.SlotButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Inventory;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class SlotButton : SlotControl
{
  public SlotButton()
  {
  }

  public SlotButton(ClientInventorySystem.SlotData slotData)
  {
    this.ButtonTexturePath = slotData.TextureName;
    this.FullButtonTexturePath = slotData.FullTextureName;
    this.Blocked = slotData.Blocked;
    this.Highlight = slotData.Highlighted;
    this.StorageTexturePath = "Slots/back";
    this.SlotName = slotData.SlotName;
  }
}
