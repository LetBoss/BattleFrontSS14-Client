// Decompiled with JetBrains decompiler
// Type: Content.Client.Info.RulesAndInfoWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Info;

public sealed class RulesAndInfoWindow : DefaultWindow
{
  [Dependency]
  private IResourceManager _resourceManager;

  public RulesAndInfoWindow()
  {
    IoCManager.InjectDependencies<RulesAndInfoWindow>(this);
    this.Title = Loc.GetString("ui-info-title");
    TabContainer tabContainer = new TabContainer();
    RulesControl rulesControl1 = new RulesControl();
    ((Control) rulesControl1).Margin = new Thickness(10f);
    RulesControl rulesControl2 = rulesControl1;
    Content.Client.Info.Info info = new Content.Client.Info.Info();
    ((Control) info).Margin = new Thickness(10f);
    Content.Client.Info.Info tutorialList = info;
    ((Control) tabContainer).AddChild((Control) rulesControl2);
    ((Control) tabContainer).AddChild((Control) tutorialList);
    TabContainer.SetTabTitle((Control) rulesControl2, Loc.GetString("ui-info-tab-rules"));
    TabContainer.SetTabTitle((Control) tutorialList, Loc.GetString("ui-info-tab-tutorial"));
    this.PopulateTutorial(tutorialList);
    this.Contents.AddChild((Control) tabContainer);
    ((Control) this).SetSize = new Vector2(650f, 650f);
  }

  private void PopulateTutorial(Content.Client.Info.Info tutorialList)
  {
    InfoControlsSection infoControlsSection = new InfoControlsSection();
    ((Control) tutorialList.InfoContainer).AddChild((Control) infoControlsSection);
    this.AddSection(tutorialList, Loc.GetString("ui-info-header-gameplay"), "Gameplay.txt", true);
    this.AddSection(tutorialList, Loc.GetString("ui-info-header-sandbox"), "Sandbox.txt", true);
    ((BaseButton) infoControlsSection.ControlsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((Control) this).UserInterfaceManager.GetUIController<OptionsUIController>().OpenWindow());
  }

  private static void AddSection(Content.Client.Info.Info info, Control control)
  {
    ((Control) info.InfoContainer).AddChild(control);
  }

  private void AddSection(Content.Client.Info.Info info, string title, string path, bool markup = false)
  {
    RulesAndInfoWindow.AddSection(info, RulesAndInfoWindow.MakeSection(title, path, markup, this._resourceManager));
  }

  private static Control MakeSection(string title, string path, bool markup, IResourceManager res)
  {
    return (Control) new InfoSection(title, res.ContentFileReadAllText("/ServerInfo/" + path), markup);
  }
}
