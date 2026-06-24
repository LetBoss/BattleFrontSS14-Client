using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;

namespace Content.Client.DeadSpace.UserInterface.Controls;

[Virtual]
public class HeadedOptionButton : ContainerButton
{
	public sealed class ItemSelectedEventArgs : EventArgs
	{
		public HeadedOptionButton Button { get; }

		public int Id { get; }

		public ItemSelectedEventArgs(int id, HeadedOptionButton button)
		{
			Id = id;
			Button = button;
		}
	}

	public sealed class ButtonData
	{
		public string Text;

		public bool Disabled;

		public object? Metadata;

		public int Id;

		public Button Button;

		public ButtonData(string text, Button button)
		{
			Text = text;
			Button = button;
		}
	}

	public const string StyleClassOptionButton = "optionButton";

	public const string StyleClassPopup = "optionButtonPopup";

	public const string StyleClassOptionTriangle = "optionTriangle";

	public const string StyleClassOptionsBackground = "optionButtonBackground";

	public readonly ScrollContainer OptionsScroll;

	public readonly List<ButtonData> _buttonData = new List<ButtonData>();

	private readonly Dictionary<int, int> _idMap = new Dictionary<int, int>();

	private readonly Popup _popup;

	private readonly BoxContainer _popupVBox;

	private readonly Label _label;

	private readonly TextureRect _triangle;

	public BoxContainer ScrollHeading;

	private bool _hideTriangle;

	public int ItemCount => _buttonData.Count;

	public bool HideTriangle
	{
		get
		{
			return _hideTriangle;
		}
		set
		{
			_hideTriangle = value;
			((Control)_triangle).Visible = !_hideTriangle;
		}
	}

	public ICollection<string> OptionStyleClasses { get; }

	public string Prefix { get; set; } = string.Empty;

	public bool PrefixMargin { get; set; } = true;

	public int SelectedId { get; private set; }

	public object? SelectedMetadata => _buttonData[_idMap[SelectedId]].Metadata;

	public event Action<ItemSelectedEventArgs>? OnItemSelected;

