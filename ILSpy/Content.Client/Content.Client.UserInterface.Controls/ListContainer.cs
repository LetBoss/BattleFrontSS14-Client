using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class ListContainer : Control
{
	public const string StylePropertySeparation = "separation";

	public const string StyleClassListContainerButton = "list-container-button";

	public Action<ListData, ListContainerButton>? GenerateItem;

	public Action<ButtonEventArgs, ListData>? ItemPressed;

	public Action<GUIBoundKeyEventArgs, ListData>? ItemKeyBindDown;

	public Action? NoItemSelected;

	private const int DefaultSeparation = 3;

	private readonly VScrollBar _vScrollBar;

	private readonly Dictionary<ListData, ListContainerButton> _buttons = new Dictionary<ListData, ListContainerButton>();

	private List<ListData> _data = new List<ListData>();

	private ListData? _selected;

	private float _itemHeight;

	private float _totalHeight;

	private int _topIndex;

	private int _bottomIndex;

	private bool _updateChildren;

	private bool _suppressScrollValueChanged;

	private ButtonGroup? _buttonGroup;

	public int? SeparationOverride { get; set; }

	public bool Group
	{
		get
		{
			return _buttonGroup != null;
		}
		set
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_buttonGroup = ((!value) ? ((ButtonGroup)null) : new ButtonGroup(true));
		}
	}

	public bool Toggle { get; set; }

	public IReadOnlyList<ListData> Data => _data;

	public int ScrollSpeedY { get; set; } = 50;

	private int ActualSeparation
	{
		get
		{
			int result = default(int);
			if (((Control)this).TryGetStyleProperty<int>("separation", ref result))
			{
				return result;
			}
			return SeparationOverride ?? 3;
		}
	}

	public ListContainer()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		((Control)this).RectClipContent = true;
		((Control)this).MouseFilter = (MouseFilterMode)1;
		_vScrollBar = new VScrollBar
		{
			HorizontalExpand = false,
			HorizontalAlignment = (HAlignment)3
		};
		((Control)this).AddChild((Control)(object)_vScrollBar);
		((Range)_vScrollBar).OnValueChanged += ScrollValueChanged;
	}

	public virtual void PopulateList(IReadOnlyList<ListData> data)
	{
		if (_itemHeight != 0f)
		{
			List<ListData> data2 = _data;
			if (data2 == null || data2.Count != 0)
			{
				goto IL_0078;
			}
		}
		if (data.Count > 0)
		{
			ListContainerButton listContainerButton = new ListContainerButton(data[0], 0);
			GenerateItem?.Invoke(data[0], listContainerButton);
			((Control)this).AddChild((Control)(object)listContainerButton);
			((Control)listContainerButton).Measure(Vector2Helpers.Infinity);
			_itemHeight = ((Control)listContainerButton).DesiredSize.Y;
			((Control)listContainerButton).Orphan();
		}
		goto IL_0078;
		IL_0078:
		foreach (ListContainerButton value in _buttons.Values)
		{
			((Control)value).Orphan();
		}
		_buttons.Clear();
		_data = data.ToList();
		_updateChildren = true;
		((Control)this).InvalidateArrange();
		if (_selected != null && !data.Contains<ListData>(_selected))
		{
			_selected = null;
			NoItemSelected?.Invoke();
		}
	}

	public void DirtyList()
	{
		_updateChildren = true;
		((Control)this).InvalidateArrange();
	}

	public void Select(ListData data)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		if (_data.Contains(data))
		{
			if (_buttons.TryGetValue(data, out ListContainerButton value) && Toggle)
			{
				((BaseButton)value).Pressed = true;
			}
			_selected = data;
			if (value == null)
			{
				value = new ListContainerButton(data, _data.IndexOf(data));
			}
			OnItemPressed(new ButtonEventArgs((BaseButton)(object)value, new GUIBoundKeyEventArgs(EngineKeyFunctions.UIClick, (BoundKeyState)0, new ScreenCoordinates(0f, 0f, WindowId.Main), true, Vector2.Zero, Vector2.Zero)));
		}
	}

	private void OnItemPressed(ButtonEventArgs args)
	{
		if (args.Button is ListContainerButton listContainerButton)
		{
			_selected = listContainerButton.Data;
			ItemPressed?.Invoke(args, listContainerButton.Data);
		}
	}

	private void OnItemKeyBindDown(ListContainerButton button, GUIBoundKeyEventArgs args)
	{
		ItemKeyBindDown?.Invoke(args, button.Data);
	}

	private Vector2 GetScrollValue()
	{
		float y = ((Range)_vScrollBar).Value;
		if (!((Control)_vScrollBar).Visible)
		{
			y = 0f;
		}
		return new Vector2(0f, y);
	}

	protected unsafe override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		float totalHeight = _totalHeight;
		float x = ((Control)_vScrollBar).DesiredSize.X;
		float num = default(float);
		float num2 = default(float);
		Vector2Helpers.Deconstruct(finalSize, ref num, ref num2);
		float num3 = num;
		float num4 = num2;
		try
		{
			_suppressScrollValueChanged = true;
			if (num4 < totalHeight)
			{
				num3 -= x;
			}
			if (num4 < totalHeight)
			{
				((Control)_vScrollBar).Visible = true;
				((Range)_vScrollBar).Page = num4;
				((Range)_vScrollBar).MaxValue = totalHeight;
			}
			else
			{
				((Control)_vScrollBar).Visible = false;
			}
		}
		finally
		{
			_suppressScrollValueChanged = false;
		}
		if (((Control)_vScrollBar).Visible)
		{
			((Control)_vScrollBar).Arrange(UIBox2.FromDimensions(Vector2.Zero, finalSize));
		}
		Vector2 scrollValue = GetScrollValue();
		int topIndex = _topIndex;
		_topIndex = (int)((scrollValue.Y + (float)ActualSeparation) / (_itemHeight + (float)ActualSeparation));
		if (_topIndex != topIndex)
		{
			_updateChildren = true;
		}
		int bottomIndex = _bottomIndex;
		_bottomIndex = (int)Math.Ceiling((scrollValue.Y + num4) / (_itemHeight + (float)ActualSeparation));
		_bottomIndex = Math.Min(_bottomIndex, _data.Count);
		if (_bottomIndex != bottomIndex)
		{
			_updateChildren = true;
		}
		if (_updateChildren)
		{
			_updateChildren = false;
			Dictionary<ListData, ListContainerButton> dictionary = new Dictionary<ListData, ListContainerButton>(_buttons);
			if (_data.Count > 0)
			{
				for (int i = _topIndex; i < _bottomIndex; i++)
				{
					ListData listData = _data[i];
					if (_buttons.TryGetValue(listData, out ListContainerButton button))
					{
						dictionary.Remove(listData);
					}
					else
					{
						button = new ListContainerButton(listData, i);
						((BaseButton)button).OnPressed += OnItemPressed;
						((Control)button).OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
						{
							OnItemKeyBindDown(button, args);
						};
						((BaseButton)button).ToggleMode = Toggle;
						((BaseButton)button).Group = _buttonGroup;
						GenerateItem?.Invoke(listData, button);
						_buttons.Add(listData, button);
						if (Toggle && listData == _selected)
						{
							((BaseButton)button).Pressed = true;
						}
						((Control)this).AddChild((Control)(object)button);
					}
					((Control)button).SetPositionInParent(i - _topIndex);
					((Control)button).Measure(finalSize);
				}
			}
			foreach (var (key, listContainerButton2) in dictionary)
			{
				_buttons.Remove(key);
				((Control)listContainerButton2).Orphan();
			}
			((Control)_vScrollBar).SetPositionLast();
		}
		int num5 = (int)(num3 * ((Control)this).UIScale);
		int num6 = (int)((float)ActualSeparation * ((Control)this).UIScale);
		int num7 = (int)(0f - (scrollValue.Y - (float)_topIndex * (_itemHeight + (float)ActualSeparation)) * ((Control)this).UIScale);
		bool flag = true;
		Enumerator enumerator2 = ((Control)this).Children.GetEnumerator();
		try
		{
			UIBox2i val = default(UIBox2i);
			while (((Enumerator)(ref enumerator2)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator2)).Current;
				if ((object)current != _vScrollBar)
				{
					if (!flag)
					{
						num7 += num6;
					}
					flag = false;
					int y = current.DesiredPixelSize.Y;
					((UIBox2i)(ref val))._002Ector(0, num7, num5, num7 + y);
					current.ArrangePixel(val);
					num7 += y;
				}
			}
			return finalSize;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	protected unsafe override Vector2 MeasureOverride(Vector2 availableSize)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((Control)_vScrollBar).Measure(availableSize);
		availableSize.X -= ((Control)_vScrollBar).DesiredSize.X;
		Vector2 vector = new Vector2(availableSize.X, float.PositiveInfinity);
		Vector2 value = Vector2.Zero;
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				current.Measure(vector);
				if ((object)current != _vScrollBar)
				{
					value = Vector2.Max(value, current.DesiredSize);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		if (_itemHeight == 0f && value.Y != 0f)
		{
			_itemHeight = value.Y;
		}
		_totalHeight = _itemHeight * (float)_data.Count + (float)(ActualSeparation * (_data.Count - 1));
		return new Vector2(value.X, 0f);
	}

	private void ScrollValueChanged(Range _)
	{
		if (!_suppressScrollValueChanged)
		{
			((Control)this).InvalidateArrange();
		}
	}

	protected override void MouseWheel(GUIMouseWheelEventArgs args)
	{
		((Control)this).MouseWheel(args);
		VScrollBar vScrollBar = _vScrollBar;
		((ScrollBar)vScrollBar).ValueTarget = ((ScrollBar)vScrollBar).ValueTarget - args.Delta.Y * (float)ScrollSpeedY;
		((InputEventArgs)args).Handle();
	}
}
