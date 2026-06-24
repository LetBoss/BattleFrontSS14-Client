// Decompiled with JetBrains decompiler
// Type: Content.Client.Configurable.UI.ConfigurationBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Configurable;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Client.Configurable.UI;

public sealed class ConfigurationBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ConfigurationMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<ConfigurationMenu>((BoundUserInterface) this);
    this._menu.OnConfiguration += new Action<Dictionary<string, string>>(this.SendConfiguration);
    ConfigurationComponent configurationComponent;
    if (!this.EntMan.TryGetComponent<ConfigurationComponent>(this.Owner, ref configurationComponent))
      return;
    this.Refresh(Entity<ConfigurationComponent>.op_Implicit((this.Owner, configurationComponent)));
  }

  public void Refresh(Entity<ConfigurationComponent> entity)
  {
    if (this._menu == null)
      return;
    ((Control) this._menu.Column).Children.Clear();
    this._menu.Inputs.Clear();
    foreach (KeyValuePair<string, string> keyValuePair in entity.Comp.Config)
    {
      Label label1 = new Label();
      ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 8f, 0.0f);
      ((Control) label1).Name = keyValuePair.Key;
      label1.Text = keyValuePair.Key + ":";
      ((Control) label1).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) label1).HorizontalExpand = true;
      ((Control) label1).SizeFlagsStretchRatio = 0.2f;
      ((Control) label1).MinSize = new Vector2(60f, 0.0f);
      Label label2 = label1;
      LineEdit lineEdit1 = new LineEdit();
      ((Control) lineEdit1).Name = keyValuePair.Key + "-input";
      lineEdit1.Text = keyValuePair.Value ?? "";
      lineEdit1.IsValid = new Func<string, bool>(this._menu.Validate);
      ((Control) lineEdit1).HorizontalExpand = true;
      ((Control) lineEdit1).SizeFlagsStretchRatio = 0.8f;
      LineEdit lineEdit2 = lineEdit1;
      this._menu.Inputs.Add((keyValuePair.Key, lineEdit2));
      BoxContainer to = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      ConfigurationMenu.CopyProperties<BoxContainer>(this._menu.Row, to);
      ((Control) to).AddChild((Control) label2);
      ((Control) to).AddChild((Control) lineEdit2);
      ((Control) this._menu.Column).AddChild((Control) to);
    }
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    base.ReceiveMessage(message);
    if (this._menu == null || !(message is ConfigurationComponent.ValidationUpdateMessage validationUpdateMessage))
      return;
    this._menu.Validation = new Regex(validationUpdateMessage.ValidationString, RegexOptions.Compiled);
  }

  public void SendConfiguration(Dictionary<string, string> config)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ConfigurationComponent.ConfigurationUpdatedMessage(config));
  }
}