	public HeadedOptionButton()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		OptionStyleClasses = new List<string>();
		((Control)this).AddStyleClass("button");
		((BaseButton)this).OnPressed += OnPressedInternal;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)this).AddChild((Control)(object)val);
		_popupVBox = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		ScrollContainer val2 = new ScrollContainer();
		((Control)val2).Children.Add((Control)(object)_popupVBox);
		val2.ReturnMeasure = true;
		((Control)val2).MaxHeight = 300f;
		OptionsScroll = val2;
		ScrollHeading = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val3).Children.Add((Control)(object)ScrollHeading);
		((Control)val3).Children.Add((Control)(object)OptionsScroll);
		BoxContainer val4 = val3;
		Popup val5 = new Popup();
		((Control)val5).Children.Add((Control)new PanelContainer
		{
			StyleClasses = { "optionButtonBackground" }
		});
		((Control)val5).Children.Add((Control)(object)val4);
		((Control)val5).StyleClasses.Add("optionButtonPopup");
		_popup = val5;
		_popup.OnPopupHide += OnPopupHide;
		_label = new Label
		{
			StyleClasses = { "optionButton" },
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)_label);
		_triangle = new TextureRect
		{
			StyleClasses = { "optionTriangle" },
			VerticalAlignment = (VAlignment)2,
			Visible = !HideTriangle
		};
		((Control)val).AddChild((Control)(object)_triangle);
	}

	public void AddItem(Texture icon, string label, int? id = null)
	{
		AddItem(label, id);
	}

	public virtual void ButtonOverride(Button button)
	{
	}

	public void AddItem(string label, int? id = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		if (!id.HasValue)
		{
			id = _buttonData.Count;
		}
		if (_idMap.ContainsKey(id.Value))
		{
			throw new ArgumentException("An item with the same ID already exists.");
		}
		Button val = new Button
		{
			Text = label,
			ToggleMode = true
		};
		foreach (string optionStyleClass in OptionStyleClasses)
		{
			((Control)val).AddStyleClass(optionStyleClass);
		}
		((BaseButton)val).OnPressed += ButtonOnPressed;
		ButtonData item = new ButtonData(label, val)
		{
			Id = id.Value
		};
		_idMap.Add(id.Value, _buttonData.Count);
		_buttonData.Add(item);
		((Control)_popupVBox).AddChild((Control)(object)val);
		if (_buttonData.Count == 1)
		{
			Select(0);
		}
		ButtonOverride(val);
	}

	private void TogglePopup(bool show)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (show)
		{
			Vector2 globalPosition = ((Control)this).GlobalPosition;
			globalPosition.Y += ((Control)this).Size.Y + 1f;
			ref float y = ref globalPosition.Y;
			float num = y;
			Thickness margin = ((Control)this).Margin;
			y = num - ((Thickness)(ref margin)).SumVertical;
			ScrollContainer optionsScroll = OptionsScroll;
			IClydeWindow window = ((Control)this).Window;
			Vector2i? val = ((window != null) ? new Vector2i?(window.Size) : ((Vector2i?)null));
			((Control)optionsScroll).Measure(val.HasValue ? Vector2i.op_Implicit(val.GetValueOrDefault()) : Vector2Helpers.Infinity);
			float num2 = default(float);
			float num3 = default(float);
			Vector2Helpers.Deconstruct(((Control)OptionsScroll).DesiredSize, ref num2, ref num3);
			float val2 = num2;
			float y2 = num3;
			UIBox2 value = UIBox2.FromDimensions(globalPosition, new Vector2(Math.Max(val2, ((Control)this).Width), y2));
			((Control)((Control)this).UserInterfaceManager.ModalRoot).AddChild((Control)(object)_popup);
			_popup.Open((UIBox2?)value, (Vector2?)null, (Vector2?)null);
		}
		else
		{
			_popup.Close();
		}
	}

	private void OnPopupHide()
	{
		((Control)((Control)this).UserInterfaceManager.ModalRoot).RemoveChild((Control)(object)_popup);
	}

	private void ButtonOnPressed(ButtonEventArgs obj)
	{
		obj.Button.Pressed = false;
		TogglePopup(show: false);
		foreach (ButtonData buttonDatum in _buttonData)
		{
			if ((object)buttonDatum.Button == obj.Button)
			{
				this.OnItemSelected?.Invoke(new ItemSelectedEventArgs(buttonDatum.Id, this));
				return;
			}
		}
		throw new InvalidOperationException();
	}

	public void Clear()
	{
		_idMap.Clear();
		foreach (ButtonData buttonDatum in _buttonData)
		{
			((BaseButton)buttonDatum.Button).OnPressed -= ButtonOnPressed;
		}
		_buttonData.Clear();
		((Control)_popupVBox).DisposeAllChildren();
		SelectedId = 0;
	}

	public int GetItemId(int idx)
	{
		return _buttonData[idx].Id;
	}

	public object? GetItemMetadata(int idx)
	{
		return _buttonData[idx].Metadata;
	}

	public bool IsItemDisabled(int idx)
	{
		return _buttonData[idx].Disabled;
	}

	public void RemoveItem(int idx)
	{
		ButtonData buttonData = _buttonData[idx];
		((BaseButton)buttonData.Button).OnPressed -= ButtonOnPressed;
		_idMap.Remove(buttonData.Id);
		((Control)_popupVBox).RemoveChild((Control)(object)buttonData.Button);
		_buttonData.RemoveAt(idx);
		int num = 0;
		foreach (ButtonData buttonDatum in _buttonData)
		{
			_idMap[buttonDatum.Id] = num++;
		}
	}

	public void Select(int idx)
	{
		if (_idMap.TryGetValue(SelectedId, out var value))
		{
			((BaseButton)_buttonData[value].Button).Pressed = false;
		}
		ButtonData buttonData = _buttonData[idx];
		SelectedId = buttonData.Id;
		_label.Text = (PrefixMargin ? (Prefix + " " + buttonData.Text) : (Prefix + buttonData.Text));
		((BaseButton)buttonData.Button).Pressed = true;
	}

	public bool TrySelect(int idx)
	{
		if (idx < 0 || idx >= _buttonData.Count)
		{
			return false;
		}
		Select(idx);
		return true;
	}

	public void SelectId(int id)
	{
		Select(GetIdx(id));
	}

	public bool TrySelectId(int id)
	{
		if (_idMap.TryGetValue(id, out var value))
		{
			return TrySelect(value);
		}
		return false;
	}

	public int GetIdx(int id)
	{
		return _idMap[id];
	}

	public void SetItemDisabled(int idx, bool disabled)
	{
		ButtonData buttonData = _buttonData[idx];
		buttonData.Disabled = disabled;
		((BaseButton)buttonData.Button).Disabled = disabled;
	}

	public void SetItemId(int idx, int id)
	{
		if (_idMap.TryGetValue(id, out var value) && value != idx)
		{
			throw new InvalidOperationException("An item with said ID already exists.");
		}
		ButtonData buttonData = _buttonData[idx];
		_idMap.Remove(buttonData.Id);
		_idMap.Add(id, idx);
		buttonData.Id = id;
	}

	public void SetItemMetadata(int idx, object metadata)
	{
		_buttonData[idx].Metadata = metadata;
	}

	public void SetItemText(int idx, string text)
	{
		ButtonData buttonData = _buttonData[idx];
		buttonData.Text = text;
		if (SelectedId == buttonData.Id)
		{
			_label.Text = text;
		}
		buttonData.Button.Text = text;
	}

	private void OnPressedInternal(ButtonEventArgs args)
	{
		TogglePopup(show: true);
	}

	protected override void ExitedTree()
	{
		((Control)this).ExitedTree();
		TogglePopup(show: false);
	}
}
