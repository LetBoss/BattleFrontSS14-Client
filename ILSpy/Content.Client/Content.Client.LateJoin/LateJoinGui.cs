using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.CrewManifest;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.CCVar;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.LateJoin;

public sealed class LateJoinGui : DefaultWindow
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IClientConsoleHost _consoleHost;

	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	private IEntitySystemManager _entitySystem;

	[Dependency]
	private JobRequirementsManager _jobRequirements;

	[Dependency]
	private IClientPreferencesManager _preferencesManager;

	[Dependency]
	private ILogManager _logManager;

	private readonly ClientGameTicker _gameTicker;

	private readonly SpriteSystem _sprites;

	private readonly CrewManifestSystem _crewManifest;

	private readonly ISawmill _sawmill;

	private readonly Dictionary<NetEntity, Dictionary<string, List<JobButton>>> _jobButtons = new Dictionary<NetEntity, Dictionary<string, List<JobButton>>>();

	private readonly Dictionary<NetEntity, Dictionary<string, BoxContainer>> _jobCategories = new Dictionary<NetEntity, Dictionary<string, BoxContainer>>();

	private readonly List<ScrollContainer> _jobLists = new List<ScrollContainer>();

	private readonly Control _base;

	public event Action<(NetEntity, string)> SelectedId;

	public LateJoinGui()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		Vector2 minSize = (((Control)this).SetSize = new Vector2(360f, 560f));
		((Control)this).MinSize = minSize;
		IoCManager.InjectDependencies<LateJoinGui>(this);
		_sprites = _entitySystem.GetEntitySystem<SpriteSystem>();
		_crewManifest = _entitySystem.GetEntitySystem<CrewManifestSystem>();
		_gameTicker = _entitySystem.GetEntitySystem<ClientGameTicker>();
		_sawmill = _logManager.GetSawmill("latejoin.panel");
		((DefaultWindow)this).Title = Loc.GetString("late-join-gui-title");
		_base = (Control)new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			VerticalExpand = true
		};
		((DefaultWindow)this).Contents.AddChild(_base);
		_jobRequirements.Updated += RebuildUI;
		RebuildUI();
		SelectedId += delegate((NetEntity, string) x)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			var (value, text) = x;
			_sawmill.Info("Late joining as ID: " + text);
			((IConsoleHost)_consoleHost).ExecuteCommand($"joingame {CommandParsing.Escape(text)} {value}");
			((BaseWindow)this).Close();
		};
		_gameTicker.LobbyJobsAvailableUpdated += JobsAvailableUpdated;
	}

	private void RebuildUI()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_012f: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Expected O, but got Unknown
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Expected O, but got Unknown
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Expected O, but got Unknown
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected O, but got Unknown
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Expected O, but got Unknown
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Expected O, but got Unknown
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Expected O, but got Unknown
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Expected O, but got Unknown
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Expected O, but got Unknown
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Expected O, but got Unknown
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Expected O, but got Unknown
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		_base.RemoveAllChildren();
		_jobLists.Clear();
		_jobButtons.Clear();
		_jobCategories.Clear();
		if (!_gameTicker.DisallowedLateJoin && _gameTicker.StationNames.Count == 0)
		{
			_sawmill.Warning("No stations exist, nothing to display in late-join GUI");
		}
		foreach (KeyValuePair<NetEntity, string> stationName in _gameTicker.StationNames)
		{
			stationName.Deconstruct(out var key, out var value);
			NetEntity id = key;
			string text = value;
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				Margin = new Thickness(0f, 0f, 5f, 0f)
			};
			ContainerButton val2 = new ContainerButton
			{
				HorizontalAlignment = (HAlignment)3,
				ToggleMode = true
			};
			((Control)val2).Children.Add((Control)new TextureRect
			{
				StyleClasses = { "optionTriangle" },
				Margin = new Thickness(8f, 0f),
				HorizontalAlignment = (HAlignment)2,
				VerticalAlignment = (VAlignment)2
			});
			ContainerButton val3 = val2;
			Control obj = _base;
			StripeBack stripeBack = new StripeBack();
			OrderedChildCollection children = ((Control)stripeBack).Children;
			PanelContainer val4 = new PanelContainer();
			((Control)val4).Children.Add((Control)new Label
			{
				StyleClasses = { "LabelBig" },
				Text = text,
				Align = (AlignMode)1
			});
			((Control)val4).Children.Add((Control)(object)val3);
			children.Add((Control)val4);
			obj.AddChild((Control)(object)stripeBack);
			if (_configManager.GetCVar<bool>(CCVars.CrewManifestWithoutEntity))
			{
				Button val5 = new Button
				{
					Text = Loc.GetString("crew-manifest-button-label")
				};
				((BaseButton)val5).OnPressed += delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					_crewManifest.RequestCrewManifest(id);
				};
				_base.AddChild((Control)(object)val5);
			}
			ScrollContainer val6 = new ScrollContainer
			{
				VerticalExpand = true
			};
			((Control)val6).Children.Add((Control)(object)val);
			((Control)val6).Visible = false;
			ScrollContainer jobListScroll = val6;
			if (_jobLists.Count == 0)
			{
				((Control)jobListScroll).Visible = true;
			}
			_jobLists.Add(jobListScroll);
			_base.AddChild((Control)(object)jobListScroll);
			((BaseButton)val3).OnToggled += delegate
			{
				foreach (ScrollContainer jobList in _jobLists)
				{
					((Control)jobList).Visible = false;
				}
				((Control)jobListScroll).Visible = true;
			};
			bool flag = true;
			DepartmentPrototype[] array = _prototypeManager.EnumerateCM<DepartmentPrototype>().ToArray();
			Array.Sort(array, DepartmentUIComparer.Instance);
			_jobButtons[id] = new Dictionary<string, List<JobButton>>();
			DepartmentPrototype[] array2 = array;
			foreach (DepartmentPrototype departmentPrototype in array2)
			{
				string item = Loc.GetString(LocId.op_Implicit(departmentPrototype.Name));
				_jobCategories[id] = new Dictionary<string, BoxContainer>();
				Dictionary<ProtoId<JobPrototype>, int?> dictionary = _gameTicker.JobsAvailable[id];
				List<JobPrototype> list = new List<JobPrototype>();
				foreach (ProtoId<JobPrototype> role in departmentPrototype.Roles)
				{
					if (dictionary.ContainsKey(role))
					{
						list.Add(_prototypeManager.Index<JobPrototype>(role));
					}
				}
				list.Sort(JobUIComparer.Instance);
				if (list.Count == 0)
				{
					continue;
				}
				BoxContainer val7 = new BoxContainer();
				val7.Orientation = (LayoutOrientation)1;
				((Control)val7).Name = departmentPrototype.ID;
				((Control)val7).ToolTip = Loc.GetString("late-join-gui-jobs-amount-in-department-tooltip", new(string, object)[1] { ("departmentName", item) });
				BoxContainer val8 = val7;
				if (flag)
				{
					flag = false;
				}
				else
				{
					((Control)val8).AddChild(new Control
					{
						MinSize = new Vector2(0f, 23f)
					});
				}
				PanelContainer val9 = new PanelContainer();
				OrderedChildCollection children2 = ((Control)val9).Children;
				Label val10 = new Label();
				((Control)val10).StyleClasses.Add("LabelBig");
				val10.Text = Loc.GetString("late-join-gui-department-jobs-label", new(string, object)[1] { ("departmentName", item) });
				children2.Add((Control)(object)val10);
				((Control)val8).AddChild((Control)(object)val9);
				_jobCategories[id][departmentPrototype.ID] = val8;
				((Control)val).AddChild((Control)(object)val8);
				foreach (JobPrototype item2 in list)
				{
					int? num2 = dictionary[ProtoId<JobPrototype>.op_Implicit(item2.ID)];
					Label val11 = new Label
					{
						Margin = new Thickness(5f, 0f, 0f, 0f)
					};
					JobButton jobButton = new JobButton(val11, ProtoId<JobPrototype>.op_Implicit(item2.ID), item2.LocalizedName, num2);
					BoxContainer val12 = new BoxContainer
					{
						Orientation = (LayoutOrientation)0,
						HorizontalExpand = true
					};
					TextureRect val13 = new TextureRect
					{
						TextureScale = new Vector2(2f, 2f),
						VerticalAlignment = (VAlignment)2
					};
					JobIconPrototype jobIconPrototype = _prototypeManager.Index<JobIconPrototype>(item2.Icon);
					val13.Texture = _sprites.Frame0(jobIconPrototype.Icon);
					((Control)val12).AddChild((Control)(object)val13);
					((Control)val12).AddChild((Control)(object)val11);
					((Control)jobButton).AddChild((Control)(object)val12);
					((Control)val8).AddChild((Control)(object)jobButton);
					((BaseButton)jobButton).OnPressed += delegate
					{
						//IL_0020: Unknown result type (might be due to invalid IL or missing references)
						this.SelectedId((id, jobButton.JobId));
					};
					if (!_jobRequirements.IsAllowed(item2, (HumanoidCharacterProfile)(_preferencesManager.Preferences?.SelectedCharacter), out FormattedMessage reason))
					{
						((BaseButton)jobButton).Disabled = true;
						if (!reason.IsEmpty)
						{
							Tooltip tooltip = new Tooltip();
							tooltip.SetMessage(reason);
							((Control)jobButton).TooltipSupplier = (TooltipSupplier)((Control _) => (Control?)(object)tooltip);
						}
						((Control)val12).AddChild((Control)new TextureRect
						{
							TextureScale = new Vector2(0.4f, 0.4f),
							Stretch = (StretchMode)4,
							Texture = _sprites.Frame0((SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Nano/lock.svg.192dpi.png"))),
							HorizontalExpand = true,
							HorizontalAlignment = (HAlignment)3
						});
					}
					else if (num2 == 0)
					{
						((BaseButton)jobButton).Disabled = true;
					}
					if (!_jobButtons[id].ContainsKey(item2.ID))
					{
						_jobButtons[id][item2.ID] = new List<JobButton>();
					}
					_jobButtons[id][item2.ID].Add(jobButton);
				}
			}
		}
	}

	private void JobsAvailableUpdated(IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> updatedJobs)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> updatedJob in updatedJobs)
		{
			if (!_jobButtons.ContainsKey(updatedJob.Key))
			{
				continue;
			}
			Dictionary<ProtoId<JobPrototype>, int?> value = updatedJob.Value;
			foreach (KeyValuePair<string, List<JobButton>> item in _jobButtons[updatedJob.Key])
			{
				if (!value.ContainsKey(ProtoId<JobPrototype>.op_Implicit(item.Key)))
				{
					continue;
				}
				int? num = value[ProtoId<JobPrototype>.op_Implicit(item.Key)];
				foreach (JobButton item2 in item.Value)
				{
					if (item2.Amount != num)
					{
						item2.RefreshLabel(num);
						((BaseButton)item2).Disabled = ((BaseButton)item2).Disabled | (item2.Amount == 0);
					}
				}
			}
		}
	}

	[Obsolete]
	protected override void Dispose(bool disposing)
	{
		((DefaultWindow)this).Dispose(disposing);
		if (disposing)
		{
			_jobRequirements.Updated -= RebuildUI;
			_gameTicker.LobbyJobsAvailableUpdated -= JobsAvailableUpdated;
			_jobButtons.Clear();
			_jobCategories.Clear();
		}
	}
}
