// Decompiled with JetBrains decompiler
// Type: Content.Client.Configurable.UI.ConfigurationMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Client.Configurable.UI;

public sealed class ConfigurationMenu : DefaultWindow
{
  public readonly BoxContainer Column;
  public readonly BoxContainer Row;
  public readonly List<(string name, LineEdit input)> Inputs;

  [Robust.Shared.ViewVariables.ViewVariables]
  public Regex? Validation { get; internal set; }

  public event Action<Dictionary<string, string>>? OnConfiguration;

  public ConfigurationMenu()
  {
    Vector2 vector2 = new Vector2(300f, 250f);
    ((Control) this).SetSize = vector2;
    ((Control) this).MinSize = vector2;
    this.Inputs = new List<(string, LineEdit)>();
    this.Title = Loc.GetString("configuration-menu-device-title");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).Margin = new Thickness(8f);
    boxContainer3.SeparationOverride = new int?(16 /*0x10*/);
    this.Column = boxContainer3;
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer4.SeparationOverride = new int?(16 /*0x10*/);
    ((Control) boxContainer4).HorizontalExpand = true;
    this.Row = boxContainer4;
    Button button1 = new Button();
    button1.Text = Loc.GetString("configuration-menu-confirm");
    ((Control) button1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) button1).VerticalAlignment = (Control.VAlignment) 2;
    Button button2 = button1;
    ((BaseButton) button2).OnButtonUp += new Action<BaseButton.ButtonEventArgs>(this.OnConfirm);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).ModulateSelfOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#202025", new Color?()));
    ScrollContainer scrollContainer2 = scrollContainer1;
    ((Control) scrollContainer2).AddChild((Control) this.Column);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    ((Control) boxContainer2).AddChild((Control) button2);
    this.Contents.AddChild((Control) boxContainer2);
  }

  private void OnConfirm(BaseButton.ButtonEventArgs args)
  {
    Dictionary<string, string> dictionary = this.GenerateDictionary((IEnumerable<(string, LineEdit)>) this.Inputs, "Text");
    Action<Dictionary<string, string>> onConfiguration = this.OnConfiguration;
    if (onConfiguration != null)
      onConfiguration(dictionary);
    ((BaseWindow) this).Close();
  }

  public bool Validate(string value)
  {
    Regex validation = this.Validation;
    return validation == null || validation.IsMatch(value);
  }

  private Dictionary<string, string> GenerateDictionary(
    IEnumerable<(string name, LineEdit input)> inputs,
    string propertyName)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach ((string name, LineEdit input) input in inputs)
      dictionary.Add(input.name, input.input.Text);
    return dictionary;
  }

  public static void CopyProperties<T>(T from, T to) where T : Control
  {
    foreach (KeyValuePair<AttachedProperty, object> attachedProperty in from.AllAttachedProperties)
      to.SetValue(attachedProperty.Key, attachedProperty.Value);
  }
}
