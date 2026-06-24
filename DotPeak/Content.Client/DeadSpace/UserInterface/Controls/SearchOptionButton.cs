// Decompiled with JetBrains decompiler
// Type: Content.Client.DeadSpace.UserInterface.Controls.SearchOptionButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Content.Client.DeadSpace.UserInterface.Controls;

[Virtual]
public class SearchOptionButton : HeadedOptionButton
{
  private LineEdit _searchBar;

  [Robust.Shared.ViewVariables.ViewVariables]
  public string? PlaceHolder
  {
    get => this._searchBar.PlaceHolder;
    set => this._searchBar.PlaceHolder = value;
  }

  public SearchOptionButton()
  {
    this._searchBar = new LineEdit();
    this._searchBar.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnSearchBarTextChanged);
    ((Control) this.ScrollHeading).AddChild((Control) this._searchBar);
  }

  public void ResetSearch()
  {
    this._searchBar.Text = "";
    this.FilterItems();
  }

  protected void FilterItems()
  {
    string lowerInvariant = this._searchBar.Text.Trim().ToLowerInvariant();
    foreach (HeadedOptionButton.ButtonData buttonData in this._buttonData)
    {
      if (!buttonData.Text.ToLowerInvariant().Contains(lowerInvariant))
        ((Control) buttonData.Button).Visible = false;
      else
        ((Control) buttonData.Button).Visible = true;
    }
  }

  protected void OnSearchBarTextChanged(LineEdit.LineEditEventArgs args) => this.FilterItems();
}
