using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Log;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public abstract class ItemSlotUIContainer<T> : GridContainer, IItemslotUIContainer where T : SlotControl
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("ui.item_slot_container");

	protected readonly Dictionary<string, T> Buttons = new Dictionary<string, T>();

	private int? _maxColumns;

	public int? MaxColumns
	{
		get
		{
			return _maxColumns;
		}
		set
		{
			_maxColumns = value;
		}
	}

	public virtual bool TryAddButton(T newButton, out T button)
	{
		if (AddButton(newButton) == null)
		{
			button = newButton;
			return false;
		}
		button = newButton;
		return true;
	}

	public void ClearButtons()
	{
		foreach (T value in Buttons.Values)
		{
			((Control)value).Orphan();
		}
		Buttons.Clear();
	}

	public bool TryRegisterButton(SlotControl control, string newSlotName)
	{
		if (newSlotName == "")
		{
			return false;
		}
		if (!(control is T val))
		{
			return false;
		}
		if (Buttons.TryGetValue(newSlotName, out var value))
		{
			if (control == value)
			{
				return true;
			}
			throw new Exception("Could not update button to slot:" + newSlotName + " slot already assigned!");
		}
		Buttons.Remove(val.SlotName);
		AddButton(val);
		return true;
	}

	public bool TryAddButton(SlotControl control)
	{
		if (!(control is T newButton))
		{
			return false;
		}
		return AddButton(newButton) != null;
	}

	public virtual T? AddButton(T newButton)
	{
		if (!((Control)this).Children.Contains((Control)(object)newButton) && ((Control)newButton).Parent == null && newButton.SlotName != "")
		{
			((Control)this).AddChild((Control)(object)newButton);
		}
		((GridContainer)this).Columns = _maxColumns ?? ((Control)this).ChildCount;
		return AddButtonToDict(newButton);
	}

	protected virtual T? AddButtonToDict(T newButton)
	{
		if (newButton.SlotName == "")
		{
			Sawmill.Warning("Could not add button " + ((Control)newButton).Name + "No slotname");
		}
		if (Buttons.TryAdd(newButton.SlotName, newButton))
		{
			return newButton;
		}
		return null;
	}

	public virtual void RemoveButton(string slotName)
	{
		if (Buttons.TryGetValue(slotName, out var value))
		{
			RemoveButton(value);
		}
	}

	public virtual void RemoveButtons(params string[] slotNames)
	{
		foreach (string slotName in slotNames)
		{
			RemoveButton(slotName);
		}
	}

	public virtual void RemoveButtons(params T?[] buttons)
	{
		foreach (T val in buttons)
		{
			if (val != null)
			{
				RemoveButton(val);
			}
		}
	}

	protected virtual void RemoveButtonFromDict(T button)
	{
		Buttons.Remove(button.SlotName);
	}

	public virtual void RemoveButton(T button)
	{
		RemoveButtonFromDict(button);
		((Control)this).Children.Remove((Control)(object)button);
		((Control)button).Orphan();
	}

	public virtual T? GetButton(string slotName)
	{
		if (Buttons.TryGetValue(slotName, out var value))
		{
			return value;
		}
		return null;
	}

	public virtual bool TryGetButton(string slotName, [NotNullWhen(true)] out T? button)
	{
		return (button = GetButton(slotName)) != null;
	}
}
