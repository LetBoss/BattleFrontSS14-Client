using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls;

public sealed class SearchListContainer : ListContainer
{
	private LineEdit? _searchBar;

	private List<ListData> _unfilteredData = new List<ListData>();

	public Func<string, ListData, bool>? DataFilterCondition;

	public LineEdit? SearchBar
	{
		get
		{
			return _searchBar;
		}
		set
		{
			if (_searchBar != null)
			{
				_searchBar.OnTextChanged -= FilterList;
			}
			_searchBar = value;
			if (_searchBar != null)
			{
				_searchBar.OnTextChanged += FilterList;
			}
		}
	}

	public override void PopulateList(IReadOnlyList<ListData> data)
	{
		_unfilteredData = data.ToList();
		FilterList();
	}

	private void FilterList(LineEditEventArgs obj)
	{
		FilterList();
	}

	private void FilterList()
	{
		LineEdit? searchBar = SearchBar;
		string text = ((searchBar != null) ? searchBar.Text : null);
		if (DataFilterCondition == null || string.IsNullOrEmpty(text))
		{
			base.PopulateList(_unfilteredData);
			return;
		}
		List<ListData> list = new List<ListData>();
		foreach (ListData unfilteredDatum in _unfilteredData)
		{
			if (DataFilterCondition(text, unfilteredDatum))
			{
				list.Add(unfilteredDatum);
			}
		}
		base.PopulateList(list);
	}
}
