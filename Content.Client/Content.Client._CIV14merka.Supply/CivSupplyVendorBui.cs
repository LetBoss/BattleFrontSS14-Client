using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._RMC14.Vendors;
using Content.Shared._RMC14.Holiday;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Vendors;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
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

namespace Content.Client._CIV14merka.Supply;

public sealed class CivSupplyVendorBui : BoundUserInterface, ICMAutomatedVendorRefreshable
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototype;

	private readonly SharedJobSystem _job;

	private readonly SharedMindSystem _mind;

	private readonly SharedRankSystem _rank;

	private readonly SharedRMCHolidaySystem _holiday;

	private readonly SpriteSystem _sprite;

	private CivSupplyVendorWindow? _window;

	public CivSupplyVendorBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_job = base.EntMan.System<SharedJobSystem>();
		_mind = base.EntMan.System<SharedMindSystem>();
		_rank = base.EntMan.System<SharedRankSystem>();
		_holiday = base.EntMan.System<SharedRMCHolidaySystem>();
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<CivSupplyVendorWindow>((BoundUserInterface)(object)this);
		CivSupplyVendorWindow? window = _window;
		MetaDataComponent componentOrNull = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(base.EntMan, ((BoundUserInterface)this).Owner);
		((DefaultWindow)window).Title = ((componentOrNull != null) ? componentOrNull.EntityName : null) ?? Loc.GetString("civ-eq-supply-vendor-default-title");
		CMAutomatedVendorComponent vendor = default(CMAutomatedVendorComponent);
		if (base.EntMan.TryGetComponent<CMAutomatedVendorComponent>(((BoundUserInterface)this).Owner, ref vendor))
		{
			BuildSections(vendor);
		}
		_window.Search.OnTextChanged += OnSearchChanged;
		Refresh();
	}

	private void BuildSections(CMAutomatedVendorComponent vendor)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Expected O, but got Unknown
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Expected O, but got Unknown
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		EntityPrototype val = default(EntityPrototype);
		SpriteComponent val2 = default(SpriteComponent);
		for (int i = 0; i < vendor.Sections.Count; i++)
		{
			CMVendorSection cMVendorSection = vendor.Sections[i];
			CMAutomatedVendorSection cMAutomatedVendorSection = new CMAutomatedVendorSection
			{
				Section = cMVendorSection
			};
			cMAutomatedVendorSection.Label.SetMessage(GetSectionName(cMVendorSection), (Color?)null);
			if (!IsSectionValid(cMVendorSection))
			{
				((Control)cMAutomatedVendorSection).Visible = false;
			}
			for (int j = 0; j < cMVendorSection.Entries.Count; j++)
			{
				CMVendorEntry cMVendorEntry = cMVendorSection.Entries[j];
				CMAutomatedVendorEntry cMAutomatedVendorEntry = new CMAutomatedVendorEntry();
				if (_prototype.TryIndex(cMVendorEntry.Id, ref val))
				{
					cMAutomatedVendorEntry.Texture.Textures = (from texture in _sprite.GetPrototypeTextures(val)
						select texture.Default).ToList();
					if (val.TryGetComponent<SpriteComponent>("Sprite", ref val2))
					{
						((Control)cMAutomatedVendorEntry.Texture).Modulate = val2.AllLayers.First().Color;
					}
					cMAutomatedVendorEntry.Panel.Button.TextLabel.Text = cMVendorEntry.Name?.Replace("\\n", "\n") ?? val.Name;
					Color color = Color.FromHex((ReadOnlySpan<char>)"#18211E", (Color?)null);
					Color borderColor = Color.FromHex((ReadOnlySpan<char>)"#7D8F6E", (Color?)null);
					Color hoveredColor = Color.FromHex((ReadOnlySpan<char>)"#A5B78D", (Color?)null);
					string text = val.Name;
					if (cMVendorSection.TakeAll != null || cMVendorSection.TakeOne != null)
					{
						text = Loc.GetString("civ-eq-supply-mandatory", new(string, object)[1] { ("title", text) });
						color = Color.FromHex((ReadOnlySpan<char>)"#2A2112", (Color?)null);
						borderColor = Color.FromHex((ReadOnlySpan<char>)"#A8863E", (Color?)null);
						hoveredColor = Color.FromHex((ReadOnlySpan<char>)"#C79F48", (Color?)null);
					}
					else if (cMVendorEntry.Recommended)
					{
						cMAutomatedVendorEntry.Panel.Button.TextLabel.Text = "* " + cMAutomatedVendorEntry.Panel.Button.TextLabel.Text;
						text = Loc.GetString("civ-eq-supply-recommended", new(string, object)[1] { ("title", text) });
						color = Color.FromHex((ReadOnlySpan<char>)"#162623", (Color?)null);
						borderColor = Color.FromHex((ReadOnlySpan<char>)"#3E8C78", (Color?)null);
						hoveredColor = Color.FromHex((ReadOnlySpan<char>)"#59B39C", (Color?)null);
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
					((Control)cMAutomatedVendorEntry.TooltipLabel).TooltipDelay = 0f;
					((Control)cMAutomatedVendorEntry.TooltipLabel).TooltipSupplier = (TooltipSupplier)((Control _) => (Control?)(object)tooltip);
					int sectionI = i;
					int entryI = j;
					List<int> linkedEntryIndexes = new List<int>();
					int num = 0;
					foreach (CMVendorEntry entry in cMVendorSection.Entries)
					{
						if (cMVendorEntry.LinkedEntries.Contains(entry.Id))
						{
							linkedEntryIndexes.Add(num);
						}
						num++;
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
			if (_holiday.IsActiveHoliday(holiday))
			{
				flag3 = true;
			}
		}
		return flag && flag2 && flag3;
	}

	private void OnButtonPressed(int sectionIndex, int entryIndex, List<int> linkedEntryIndexes)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CMVendorVendBuiMsg(sectionIndex, entryIndex, linkedEntryIndexes));
	}

	private void OnSearchChanged(LineEditEventArgs args)
	{
		ApplySearchFilter(args.Text);
	}

	private unsafe void ApplySearchFilter(string text)
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
								((Control)cMAutomatedVendorEntry).Visible = cMAutomatedVendorEntry.Panel.Button.TextLabel.Text?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false;
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		CMAutomatedVendorComponent cMAutomatedVendorComponent = default(CMAutomatedVendorComponent);
		if (_window == null || !base.EntMan.TryGetComponent<CMAutomatedVendorComponent>(((BoundUserInterface)this).Owner, ref cMAutomatedVendorComponent))
		{
			return;
		}
		if (!StructureMatches(cMAutomatedVendorComponent))
		{
			((Control)_window.Sections).DisposeAllChildren();
			BuildSections(cMAutomatedVendorComponent);
			ApplySearchFilter(_window.Search.Text);
		}
		bool flag = false;
		CMVendorUserComponent componentOrNull = EntityManagerExt.GetComponentOrNull<CMVendorUserComponent>(base.EntMan, ((ISharedPlayerManager)_player).LocalEntity);
		int num = ((cMAutomatedVendorComponent.PointsType != null) ? (componentOrNull?.ExtraPoints?.GetValueOrDefault(cMAutomatedVendorComponent.PointsType)).GetValueOrDefault() : (componentOrNull?.Points ?? 0));
		for (int i = 0; i < cMAutomatedVendorComponent.Sections.Count; i++)
		{
			CMVendorSection cMVendorSection = cMAutomatedVendorComponent.Sections[i];
			CMAutomatedVendorSection cMAutomatedVendorSection = (CMAutomatedVendorSection)(object)((Control)_window.Sections).GetChild(i);
			cMAutomatedVendorSection.Label.SetMessage(GetSectionName(cMVendorSection), (Color?)null);
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
					cMAutomatedVendorEntry.Amount.Text = cMVendorEntry.Amount?.ToString() ?? string.Empty;
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
		_window.PointsLabel.Text = (flag ? Loc.GetString("civ-eq-supply-points", new(string, object)[1] { ("points", num) }) : string.Empty);
	}

	private bool StructureMatches(CMAutomatedVendorComponent vendor)
	{
		if (_window == null || ((Control)_window.Sections).ChildCount != vendor.Sections.Count)
		{
			return false;
		}
		for (int i = 0; i < vendor.Sections.Count; i++)
		{
			if (!(((Control)_window.Sections).GetChild(i) is CMAutomatedVendorSection cMAutomatedVendorSection) || ((Control)cMAutomatedVendorSection.Entries).ChildCount != vendor.Sections[i].Entries.Count)
			{
				return false;
			}
		}
		return true;
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (message is CMVendorRefreshBuiMsg)
		{
			Refresh();
		}
	}

	private static FormattedMessage GetSectionName(CMVendorSection section)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage val = new FormattedMessage();
		val.AddText(section.Name.ToUpperInvariant());
		if (section.TakeAll != null)
		{
			val.AddText(Loc.GetString("civ-eq-supply-section-take-all"));
		}
		else if (section.TakeOne != null)
		{
			val.AddText(Loc.GetString("civ-eq-supply-section-take-one"));
		}
		else
		{
			(string, int)? choices = section.Choices;
			if (choices.HasValue)
			{
				(string, int) valueOrDefault = choices.GetValueOrDefault();
				val.AddText(Loc.GetString("civ-eq-supply-section-choose", new(string, object)[1] { ("amount", valueOrDefault.Item2) }));
			}
		}
		return val;
	}
}
