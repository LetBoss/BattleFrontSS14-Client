using System;
using System.Collections.Generic;
using Content.Client._RMC14.UserInterface;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared.Stacks;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Construction;

public sealed class RMCConstructionBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IPlayerManager _player;

	private readonly SpriteSystem _sprite;

	private Texture? _materialIcon;

	private Texture? _hammerIcon;

	[ViewVariables]
	private RMCConstructionWindow? _window;

	public RMCConstructionBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCConstructionWindow>((BoundUserInterface)(object)this);
		((DefaultWindow)_window).Title = "Construction using the " + base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		RMCConstructionItemComponent rMCConstructionItemComponent = default(RMCConstructionItemComponent);
		if (base.EntMan.TryGetComponent<RMCConstructionItemComponent>(((BoundUserInterface)this).Owner, ref rMCConstructionItemComponent))
		{
			ProtoId<RMCConstructionPrototype>[] buildable = rMCConstructionItemComponent.Buildable;
			if (buildable != null)
			{
				Refresh(buildable);
			}
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (((BoundUserInterface)this).State is RMCConstructionBuiState)
		{
			RefreshStackAmount();
		}
	}

	private void AddEntry(ProtoId<RMCConstructionPrototype> prototypeId)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Expected O, but got Unknown
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Expected O, but got Unknown
		RMCConstructionPrototype build = default(RMCConstructionPrototype);
		if (!_prototype.TryIndex<RMCConstructionPrototype>(prototypeId, ref build))
		{
			return;
		}
		if (!string.IsNullOrEmpty(build.SideId))
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (!localEntity.HasValue)
			{
				return;
			}
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (!base.EntMan.TryGetComponent<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) || civTeamMemberComponent.SideId != build.SideId)
			{
				return;
			}
		}
		if (build.IsDivider)
		{
			BlueHorizontalSeparator blueHorizontalSeparator = new BlueHorizontalSeparator();
			((Control)blueHorizontalSeparator).Margin = new Thickness(5f);
			RMCConstructionWindow? window = _window;
			if (window != null)
			{
				((Control)window.ConstructionContainer).AddChild((Control)(object)blueHorizontalSeparator);
			}
			return;
		}
		if (build.Listed != null)
		{
			AddListButton(build);
			return;
		}
		string name = Loc.GetString("rmc-construction-list", new(string, object)[1] { ("name", build.Name) });
		if (build.MaterialCost.HasValue)
		{
			name = Loc.GetString("rmc-construction-entry", new(string, object)[3]
			{
				("name", build.Name),
				("amount", build.MaterialCost),
				("material", ((BoundUserInterface)this).Owner)
			});
		}
		RMCBuildChoiceControl rMCBuildChoiceControl = new RMCBuildChoiceControl();
		rMCBuildChoiceControl.Set(name);
		SpriteSpecifier icon = build.Icon;
		rMCBuildChoiceControl.SetIcon((icon != null) ? _sprite.Frame0(icon) : null);
		int valueOrDefault2 = build.MaterialCost.GetValueOrDefault();
		double num = Math.Max(build.DoAfterTime.TotalSeconds, build.DoAfterTimeMin.TotalSeconds);
		int num2 = ((num > 0.0) ? Math.Max(1, (int)Math.Ceiling(num)) : 0);
		if (_materialIcon == null)
		{
			_materialIcon = _sprite.Frame0((SpriteSpecifier)new Rsi(new ResPath("_PUBG/Interface/Construction/build_menu.rsi"), "craft_material"));
		}
		if (_hammerIcon == null)
		{
			_hammerIcon = _sprite.Frame0((SpriteSpecifier)new Rsi(new ResPath("_PUBG/Interface/Construction/build_menu.rsi"), "craft_hammer"));
		}
		rMCBuildChoiceControl.SetCosts((valueOrDefault2 > 0) ? _materialIcon : null, valueOrDefault2, (num2 > 0) ? _hammerIcon : null, num2);
		HashSet<int> stackAmounts = build.StackAmounts;
		if (stackAmounts != null)
		{
			foreach (int stack in stackAmounts)
			{
				Button val = new Button
				{
					Text = "x" + stack,
					StyleClasses = { "OpenBoth" },
					SetWidth = 45f,
					Margin = new Thickness(0f, 0f, 0f, 3f),
					HorizontalAlignment = (HAlignment)3
				};
				((Control)rMCBuildChoiceControl.StackAmountContainer).AddChild((Control)(object)val);
				((BaseButton)val).OnPressed += delegate
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCConstructionBuiMsg(ProtoId<RMCConstructionPrototype>.op_Implicit(build), stack));
				};
				((Control)rMCBuildChoiceControl.Button).SetWidth = 250f;
				((Control)rMCBuildChoiceControl.Button).HorizontalAlignment = (HAlignment)1;
			}
		}
		((BaseButton)rMCBuildChoiceControl.Button).OnPressed += delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCConstructionBuiMsg(ProtoId<RMCConstructionPrototype>.op_Implicit(build), build.Amount));
		};
		RMCConstructionWindow? window2 = _window;
		if (window2 != null)
		{
			((Control)window2.ConstructionContainer).AddChild((Control)(object)rMCBuildChoiceControl);
		}
	}

	private void AddListButton(RMCConstructionPrototype build)
	{
		ProtoId<RMCConstructionPrototype>[] listed = build.Listed;
		if (listed == null)
		{
			return;
		}
		RMCBuildChoiceControl rMCBuildChoiceControl = new RMCBuildChoiceControl();
		rMCBuildChoiceControl.Set(build.Name);
		((BaseButton)rMCBuildChoiceControl.Button).OnPressed += delegate
		{
			RMCConstructionWindow? window2 = _window;
			if (window2 != null)
			{
				((Control)window2.ConstructionContainer).Children.Clear();
			}
			Refresh(listed);
		};
		RMCConstructionWindow? window = _window;
		if (window != null)
		{
			((Control)window.ConstructionContainer).AddChild((Control)(object)rMCBuildChoiceControl);
		}
	}

	public void Refresh(ProtoId<RMCConstructionPrototype>[] entries)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			RefreshStackAmount();
			foreach (ProtoId<RMCConstructionPrototype> prototypeId in entries)
			{
				AddEntry(prototypeId);
			}
		}
	}

	public void RefreshStackAmount()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		StackComponent stackComponent = default(StackComponent);
		if (_window != null && base.EntMan.TryGetComponent<StackComponent>(((BoundUserInterface)this).Owner, ref stackComponent))
		{
			_window.MaterialLabel.Text = $"Amount Left: {stackComponent.Count}";
		}
	}
}
