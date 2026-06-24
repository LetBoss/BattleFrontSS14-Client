// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Lobby.UI.PubgCharacterSetupGuiSavePanel
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._PUBG.Lobby.UI;

public sealed class PubgCharacterSetupGuiSavePanel : DefaultWindow
{
  public Button SaveButton { get; }

  public Button NoSaveButton { get; }

  public PubgCharacterSetupGuiSavePanel()
  {
    this.Title = Loc.GetString("character-setup-gui-save-panel-title");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Margin = new Thickness(10f);
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = Loc.GetString("character-setup-gui-save-panel-text")
    });
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
    BoxContainer boxContainer4 = boxContainer3;
    this.SaveButton = new Button()
    {
      Text = Loc.GetString("character-setup-gui-save-panel-save-button")
    };
    Button button = new Button();
    button.Text = Loc.GetString("character-setup-gui-save-panel-nosave-button");
    ((Control) button).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    this.NoSaveButton = button;
    ((Control) boxContainer4).AddChild((Control) this.SaveButton);
    ((Control) boxContainer4).AddChild((Control) this.NoSaveButton);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    this.Contents.AddChild((Control) boxContainer2);
  }
}
