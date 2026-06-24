using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Holiday;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Medical.Refill;
using Content.Shared._RMC14.Vendors;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Vendors;

public sealed class CMAutomatedVendorBui : BoundUserInterface, ICMAutomatedVendorRefreshable
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IResourceCache _resource;

	private readonly SharedJobSystem _job;

	private readonly SharedMindSystem _mind;

	private readonly SharedRankSystem _rank;

	private readonly SharedRMCHolidaySystem _rmcHoliday;

	private readonly SpriteSystem _sprite;

	private CMAutomatedVendorWindow? _window;

	public CMAutomatedVendorBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_job = base.EntMan.System<SharedJobSystem>();
		_mind = base.EntMan.System<SharedMindSystem>();
		_rmcHoliday = base.EntMan.System<SharedRMCHolidaySystem>();
		_rank = base.EntMan.System<SharedRankSystem>();
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<CMAutomatedVendorWindow>((BoundUserInterface)(object)this);
		CMAutomatedVendorWindow? window = _window;
		MetaDataComponent componentOrNull = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(base.EntMan, ((BoundUserInterface)this).Owner);
		((DefaultWindow)window).Title = ((componentOrNull != null) ? componentOrNull.EntityName : null) ?? "ColMarTech Vendor";
		_window.ReagentsBar.ForegroundStyleBoxOverride = (StyleBox)new StyleBoxFlat(Color.FromHex((ReadOnlySpan<char>)"#AF7F38", (Color?)null));
		CMAutomatedVendorComponent vendor = default(CMAutomatedVendorComponent);
		if (base.EntMan.TryGetComponent<CMAutomatedVendorComponent>(((BoundUserInterface)this).Owner, ref vendor))
		{
			RebuildSections(vendor, EntityManagerExt.GetComponentOrNull<CMVendorUserComponent>(base.EntMan, ((ISharedPlayerManager)_player).LocalEntity));
		}
		_window.Search.OnTextChanged += OnSearchChanged;
		Refresh();
	}

	private bool NeedsSectionRebuild(CMAutomatedVendorComponent vendor)
	{
		if (_window == null)
		{
			return false;
		}
		if (((Control)_window.Sections).ChildCount != vendor.Sections.Count)
		{
			return true;
		}
		for (int i = 0; i < vendor.Sections.Count; i++)
		{
			if (!(((Control)_window.Sections).GetChild(i) is CMAutomatedVendorSection cMAutomatedVendorSection) || ((Control)cMAutomatedVendorSection.Entries).ChildCount != vendor.Sections[i].Entries.Count)
			{
				return true;
			}
		}
		return false;
	}

	private void RebuildSections(CMAutomatedVendorComponent vendor, CMVendorUserComponent? user)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Expected O, but got Unknown
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		((Control)_window.Sections).DisposeAllChildren();
		EntityPrototype val = default(EntityPrototype);
		SpriteComponent val2 = default(SpriteComponent);
		for (int i = 0; i < vendor.Sections.Count; i++)
		{
			CMVendorSection cMVendorSection = vendor.Sections[i];
			CMAutomatedVendorSection cMAutomatedVendorSection = new CMAutomatedVendorSection
			{
				Section = cMVendorSection
			};
			cMAutomatedVendorSection.Label.SetMessage(GetSectionName(user, cMVendorSection), (Color?)null);
			((Control)cMAutomatedVendorSection).Visible = IsSectionValid(cMVendorSection);
			for (int j = 0; j < cMVendorSection.Entries.Count; j++)
			{
				CMVendorEntry cMVendorEntry = cMVendorSection.Entries[j];
				CMAutomatedVendorEntry cMAutomatedVendorEntry = new CMAutomatedVendorEntry();
				if (_prototype.TryIndex(cMVendorEntry.Id, ref val))
				{
					cMAutomatedVendorEntry.Texture.Textures = (from o in _sprite.GetPrototypeTextures(val)
						select o.Default).ToList();
					if (val.TryGetComponent<SpriteComponent>("Sprite", ref val2))
					{
						((Control)cMAutomatedVendorEntry.Texture).Modulate = val2.AllLayers.First().Color;
					}
					((Button)cMAutomatedVendorEntry.Panel.Button).Label.Text = cMVendorEntry.Name?.Replace("\\n", "\n") ?? val.Name;
					string text = val.Name;
					Color color = CMAutomatedVendorPanel.DefaultColor;
					Color borderColor = CMAutomatedVendorPanel.DefaultBorderColor;
					Color hoveredColor = CMAutomatedVendorPanel.DefaultBorderColor;
					if (cMVendorSection.TakeAll != null || cMVendorSection.TakeOne != null)
					{
						text = "Mandatory: " + text;
						color = Color.FromHex((ReadOnlySpan<char>)"#251A0C", (Color?)null);
						borderColor = Color.FromHex((ReadOnlySpan<char>)"#805300", (Color?)null);
						hoveredColor = Color.FromHex((ReadOnlySpan<char>)"#805300", (Color?)null);
					}
					else if (cMVendorEntry.Recommended)
					{
						((Button)cMAutomatedVendorEntry.Panel.Button).Label.Text = "★ " + ((Button)cMAutomatedVendorEntry.Panel.Button).Label.Text;
						text = "Recommended: " + text;
						color = Color.FromHex((ReadOnlySpan<char>)"#102919", (Color?)null);
						borderColor = Color.FromHex((ReadOnlySpan<char>)"#3A9B52", (Color?)null);
						hoveredColor = Color.FromHex((ReadOnlySpan<char>)"#3A9B52", (Color?)null);
					}
					cMAutomatedVendorEntry.Panel.Color = color;
					cMAutomatedVendorEntry.Panel.BorderColor = borderColor;
					cMAutomatedVendorEntry.Panel.HoveredColor = hoveredColor;
					FormattedMessage val3 = new FormattedMessage();
					val3.AddText(text);
					val3.PushNewline();
					if (!string.IsNullOrWhiteSpace(val.Description))
					{
						val3.AddText(val.Description);
					}
					Tooltip tooltip = new Tooltip();
					tooltip.SetMessage(val3);
					((Control)cMAutomatedVendorEntry.TooltipLabel).ToolTip = val.Description;
					((Control)cMAutomatedVendorEntry.TooltipLabel).TooltipDelay = 0f;
					((Control)cMAutomatedVendorEntry.TooltipLabel).TooltipSupplier = (TooltipSupplier)((Control _) => (Control?)(object)tooltip);
					int sectionI = i;
					int entryI = j;
					List<int> linkedEntryIndexes = new List<int>();
					foreach (EntProtoId linkedEntry in cMVendorEntry.LinkedEntries)
					{
						int num = 0;
						foreach (CMVendorEntry entry in cMVendorSection.Entries)
						{
							if (entry.Id == linkedEntry)
							{
								linkedEntryIndexes.Add(num);
							}
							num++;
						}
					}
					((BaseButton)cMAutomatedVendorEntry.Panel.Button).OnPressed += delegate
					{
						OnButtonPressed(sectionI, entryI, linkedEntryIndexes);
					};
				}
				((Control)cMAutomatedVendorSection.Entries).AddChild((Control)(object)cMAutomatedVendorEntry);
			}
			((Control)_window.Sections).AddChild((Control)(object)cMAutomatedVendorSection);
		}
	}

	private bool IsSectionValid(CMVendorSection section)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		bool flag2 = true;
		if (((ISharedPlayerManager)_player).LocalSession != null && _mind.TryGetMind(((ISharedPlayerManager)_player).LocalSession.UserId, out Entity<MindComponent>? mind))
		{
			foreach (ProtoId<JobPrototype> job2 in section.Jobs)
			{
				SharedJobSystem job = _job;
				Entity<MindComponent>? val = mind;
				if (!job.MindHasJobWithId(val.HasValue ? new EntityUid?(Entity<MindComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null), job2.Id))
				{
					flag = false;
					continue;
				}
				flag = true;
				break;
			}
			if (((ISharedPlayerManager)_player).LocalEntity.HasValue)
			{
				foreach (ProtoId<RankPrototype> rank2 in section.Ranks)
				{
					RankPrototype rank = _rank.GetRank(((ISharedPlayerManager)_player).LocalEntity.Value);
					if (rank == null || ProtoId<RankPrototype>.op_Implicit(rank) != rank2)
					{
						flag2 = false;
						continue;
					}
					flag2 = true;
					break;
				}
			}
		}
		bool flag3 = section.Holidays.Count == 0;
		foreach (string holiday in section.Holidays)
		{
			if (_rmcHoliday.IsActiveHoliday(holiday))
			{
				flag3 = true;
			}
		}
		return flag && flag3 && flag2;
	}

	private void OnButtonPressed(int sectionIndex, int entryIndex, List<int> linkedEntryIndexes)
	{
		CMVendorVendBuiMsg cMVendorVendBuiMsg = new CMVendorVendBuiMsg(sectionIndex, entryIndex, linkedEntryIndexes);
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)cMVendorVendBuiMsg);
	}

	private void OnSearchChanged(LineEditEventArgs args)
	{
		ApplySearchFilter(args.Text);
	}

	private unsafe void ApplySearchFilter(string? text)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		Enumerator enumerator = ((Control)_window.Sections).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (!(((Enumerator)(ref enumerator)).Current is CMAutomatedVendorSection cMAutomatedVendorSection))
				{
					continue;
				}
				bool flag = false;
				Enumerator enumerator2 = ((Control)cMAutomatedVendorSection.Entries).Children.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						if (((Enumerator)(ref enumerator2)).Current is CMAutomatedVendorEntry cMAutomatedVendorEntry)
						{
							if (string.IsNullOrWhiteSpace(text))
							{
								((Control)cMAutomatedVendorEntry).Visible = true;
							}
							else
							{
								((Control)cMAutomatedVendorEntry).Visible = ((Button)cMAutomatedVendorEntry.Panel.Button).Label.Text?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false;
							}
							if (((Control)cMAutomatedVendorEntry).Visible)
							{
								flag = true;
							}
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				((Control)cMAutomatedVendorSection).Visible = flag && (cMAutomatedVendorSection.Section == null || IsSectionValid(cMAutomatedVendorSection.Section));
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void Refresh()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		CMAutomatedVendorComponent cMAutomatedVendorComponent = default(CMAutomatedVendorComponent);
		if (_window == null || !base.EntMan.TryGetComponent<CMAutomatedVendorComponent>(((BoundUserInterface)this).Owner, ref cMAutomatedVendorComponent))
		{
			return;
		}
		bool flag = false;
		CMVendorUserComponent componentOrNull = EntityManagerExt.GetComponentOrNull<CMVendorUserComponent>(base.EntMan, ((ISharedPlayerManager)_player).LocalEntity);
		int num = ((cMAutomatedVendorComponent.PointsType != null) ? (componentOrNull?.ExtraPoints?.GetValueOrDefault(cMAutomatedVendorComponent.PointsType)).GetValueOrDefault() : (componentOrNull?.Points ?? 0));
		if (NeedsSectionRebuild(cMAutomatedVendorComponent))
		{
			RebuildSections(cMAutomatedVendorComponent, componentOrNull);
		}
		for (int i = 0; i < cMAutomatedVendorComponent.Sections.Count; i++)
		{
			CMVendorSection cMVendorSection = cMAutomatedVendorComponent.Sections[i];
			CMAutomatedVendorSection cMAutomatedVendorSection = (CMAutomatedVendorSection)(object)((Control)_window.Sections).GetChild(i);
			cMAutomatedVendorSection.Label.SetMessage(GetSectionName(componentOrNull, cMVendorSection), (Color?)null);
			bool flag2 = false;
			(string, int)? choices = cMVendorSection.Choices;
			if (choices.HasValue)
			{
				(string, int) valueOrDefault = choices.GetValueOrDefault();
				if (componentOrNull?.Choices.GetValueOrDefault(valueOrDefault.Item1) >= valueOrDefault.Item2 || (componentOrNull == null && valueOrDefault.Item2 <= 0))
				{
					flag2 = true;
				}
			}
			bool visible = false;
			for (int j = 0; j < cMVendorSection.Entries.Count; j++)
			{
				CMVendorEntry cMVendorEntry = cMVendorSection.Entries[j];
				CMAutomatedVendorEntry cMAutomatedVendorEntry = (CMAutomatedVendorEntry)(object)((Control)cMAutomatedVendorSection.Entries).GetChild(j);
				bool flag3 = flag2 || cMVendorEntry.Amount <= 0;
				string takeAll = cMVendorSection.TakeAll;
				if (takeAll != null)
				{
					HashSet<(string, EntProtoId)> hashSet = componentOrNull?.TakeAll;
					if (hashSet != null && hashSet.Contains((takeAll, cMVendorEntry.Id)))
					{
						flag3 = true;
					}
				}
				string takeOne = cMVendorSection.TakeOne;
				if (takeOne != null)
				{
					HashSet<string> hashSet2 = componentOrNull?.TakeOne;
					if (hashSet2 != null && hashSet2.Contains(takeOne))
					{
						flag3 = true;
					}
				}
				if (cMVendorEntry.Points.HasValue)
				{
					flag = true;
					cMAutomatedVendorEntry.Amount.Text = $"{cMVendorEntry.Points}P";
					if (componentOrNull == null || num < cMVendorEntry.Points)
					{
						flag3 = true;
					}
				}
				else
				{
					cMAutomatedVendorEntry.Amount.Text = cMVendorEntry.Amount.ToString();
				}
				((Control)cMAutomatedVendorEntry.Amount).Modulate = (flag3 ? Color.Red : Color.White);
				((BaseButton)cMAutomatedVendorEntry.Panel.Button).Disabled = flag3;
				if (!string.IsNullOrWhiteSpace(cMAutomatedVendorEntry.Amount.Text))
				{
					visible = true;
				}
			}
			for (int k = 0; k < cMVendorSection.Entries.Count; k++)
			{
				((Control)((CMAutomatedVendorEntry)(object)((Control)cMAutomatedVendorSection.Entries).GetChild(k)).Amount).Visible = visible;
			}
		}
		ApplySearchFilter(_window.Search.Text);
		_window.PointsLabel.Text = (flag ? $"Points Remaining: {num}" : string.Empty);
		CMSolutionRefillerComponent cMSolutionRefillerComponent = default(CMSolutionRefillerComponent);
		if (!base.EntMan.TryGetComponent<CMSolutionRefillerComponent>(((BoundUserInterface)this).Owner, ref cMSolutionRefillerComponent))
		{
			((Control)_window.ReagentsContainer).Visible = false;
			return;
		}
		((Control)_window.ReagentsContainer).Visible = true;
		FixedPoint2 current = cMSolutionRefillerComponent.Current;
		FixedPoint2 max = cMSolutionRefillerComponent.Max;
		((Range)_window.ReagentsBar).MinValue = 0f;
		((Range)_window.ReagentsBar).MaxValue = max.Int();
		((Range)_window.ReagentsBar).SetAsRatio((cMSolutionRefillerComponent.Current / cMSolutionRefillerComponent.Max).Float());
		_window.ReagentsLabel.Text = $"{current.Int()} units";
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (message is CMVendorRefreshBuiMsg)
		{
			Refresh();
		}
	}

	private FormattedMessage GetSectionName(CMVendorUserComponent? user, CMVendorSection section)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage val = new FormattedMessage();
		val.PushTag(new MarkupNode("bold", (MarkupParameter?)new MarkupParameter(section.Name.ToUpperInvariant()), (Dictionary<string, MarkupParameter>)null, false), false);
		val.AddText(section.Name.ToUpperInvariant());
		if (section.TakeAll != null)
		{
			HashSet<(string, EntProtoId)> hashSet = user?.TakeAll;
			foreach (CMVendorEntry entry in section.Entries)
			{
				if (hashSet == null || !hashSet.Contains((section.TakeAll, entry.Id)))
				{
					val.AddText(" (TAKE ALL)");
					break;
				}
			}
		}
		else if (section.TakeOne != null)
		{
			HashSet<string> hashSet2 = user?.TakeOne;
			if (hashSet2 == null || !hashSet2.Contains(section.TakeOne))
			{
				val.AddText(" (TAKE ONE)");
			}
		}
		else
		{
			(string, int)? choices = section.Choices;
			if (choices.HasValue)
			{
				(string, int) valueOrDefault = choices.GetValueOrDefault();
				if (user == null)
				{
					val.AddText($" (CHOOSE {valueOrDefault.Item2})");
				}
				else
				{
					int num = valueOrDefault.Item2 - user.Choices.GetValueOrDefault(valueOrDefault.Item1);
					if (num > 0)
					{
						val.AddText($" (CHOOSE {num})");
					}
				}
			}
		}
		val.Pop();
		return val;
	}
}
