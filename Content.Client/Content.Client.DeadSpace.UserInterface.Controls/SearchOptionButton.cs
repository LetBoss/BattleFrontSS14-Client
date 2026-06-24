using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.ViewVariables;

namespace Content.Client.DeadSpace.UserInterface.Controls;

[Virtual]
public class SearchOptionButton : HeadedOptionButton
{
	private LineEdit _searchBar;

	[ViewVariables]
	public string? PlaceHolder
	{
		get
		{
			return _searchBar.PlaceHolder;
		}
		set
		{
			_searchBar.PlaceHolder = value;
		}
	}

	public SearchOptionButton()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		_searchBar = new LineEdit();
		_searchBar.OnTextChanged += OnSearchBarTextChanged;
		((Control)ScrollHeading).AddChild((Control)(object)_searchBar);
	}

	public void ResetSearch()
	{
		_searchBar.Text = "";
		FilterItems();
	}

	protected void FilterItems()
	{
		string value = _searchBar.Text.Trim().ToLowerInvariant();
		foreach (ButtonData buttonDatum in _buttonData)
		{
			if (!buttonDatum.Text.ToLowerInvariant().Contains(value))
			{
				((Control)buttonDatum.Button).Visible = false;
			}
			else
			{
				((Control)buttonDatum.Button).Visible = true;
			}
		}
	}

	protected void OnSearchBarTextChanged(LineEditEventArgs args)
	{
		FilterItems();
	}
}
