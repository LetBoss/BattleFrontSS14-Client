using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Message;
using Content.Shared.Salvage;
using Content.Shared.Salvage.Magnet;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Salvage.UI;

public sealed class SalvageMagnetBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IEntityManager _entManager;

	private OfferingWindow? _window;

	public SalvageMagnetBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SalvageMagnetBoundUserInterface>(this);
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindowCenteredLeft<OfferingWindow>((BoundUserInterface)(object)this);
		_window.Title = Loc.GetString("salvage-magnet-window-title");
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected O, but got Unknown
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Expected O, but got Unknown
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is SalvageMagnetBoundUserInterfaceState salvageMagnetBoundUserInterfaceState) || _window == null)
		{
			return;
		}
		_window.ClearOptions();
		SharedSalvageSystem sharedSalvageSystem = _entManager.System<SharedSalvageSystem>();
		_window.NextOffer = salvageMagnetBoundUserInterfaceState.NextOffer;
		_window.Progression = salvageMagnetBoundUserInterfaceState.EndTime ?? TimeSpan.Zero;
		_window.Claimed = salvageMagnetBoundUserInterfaceState.EndTime.HasValue;
		_window.Cooldown = salvageMagnetBoundUserInterfaceState.Cooldown;
		_window.ProgressionCooldown = salvageMagnetBoundUserInterfaceState.Duration;
		for (int i = 0; i < salvageMagnetBoundUserInterfaceState.Offers.Count; i++)
		{
			int num = salvageMagnetBoundUserInterfaceState.Offers[i];
			ISalvageMagnetOffering salvageOffering = sharedSalvageSystem.GetSalvageOffering(num);
			OfferingWindowOption offeringWindowOption = new OfferingWindowOption();
			((Control)offeringWindowOption).MinWidth = 210f;
			offeringWindowOption.Disabled = salvageMagnetBoundUserInterfaceState.EndTime.HasValue;
			offeringWindowOption.Claimed = salvageMagnetBoundUserInterfaceState.ActiveSeed == num;
			int claimIndex = i;
			offeringWindowOption.ClaimPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MagnetClaimOfferEvent
				{
					Index = claimIndex
				});
			};
			if (!(salvageOffering is AsteroidOffering asteroidOffering))
			{
				if (!(salvageOffering is DebrisOffering debrisOffering))
				{
					if (!(salvageOffering is SalvageOffering salvageOffering2))
					{
						throw new ArgumentOutOfRangeException();
					}
					offeringWindowOption.Title = Loc.GetString("salvage-map-wreck");
					BoxContainer val = new BoxContainer
					{
						Orientation = (LayoutOrientation)0,
						HorizontalExpand = true
					};
					Label val2 = new Label
					{
						Text = Loc.GetString("salvage-map-wreck-desc-size"),
						HorizontalAlignment = (HAlignment)1
					};
					RichTextLabel val3 = new RichTextLabel
					{
						HorizontalAlignment = (HAlignment)3,
						HorizontalExpand = true
					};
					val3.SetMarkup(Loc.GetString(LocId.op_Implicit(salvageOffering2.SalvageMap.SizeString)));
					((Control)val).AddChild((Control)(object)val2);
					((Control)val).AddChild((Control)(object)val3);
					offeringWindowOption.AddContent((Control)(object)val);
				}
				else
				{
					offeringWindowOption.Title = Loc.GetString("salvage-magnet-debris-" + debrisOffering.Id);
				}
			}
			else
			{
				offeringWindowOption.Title = Loc.GetString("dungeon-config-proto-" + asteroidOffering.Id);
				List<string> list = asteroidOffering.MarkerLayers.Keys.ToList();
				list.Sort();
				foreach (string item in list)
				{
					int num2 = asteroidOffering.MarkerLayers[item];
					BoxContainer val4 = new BoxContainer
					{
						Orientation = (LayoutOrientation)0,
						HorizontalExpand = true
					};
					Label val5 = new Label();
					val5.Text = Loc.GetString("salvage-magnet-resources", new(string, object)[1] { ("resource", item) });
					((Control)val5).HorizontalAlignment = (HAlignment)1;
					Label val6 = val5;
					val5 = new Label();
					val5.Text = Loc.GetString("salvage-magnet-resources-count", new(string, object)[1] { ("count", num2) });
					((Control)val5).HorizontalAlignment = (HAlignment)3;
					((Control)val5).HorizontalExpand = true;
					Label val7 = val5;
					((Control)val4).AddChild((Control)(object)val6);
					((Control)val4).AddChild((Control)(object)val7);
					offeringWindowOption.AddContent((Control)(object)val4);
				}
			}
			_window.AddOption(offeringWindowOption);
		}
	}
}
