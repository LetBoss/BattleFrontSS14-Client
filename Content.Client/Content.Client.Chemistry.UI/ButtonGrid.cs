using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Chemistry.UI;

public sealed class ButtonGrid : GridContainer
{
	private string _buttonList = "";

	private string? _selected;

	public Action<string>? OnButtonPressed;

	public string ButtonList
	{
		get
		{
			return _buttonList;
		}
		set
		{
			_buttonList = value;
			Update();
		}
	}

	public bool RadioGroup { get; set; }

	public string? Selected
	{
		get
		{
			return _selected;
		}
		set
		{
			_selected = value;
			Update();
		}
	}

	public int Columns
	{
		get
		{
			return ((GridContainer)this).Columns;
		}
		set
		{
			((GridContainer)this).Columns = value;
			Update();
		}
	}

	public int Rows
	{
		get
		{
			return ((GridContainer)this).Rows;
		}
		set
		{
			((GridContainer)this).Rows = value;
			Update();
		}
	}

	private void Update()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		if (ButtonList == "")
		{
			return;
		}
		((Control)this).Children.Clear();
		int num = 0;
		string[] array = ButtonList.Split(",");
		ButtonGroup val = new ButtonGroup(true);
		string[] array2 = array;
		foreach (string button in array2)
		{
			Button btn = new Button();
			btn.Text = button;
			((BaseButton)btn).OnPressed += delegate
			{
				if (RadioGroup)
				{
					((BaseButton)btn).Pressed = true;
				}
				Selected = button;
				OnButtonPressed?.Invoke(button);
			};
			if (button == Selected)
			{
				((BaseButton)btn).Pressed = true;
			}
			((GridContainer)this).HSeparationOverride.GetValueOrDefault();
			((BaseButton)btn).Group = val;
			int num2 = num / Columns;
			int num3 = num % Columns;
			bool flag = num == array.Length - 1;
			bool flag2 = num == Columns - 1;
			bool flag3 = num2 == array.Length / Columns - 1;
			if (num2 == 0 && (flag2 || flag))
			{
				((Control)btn).AddStyleClass("OpenLeft");
			}
			else if (num3 == 0 && flag3)
			{
				((Control)btn).AddStyleClass("OpenRight");
			}
			else
			{
				((Control)btn).AddStyleClass("OpenBoth");
			}
			((Control)this).Children.Add((Control)(object)btn);
			num++;
		}
	}
}
