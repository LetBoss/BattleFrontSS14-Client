using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._RMC14.UserInterface;
using Content.Shared._RMC14.Intel.Tech;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Intel;

public sealed class TechControlConsoleBui : BoundUserInterface
{
	[Dependency]
	private IEntityManager _entities;

	private TechControlConsoleWindow? _window;

	private TechControlConsoleOptionWindow? _optionWindow;

	private readonly SharedGameTicker _ticker;

	private readonly SpriteSystem _sprite;

	public TechControlConsoleBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_ticker = _entities.System<SharedGameTicker>();
		_sprite = _entities.System<SpriteSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<TechControlConsoleWindow>((BoundUserInterface)(object)this);
		Refresh();
	}

	public void Refresh()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Expected O, but got Unknown
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Expected O, but got Unknown
		TechControlConsoleWindow window = _window;
		TechControlConsoleComponent console = default(TechControlConsoleComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<TechControlConsoleComponent>(((BoundUserInterface)this).Owner, ref console))
		{
			return;
		}
		((Control)_window.Options).DisposeAllChildren();
		for (int num = console.Tree.Options.Count - 1; num >= 0; num--)
		{
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			RichTextLabel val2 = new RichTextLabel();
			val2.Text = Loc.GetString("rmc-ui-tech-tier-header", new(string, object)[1] { ("tier", num) });
			((Control)val).AddChild((Control)(object)val2);
			if (num == console.Tree.Options.Count - 1)
			{
				((Control)val).AddChild(new Control
				{
					HorizontalExpand = true
				});
				val2 = new RichTextLabel();
				val2.Text = Loc.GetString("rmc-ui-tech-points", new(string, object)[1] { ("points", console.Tree.Points) });
				((Control)val).AddChild((Control)(object)val2);
			}
			((Control)_window.Options).AddChild((Control)(object)val);
			((Control)_window.Options).AddChild((Control)(object)new BlueHorizontalSeparator());
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			((Control)_window.Options).AddChild((Control)(object)val3);
			List<TechOption> list = console.Tree.Options[num];
			for (int i = 0; i < list.Count; i++)
			{
				TechOption option = list[i];
				Control val4 = new Control();
				Texture textureNormal = ((IDirectionalTextureProvider)_sprite.RsiStateLike((SpriteSpecifier)(object)option.Icon)).TextureFor((Direction)0);
				val4.AddChild((Control)new TextureButton
				{
					TextureNormal = textureNormal,
					SetSize = new Vector2(64f, 64f)
				});
				Rsi val5 = (option.Purchased ? console.UnlockedRsi : console.LockedRsi);
				TextureButton val6 = new TextureButton
				{
					TextureNormal = ((IDirectionalTextureProvider)_sprite.RsiStateLike((SpriteSpecifier)(object)val5)).TextureFor((Direction)0),
					Scale = new Vector2(2f, 2f)
				};
				val4.AddChild((Control)(object)val6);
				int tier = num;
				int optionIndex = i;
				((BaseButton)val6).OnPressed += delegate
				{
					OpenOptionWindow(option, tier, optionIndex, console.Tree.Points, console.Tree.Tier);
				};
				((Control)val6).ToolTip = option.Name;
				((Control)val6).TooltipDelay = 0.1f;
				((Control)val3).AddChild(new Control
				{
					HorizontalExpand = true
				});
				((Control)val3).AddChild(val4);
				if (i == list.Count - 1)
				{
					((Control)val3).AddChild(new Control
					{
						HorizontalExpand = true
					});
				}
			}
		}
	}

	private void OpenOptionWindow(TechOption option, int tier, int optionIndex, FixedPoint2 points, int currentTier)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		TechControlConsoleOptionWindow optionWindow = _optionWindow;
		if (optionWindow != null && ((BaseWindow)optionWindow).IsOpen)
		{
			((BaseWindow)_optionWindow).Close();
			_optionWindow = null;
		}
		_optionWindow = new TechControlConsoleOptionWindow();
		((BaseWindow)_optionWindow).OpenCentered();
		((BaseWindow)_optionWindow).OnClose += delegate
		{
			_optionWindow = null;
		};
		((DefaultWindow)_optionWindow).Title = option.Name;
		_optionWindow.CurrentPointsLabel.Text = Loc.GetString("rmc-ui-tech-points-value", new(string, object)[1] { ("value", points.Double().ToString("F1")) });
		_optionWindow.NameLabel.Text = option.Name;
		_optionWindow.DescriptionLabel.Text = option.Description;
		_optionWindow.CostLabel.Text = $"{option.CurrentCost}";
		((Control)_optionWindow.Statistics).DisposeAllChildren();
		bool visible = false;
		if (option.Repurchasable)
		{
			visible = true;
			((Control)_optionWindow.Statistics).AddChild((Control)new Label
			{
				Text = Loc.GetString("rmc-ui-tech-repurchasable")
			});
		}
		if (option.Increase != 0)
		{
			visible = true;
			BoxContainer statistics = _optionWindow.Statistics;
			Label val = new Label();
			val.Text = Loc.GetString("rmc-ui-tech-incremental-price", new(string, object)[1] { ("increase", option.Increase) });
			((Control)statistics).AddChild((Control)(object)val);
		}
		((Control)_optionWindow.StatisticsContainer).Visible = visible;
		bool flag = points >= option.CurrentCost && currentTier >= tier && (!option.Purchased || option.Repurchasable) && option.TimeLock < _ticker.RoundDuration();
		_optionWindow.PurchaseButton.Text = Loc.GetString("rmc-ui-tech-purchase-button");
		((BaseButton)_optionWindow.PurchaseButton).Disabled = !flag;
		((BaseButton)_optionWindow.PurchaseButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new TechPurchaseOptionBuiMsg(tier, optionIndex));
			((BaseWindow)_optionWindow).Close();
		};
	}
}
