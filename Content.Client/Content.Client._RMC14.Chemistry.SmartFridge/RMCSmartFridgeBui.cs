using System;
using System.Collections.Generic;
using Content.Client._RMC14.UserInterface;
using Content.Shared._RMC14.Chemistry.SmartFridge;
using Content.Shared._RMC14.UserInterface;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Chemistry.SmartFridge;

public sealed class RMCSmartFridgeBui : BoundUserInterface, IRefreshableBui
{
	[Dependency]
	private ILocalizationManager _loc;

	private readonly ContainerSystem _container;

	private readonly EntityQuery<RMCSmartFridgeInsertableComponent> _insertableQuery;

	private readonly EntityQuery<MetaDataComponent> _metaDataQuery;

	private RMCSmartFridgeWindow? _window;

	private readonly SortedDictionary<string, SortedDictionary<string, int>> _contents = new SortedDictionary<string, SortedDictionary<string, int>>();

	private readonly Dictionary<string, EntityUid> _first = new Dictionary<string, EntityUid>();

	public RMCSmartFridgeBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_container = base.EntMan.System<ContainerSystem>();
		_insertableQuery = base.EntMan.GetEntityQuery<RMCSmartFridgeInsertableComponent>();
		_metaDataQuery = base.EntMan.GetEntityQuery<MetaDataComponent>();
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCSmartFridgeWindow>((BoundUserInterface)(object)this);
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.TryGetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner, ref val))
		{
			((DefaultWindow)_window).Title = val.EntityName;
		}
		Refresh();
	}

	public void Refresh()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Expected O, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Expected O, but got Unknown
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Expected O, but got Unknown
		if (_window == null)
		{
			return;
		}
		TabContainer contentsTabs = _window.ContentsTabs;
		RMCSmartFridgeComponent rMCSmartFridgeComponent = default(RMCSmartFridgeComponent);
		BaseContainer val = default(BaseContainer);
		if (!base.EntMan.TryGetComponent<RMCSmartFridgeComponent>(((BoundUserInterface)this).Owner, ref rMCSmartFridgeComponent) || !((SharedContainerSystem)_container).TryGetContainer(((BoundUserInterface)this).Owner, rMCSmartFridgeComponent.ContainerId, ref val, (ContainerManagerComponent)null) || val.ContainedEntities.Count == 0)
		{
			((Control)contentsTabs).RemoveAllChildren();
			((Control)_window.ContentsEmptyLabel).Visible = true;
			((Control)contentsTabs).Visible = false;
			return;
		}
		((Control)_window.ContentsEmptyLabel).Visible = false;
		((Control)contentsTabs).Visible = true;
		foreach (SortedDictionary<string, int> value5 in _contents.Values)
		{
			value5.Clear();
		}
		_first.Clear();
		RMCSmartFridgeInsertableComponent rMCSmartFridgeInsertableComponent = default(RMCSmartFridgeInsertableComponent);
		MetaDataComponent val2 = default(MetaDataComponent);
		string text2 = default(string);
		foreach (EntityUid containedEntity in val.ContainedEntities)
		{
			if (_insertableQuery.TryComp(containedEntity, ref rMCSmartFridgeInsertableComponent) && _metaDataQuery.TryComp(containedEntity, ref val2))
			{
				string text = rMCSmartFridgeInsertableComponent.Category;
				if (_loc.TryGetString(rMCSmartFridgeInsertableComponent.Category, ref text2))
				{
					text = text2;
				}
				string entityName = val2.EntityName;
				SortedDictionary<string, int> orNew = Extensions.GetOrNew<string, SortedDictionary<string, int>>((IDictionary<string, SortedDictionary<string, int>>)_contents, text);
				orNew[entityName] = orNew.GetValueOrDefault(entityName) + 1;
				_first.TryAdd(entityName, containedEntity);
			}
		}
		int num = 0;
		MetaDataComponent val3 = default(MetaDataComponent);
		foreach (KeyValuePair<string, SortedDictionary<string, int>> content in _contents)
		{
			content.Deconstruct(out var key, out var value);
			string text3 = key;
			SortedDictionary<string, int> sortedDictionary = value;
			if (sortedDictionary.Count == 0)
			{
				continue;
			}
			RMCSmartFridgeSection rMCSmartFridgeSection;
			if (num < ((Control)contentsTabs).ChildCount)
			{
				rMCSmartFridgeSection = (RMCSmartFridgeSection)(object)((Control)contentsTabs).GetChild(num);
			}
			else
			{
				rMCSmartFridgeSection = new RMCSmartFridgeSection();
				((Control)contentsTabs).AddChild((Control)(object)rMCSmartFridgeSection);
			}
			TabContainer.SetTabTitle((Control)(object)rMCSmartFridgeSection, text3);
			TabContainer.SetTabVisible((Control)(object)rMCSmartFridgeSection, true);
			int num2 = 0;
			foreach (KeyValuePair<string, int> item in sortedDictionary)
			{
				item.Deconstruct(out key, out var value2);
				string text4 = key;
				int value3 = value2;
				if (!_first.TryGetValue(text4, out var value4))
				{
					num2++;
					continue;
				}
				NetEntity netFirst = base.EntMan.GetNetEntity(value4, (MetaDataComponent)null);
				RMCSmartFridgeRow rMCSmartFridgeRow;
				if (num2 < ((Control)rMCSmartFridgeSection.Container).ChildCount)
				{
					rMCSmartFridgeRow = (RMCSmartFridgeRow)(object)((Control)rMCSmartFridgeSection.Container).GetChild(num2);
				}
				else
				{
					rMCSmartFridgeRow = new RMCSmartFridgeRow();
					((Control)rMCSmartFridgeSection.Container).AddChild((Control)(object)rMCSmartFridgeRow);
				}
				rMCSmartFridgeRow.SpriteView.SetEntity((EntityUid?)value4);
				rMCSmartFridgeRow.AmountLabel.Text = $"{value3}";
				rMCSmartFridgeRow.NameButton.Text = text4;
				rMCSmartFridgeRow.NameButton.ClearOnPressed();
				rMCSmartFridgeRow.NameButton.OnPressed += delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCSmartFridgeVendMsg(netFirst));
				};
				if (base.EntMan.TryGetComponent<MetaDataComponent>(value4, ref val3))
				{
					((Control)rMCSmartFridgeRow.TooltipLabel).Visible = true;
					FormattedMessage val4 = new FormattedMessage();
					val4.AddText(text4);
					val4.PushNewline();
					if (!string.IsNullOrWhiteSpace(val3.EntityDescription))
					{
						val4.AddText(val3.EntityDescription);
					}
					Tooltip tooltip = new Tooltip();
					tooltip.SetMessage(val4);
					((Control)rMCSmartFridgeRow.TooltipLabel).ToolTip = val3.EntityDescription;
					((Control)rMCSmartFridgeRow.TooltipLabel).TooltipDelay = 0f;
					((Control)rMCSmartFridgeRow.TooltipLabel).TooltipSupplier = (TooltipSupplier)((Control _) => (Control?)(object)tooltip);
				}
				else
				{
					((Control)rMCSmartFridgeRow.TooltipLabel).Visible = false;
				}
				num2++;
			}
			((Control)(object)rMCSmartFridgeSection.Container).RemoveChildrenAfter(num2);
			num++;
		}
		((Control)(object)contentsTabs).SetTabVisibleAfter(num, visible: false);
		((Control)(object)contentsTabs).SetVisibleAfter(num, visible: false);
		int currentTab = contentsTabs.CurrentTab;
		if (currentTab < ((Control)contentsTabs).ChildCount && !((Control)contentsTabs).GetChild(currentTab).Visible)
		{
			contentsTabs.CurrentTab = 0;
		}
	}
}
