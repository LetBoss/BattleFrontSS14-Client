// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.SearchListContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class SearchListContainer : ListContainer
{
  private LineEdit? _searchBar;
  private List<ListData> _unfilteredData = new List<ListData>();
  public Func<string, ListData, bool>? DataFilterCondition;

  public LineEdit? SearchBar
  {
    get => this._searchBar;
    set
    {
      if (this._searchBar != null)
        this._searchBar.OnTextChanged -= new Action<LineEdit.LineEditEventArgs>(this.FilterList);
      this._searchBar = value;
      if (this._searchBar == null)
        return;
      this._searchBar.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.FilterList);
    }
  }

  public override void PopulateList(IReadOnlyList<ListData> data)
  {
    this._unfilteredData = data.ToList<ListData>();
    this.FilterList();
  }

  private void FilterList(LineEdit.LineEditEventArgs obj) => this.FilterList();

  private void FilterList()
  {
    string text = this.SearchBar?.Text;
    if (this.DataFilterCondition == null || string.IsNullOrEmpty(text))
    {
      base.PopulateList((IReadOnlyList<ListData>) this._unfilteredData);
    }
    else
    {
      List<ListData> data = new List<ListData>();
      foreach (ListData listData in this._unfilteredData)
      {
        if (this.DataFilterCondition(text, listData))
          data.Add(listData);
      }
      base.PopulateList((IReadOnlyList<ListData>) data);
    }
  }
}
