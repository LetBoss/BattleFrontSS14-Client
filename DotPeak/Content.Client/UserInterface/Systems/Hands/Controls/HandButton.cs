// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Hands.Controls.HandButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared.Hands.Components;

#nullable enable
namespace Content.Client.UserInterface.Systems.Hands.Controls;

public sealed class HandButton : SlotControl
{
  public HandLocation HandLocation { get; }

  public HandButton(string handName, HandLocation handLocation)
  {
    this.HandLocation = handLocation;
    this.Name = "hand_" + handName;
    this.SlotName = handName;
    this.SetBackground(handLocation);
  }

  private void SetBackground(HandLocation handLoc)
  {
    string str;
    switch (handLoc)
    {
      case HandLocation.Left:
        str = "Slots/hand_l";
        break;
      case HandLocation.Middle:
        str = "Slots/hand_m";
        break;
      case HandLocation.Right:
        str = "Slots/hand_r";
        break;
      default:
        str = this.ButtonTexturePath;
        break;
    }
    this.ButtonTexturePath = str;
  }
}
