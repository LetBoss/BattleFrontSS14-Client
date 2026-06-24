using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Hands.Controls;

public sealed class HandsContainer : ItemSlotUIContainer<HandButton>
{
	private readonly GridContainer _grid;

	public int ColumnLimit
	{
		get
		{
			return _grid.Columns;
		}
		set
		{
			_grid.Columns = value;
		}
	}

	public int MaxButtonCount { get; set; }

	public int MaxButtonsPerRow { get; set; } = 6;

	public string? Indexer { get; set; }

	public bool IsFull
	{
		get
		{
			if (MaxButtonCount != 0)
			{
				return ButtonCount >= MaxButtonCount;
			}
			return false;
		}
	}

	public int ButtonCount => ((Control)_grid).ChildCount;

	public HandsContainer()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001b: Expected O, but got Unknown
		GridContainer val = new GridContainer();
		GridContainer val2 = val;
		_grid = val;
		((Control)this).AddChild((Control)(object)val2);
		_grid.ExpandBackwards = true;
	}

	public override HandButton? AddButton(HandButton newButton)
	{
		if (MaxButtonCount > 0)
		{
			if (ButtonCount >= MaxButtonCount)
			{
				return null;
			}
			((Control)_grid).AddChild((Control)(object)newButton);
		}
		else
		{
			((Control)_grid).AddChild((Control)(object)newButton);
		}
		_grid.Columns = Math.Min(((Control)_grid).ChildCount, MaxButtonsPerRow);
		return base.AddButton(newButton);
	}

	public override void RemoveButton(string handName)
	{
		HandButton button = GetButton(handName);
		if (button != null)
		{
			base.RemoveButton(button);
			((Control)_grid).RemoveChild((Control)(object)button);
		}
	}

	public bool TryGetLastButton(out HandButton? control)
	{
		if (Buttons.Count == 0)
		{
			control = null;
			return false;
		}
		control = Buttons.Values.Last();
		return true;
	}

	public bool TryRemoveLastHand(out HandButton? control)
	{
		bool result = TryGetLastButton(out control);
		if (control != null)
		{
			RemoveButton(control);
		}
		return result;
	}

	public void Clear()
	{
		ClearButtons();
		((Control)_grid).DisposeAllChildren();
	}

	public unsafe IEnumerable<HandButton> GetButtons()
	{
		Enumerator val = ((Control)_grid).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref val)).MoveNext())
			{
				if (((Enumerator)(ref val)).Current is HandButton handButton)
				{
					yield return handButton;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
		val = default(Enumerator);
	}
}
