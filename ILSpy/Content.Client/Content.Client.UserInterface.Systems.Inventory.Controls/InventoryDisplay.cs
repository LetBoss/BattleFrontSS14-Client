using System.Collections.Generic;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public sealed class InventoryDisplay : LayoutContainer
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("ui.inventory_display");

	private int Columns;

	private int Rows;

	private const int MarginThickness = 10;

	private const int ButtonSpacing = 5;

	private const int ButtonSize = 75;

	private readonly Control resizer;

	private readonly Dictionary<string, (SlotControl, Vector2i)> _buttons = new Dictionary<string, (SlotControl, Vector2i)>();

	public InventoryDisplay()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		resizer = new Control();
		((Control)this).AddChild(resizer);
	}

	public SlotControl AddButton(SlotControl newButton, Vector2i buttonOffset)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).AddChild((Control)(object)newButton);
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		((LayoutContainer)this).InheritChildMeasure = true;
		if (!_buttons.TryAdd(newButton.SlotName, (newButton, buttonOffset)))
		{
			Sawmill.Warning("Tried to add button without a slot!");
		}
		LayoutContainer.SetPosition((Control)(object)newButton, Vector2i.op_Implicit(buttonOffset * 75) + new Vector2(5f, 5f));
		UpdateSizeData(buttonOffset);
		return newButton;
	}

	public SlotControl? GetButton(string slotName)
	{
		if (_buttons.TryGetValue(slotName, out (SlotControl, Vector2i) value))
		{
			return value.Item1;
		}
		return null;
	}

	private void UpdateSizeData(Vector2i buttonOffset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = buttonOffset;
		int num = default(int);
		int num2 = default(int);
		((Vector2i)(ref val)).Deconstruct(ref num, ref num2);
		int num3 = num;
		if (num3 > Columns)
		{
			Columns = num3;
		}
		val = buttonOffset;
		((Vector2i)(ref val)).Deconstruct(ref num2, ref num);
		int num4 = num;
		if (num4 > Rows)
		{
			Rows = num4;
		}
		resizer.SetHeight = (Rows + 1) * 80;
		resizer.SetWidth = (Columns + 1) * 80;
	}

	public bool TryGetButton(string slotName, out SlotControl? button)
	{
		(SlotControl, Vector2i) value;
		bool result = _buttons.TryGetValue(slotName, out value);
		(button, _) = value;
		return result;
	}

	public void RemoveButton(string slotName)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!_buttons.Remove(slotName))
		{
			return;
		}
		Columns = 0;
		Rows = 0;
		foreach (KeyValuePair<string, (SlotControl, Vector2i)> button in _buttons)
		{
			button.Deconstruct(out var _, out var value);
			Vector2i item = value.Item2;
			UpdateSizeData(item);
		}
	}

	public void ClearButtons()
	{
		((Control)this).Children.Clear();
	}
}
