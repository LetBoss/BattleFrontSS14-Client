using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class MenuButton : ContainerButton
{
	[Dependency]
	private IInputManager _inputManager;

	public const string StyleClassLabelTopButton = "topButtonLabel";

	public const string StyleClassRedTopButton = "topButtonLabel";

	private static readonly Color ColorNormal = Color.FromHex((ReadOnlySpan<char>)"#7b7e9e", (Color?)null);

	private static readonly Color ColorRedNormal = Color.FromHex((ReadOnlySpan<char>)"#FEFEFE", (Color?)null);

	private static readonly Color ColorHovered = Color.FromHex((ReadOnlySpan<char>)"#9699bb", (Color?)null);

	private static readonly Color ColorRedHovered = Color.FromHex((ReadOnlySpan<char>)"#FFFFFF", (Color?)null);

	private static readonly Color ColorPressed = Color.FromHex((ReadOnlySpan<char>)"#789B8C", (Color?)null);

	private const float VertPad = 8f;

	private BoundKeyFunction _function;

	private readonly BoxContainer _root;

	private readonly TextureRect? _buttonIcon;

	private readonly Label? _buttonLabel;

	private Color NormalColor
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (!((Control)this).HasStyleClass("topButtonLabel"))
			{
				return ColorNormal;
			}
			return ColorRedNormal;
		}
	}

	private Color HoveredColor
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (!((Control)this).HasStyleClass("topButtonLabel"))
			{
				return ColorHovered;
			}
			return ColorRedHovered;
		}
	}

	public string AppendStyleClass
	{
		set
		{
			((Control)this).AddStyleClass(value);
		}
	}

	public Texture? Icon
	{
		get
		{
			return _buttonIcon.Texture;
		}
		set
		{
			_buttonIcon.Texture = value;
		}
	}

	public BoundKeyFunction BoundKey
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _function;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			_function = value;
			_buttonLabel.Text = BoundKeyHelper.ShortKeyName(value);
		}
	}

	public BoxContainer ButtonRoot => _root;

	public MenuButton()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		IoCManager.InjectDependencies<MenuButton>(this);
		_buttonIcon = new TextureRect
		{
			TextureScale = new Vector2(0.5f, 0.5f),
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			VerticalExpand = true,
			Margin = new Thickness(0f, 8f),
			ModulateSelfOverride = NormalColor,
			Stretch = (StretchMode)4
		};
		_buttonLabel = new Label
		{
			Text = "",
			HorizontalAlignment = (HAlignment)2,
			ModulateSelfOverride = NormalColor,
			StyleClasses = { "topButtonLabel" }
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val).Children.Add((Control)(object)_buttonIcon);
		((Control)val).Children.Add((Control)(object)_buttonLabel);
		_root = val;
		((Control)this).AddChild((Control)(object)_root);
		((BaseButton)this).ToggleMode = true;
	}

	protected override void EnteredTree()
	{
		_inputManager.OnKeyBindingAdded += OnKeyBindingChanged;
		_inputManager.OnKeyBindingRemoved += OnKeyBindingChanged;
		_inputManager.OnInputModeChanged += OnKeyBindingChanged;
	}

	protected override void ExitedTree()
	{
		_inputManager.OnKeyBindingAdded -= OnKeyBindingChanged;
		_inputManager.OnKeyBindingRemoved -= OnKeyBindingChanged;
		_inputManager.OnInputModeChanged -= OnKeyBindingChanged;
	}

	private void OnKeyBindingChanged(IKeyBinding obj)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_buttonLabel.Text = BoundKeyHelper.ShortKeyName(_function);
	}

	private void OnKeyBindingChanged()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_buttonLabel.Text = BoundKeyHelper.ShortKeyName(_function);
	}

	protected override void StylePropertiesChanged()
	{
		((Control)this).StylePropertiesChanged();
		UpdateChildColors();
	}

	private void UpdateChildColors()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected I4, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (_buttonIcon != null && _buttonLabel != null)
		{
			DrawModeEnum drawMode = ((BaseButton)this).DrawMode;
			switch ((int)drawMode)
			{
			case 0:
				((Control)_buttonIcon).ModulateSelfOverride = NormalColor;
				((Control)_buttonLabel).ModulateSelfOverride = NormalColor;
				break;
			case 1:
				((Control)_buttonIcon).ModulateSelfOverride = ColorPressed;
				((Control)_buttonLabel).ModulateSelfOverride = ColorPressed;
				break;
			case 2:
				((Control)_buttonIcon).ModulateSelfOverride = HoveredColor;
				((Control)_buttonLabel).ModulateSelfOverride = HoveredColor;
				break;
			case 3:
				break;
			}
		}
	}

	protected override void DrawModeChanged()
	{
		((ContainerButton)this).DrawModeChanged();
		UpdateChildColors();
	}
}
