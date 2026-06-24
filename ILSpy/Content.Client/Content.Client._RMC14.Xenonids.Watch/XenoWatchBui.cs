using System;
using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Watch;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Watch;

public sealed class XenoWatchBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	[ViewVariables]
	private XenoWatchWindow? _window;

	private readonly SpriteSystem _sprite;

	public XenoWatchBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		EnsureWindow();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!(state is XenoWatchBuiState xenoWatchBuiState))
		{
			return;
		}
		_window = EnsureWindow();
		_window.BurrowedLarvaLabel.Text = $"Burrowed Larva: {xenoWatchBuiState.BurrowedLarva}";
		((Control)_window.XenoContainer).DisposeAllChildren();
		EntityPrototype val = default(EntityPrototype);
		foreach (Xeno xeno in xenoWatchBuiState.Xenos)
		{
			Texture texture = null;
			if (xeno.Id.HasValue && _prototype.TryIndex(xeno.Id.Value, ref val))
			{
				texture = _sprite.Frame0(val);
			}
			XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
			xenoChoiceControl.Set(xeno.Name, texture);
			((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoWatchBuiMsg(xeno.Entity));
			};
			((Control)_window.XenoContainer).AddChild((Control)(object)xenoChoiceControl);
		}
	}

	private XenoWatchWindow EnsureWindow()
	{
		if (_window != null)
		{
			return _window;
		}
		_window = BoundUserInterfaceExt.CreateWindow<XenoWatchWindow>((BoundUserInterface)(object)this);
		_window.SearchBar.OnTextChanged += OnSearchBarChanged;
		return _window;
	}

	private unsafe void OnSearchBarChanged(LineEditEventArgs args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		XenoWatchWindow window = _window;
		if (window == null || ((Control)window).Disposed)
		{
			return;
		}
		Enumerator enumerator = ((Control)_window.XenoContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is XenoChoiceControl xenoChoiceControl)
				{
					if (string.IsNullOrWhiteSpace(args.Text))
					{
						((Control)xenoChoiceControl).Visible = true;
					}
					else
					{
						((Control)xenoChoiceControl).Visible = xenoChoiceControl.NameLabel.GetMessage()?.Contains(args.Text, StringComparison.OrdinalIgnoreCase) ?? false;
					}
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
