using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.Lobby.UI;
using Content.Client._PUBG.Lobby;
using Content.Client._PUBG.Lobby.UI;
using Content.Client._PUBG.Party;
using Content.Client._PUBG.UserInterface.MainMenu;
using Content.Client._RMC14.LinkAccount;
using Content.Client.Guidebook;
using Content.Client.Humanoid;
using Content.Client.Inventory;
using Content.Client.Lobby.UI;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.Station;
using Content.Shared._PUBG;
using Content.Shared._PUBG.Party;
using Content.Shared._RMC14.Armor;
using Content.Shared.CCVar;
using Content.Shared.Clothing;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Traits;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby;

public sealed class LobbyUIController : UIController, IOnStateEntered<LobbyState>, IOnStateExited<LobbyState>, IOnStateEntered<CivLobbyState>, IOnStateExited<CivLobbyState>
{
	[Dependency]
	private IClientPreferencesManager _preferencesManager;

	[Dependency]
	private IConfigurationManager _configurationManager;

	[Dependency]
	private IFileDialogManager _dialogManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private JobRequirementsManager _requirements;

	[Dependency]
	private MarkingManager _markings;

	[Dependency]
	private LinkAccountManager _linkAccount;

	[UISystemDependency]
	private readonly HumanoidAppearanceSystem _humanoid;

	[UISystemDependency]
	private readonly ClientInventorySystem _inventory;

	[UISystemDependency]
	private readonly StationSpawningSystem _spawn;

	[UISystemDependency]
	private readonly GuidebookSystem _guide;

	[UISystemDependency]
	private readonly CMArmorSystem _armorSystem;

	private CharacterSetupGui? _characterSetup;

	private CivCharacterSetupGui? _civCharacterSetup;

	private HumanoidProfileEditor? _profileEditor;

	private CharacterSetupGuiSavePanel? _savePanel;

	private PubgCharacterSetupGui? _pubgCharacterSetup;

	private PubgHumanoidProfileEditor? _pubgProfileEditor;

	private PubgCharacterSetupGuiSavePanel? _pubgSavePanel;

	private bool _pubgMode = true;

	private Dictionary<string, int> _lobbyPermissions = new Dictionary<string, int>();

	private bool _permissionsSubscribed;

	private LobbyCharacterPreviewPanel? PreviewPanel => GetLobbyPreview();

	private HumanoidCharacterProfile? EditedProfile
	{
		get
		{
			if (!_pubgMode)
			{
				return _profileEditor?.Profile;
			}
			return _pubgProfileEditor?.Profile;
		}
	}

	private int? EditedSlot
	{
		get
		{
			if (!_pubgMode)
			{
				return _profileEditor?.CharacterSlot;
			}
			return _pubgProfileEditor?.CharacterSlot;
		}
	}

	public override void Initialize()
	{
		((UIController)this).Initialize();
		_prototypeManager.PrototypesReloaded += OnProtoReload;
		_preferencesManager.OnServerDataLoaded += PreferencesDataLoaded;
		_requirements.Updated += OnRequirementsUpdated;
		_configurationManager.OnValueChanged<bool>(CCVars.FlavorText, (Action<bool>)delegate
		{
			_profileEditor?.RefreshFlavorText();
		}, false);
		_configurationManager.OnValueChanged<bool>(CCVars.GameRoleTimers, (Action<bool>)delegate
		{
			RefreshProfileEditor();
		}, false);
		_configurationManager.OnValueChanged<bool>(CCVars.GameRoleWhitelist, (Action<bool>)delegate
		{
			RefreshProfileEditor();
		}, false);
		_linkAccount.Updated += RefreshProfileEditor;
		((UIController)this).SubscribeNetworkEvent<PubgModeStatusEvent>((EntitySessionEventHandler<PubgModeStatusEvent>)OnPubgModeStatus, (Type[])null, (Type[])null);
	}

	private void EnsurePermissionsSubscribed()
	{
		if (!_permissionsSubscribed)
		{
			base.EntityManager.System<LobbyPermissionsSystem>().OnPermissionsReceived += OnLobbyPermissions;
			_permissionsSubscribed = true;
		}
	}

	private void OnLobbyPermissions(Dictionary<string, int> permissions)
	{
		_lobbyPermissions = new Dictionary<string, int>(permissions);
		_pubgProfileEditor?.UpdatePermissions(_lobbyPermissions);
		_profileEditor?.UpdateCompactPermissions(_lobbyPermissions);
	}

	private void OnPubgModeStatus(PubgModeStatusEvent msg, EntitySessionEventArgs args)
	{
		if (_stateManager.CurrentState is CivLobbyState)
		{
			_pubgMode = false;
			return;
		}
		_pubgMode = msg.Enabled;
		State currentState = _stateManager.CurrentState;
		if ((currentState is LobbyState || currentState is CivLobbyState) ? true : false)
		{
			ReloadCharacterSetup();
		}
	}

	private LobbyCharacterPreviewPanel? GetLobbyPreview()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (_stateManager.CurrentState is CivLobbyState civLobbyState)
		{
			return civLobbyState.Lobby?.CharacterPreview;
		}
		if (_stateManager.CurrentState is LobbyState lobbyState)
		{
			LobbyGui lobby = lobbyState.Lobby;
			if (lobby == null)
			{
				return null;
			}
			PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
			if (pubgPreLobbyPartyClientSystem.Members.Count > 0 && val.HasValue)
			{
				int num = -1;
				for (int i = 0; i < pubgPreLobbyPartyClientSystem.Members.Count; i++)
				{
					NetUserId userId = pubgPreLobbyPartyClientSystem.Members[i].UserId;
					NetUserId? val2 = val;
					if (val2.HasValue && userId == val2.GetValueOrDefault())
					{
						num = i;
						break;
					}
				}
				return num switch
				{
					1 => lobby.PartySlot2Preview, 
					2 => lobby.PartySlot3Preview, 
					3 => lobby.PartySlot4Preview, 
					_ => lobby.CharacterPreview, 
				};
			}
			return lobby.CharacterPreview;
		}
		return null;
	}

	private void OnRequirementsUpdated()
	{
		if (_profileEditor != null)
		{
			_profileEditor.RefreshAntags();
			_profileEditor.RefreshJobs();
		}
	}

	private void OnProtoReload(PrototypesReloadedEventArgs obj)
	{
		if (_profileEditor != null)
		{
			if (obj.WasModified<AntagPrototype>())
			{
				_profileEditor.RefreshAntags();
			}
			if (obj.WasModified<JobPrototype>() || obj.WasModified<DepartmentPrototype>())
			{
				_profileEditor.RefreshJobs();
			}
			if (obj.WasModified<LoadoutPrototype>() || obj.WasModified<LoadoutGroupPrototype>() || obj.WasModified<RoleLoadoutPrototype>())
			{
				_profileEditor.RefreshLoadouts();
			}
			if (obj.WasModified<SpeciesPrototype>())
			{
				_profileEditor.RefreshSpecies();
			}
			if (obj.WasModified<TraitPrototype>())
			{
				_profileEditor.RefreshTraits();
			}
		}
	}

	private void PreferencesDataLoaded()
	{
		PreviewPanel?.SetLoaded(value: true);
		if (_stateManager.CurrentState is LobbyState || _stateManager.CurrentState is CivLobbyState)
		{
			ReloadCharacterSetup();
		}
	}

	public void OnStateEntered(LobbyState state)
	{
		_pubgMode = true;
		PreviewPanel?.SetLoaded(_preferencesManager.ServerDataLoaded);
		EnsurePermissionsSubscribed();
		base.EntityManager.System<LobbyPermissionsSystem>().RequestPermissions();
		ReloadCharacterSetup();
	}

	public void OnStateEntered(CivLobbyState state)
	{
		_pubgMode = false;
		PreviewPanel?.SetLoaded(_preferencesManager.ServerDataLoaded);
		EnsurePermissionsSubscribed();
		base.EntityManager.System<LobbyPermissionsSystem>().RequestPermissions();
		ReloadCharacterSetup();
	}

	public void OnStateExited(LobbyState state)
	{
		ResetLobbyState();
	}

	public void OnStateExited(CivLobbyState state)
	{
		ResetLobbyState();
	}

	private void ResetLobbyState()
	{
		PreviewPanel?.SetLoaded(value: false);
		HumanoidProfileEditor? profileEditor = _profileEditor;
		if (profileEditor != null)
		{
			((Control)profileEditor).Orphan();
		}
		CharacterSetupGui? characterSetup = _characterSetup;
		if (characterSetup != null)
		{
			((Control)characterSetup).Orphan();
		}
		CivCharacterSetupGui? civCharacterSetup = _civCharacterSetup;
		if (civCharacterSetup != null)
		{
			((Control)civCharacterSetup).Orphan();
		}
		PubgHumanoidProfileEditor? pubgProfileEditor = _pubgProfileEditor;
		if (pubgProfileEditor != null)
		{
			((Control)pubgProfileEditor).Orphan();
		}
		PubgCharacterSetupGui? pubgCharacterSetup = _pubgCharacterSetup;
		if (pubgCharacterSetup != null)
		{
			((Control)pubgCharacterSetup).Orphan();
		}
		_characterSetup = null;
		_civCharacterSetup = null;
		_profileEditor = null;
		_pubgCharacterSetup = null;
		_pubgProfileEditor = null;
		_lobbyPermissions.Clear();
		_permissionsSubscribed = false;
	}

	public void ReloadCharacterSetup()
	{
		if (_entityManager.System<PubgPreLobbyPartyClientSystem>().Members.Count <= 1)
		{
			RefreshLobbyPreview();
		}
		if (_stateManager.CurrentState is CivLobbyState)
		{
			if (_civCharacterSetup == null)
			{
				Control? characterSetupContainer = GetCharacterSetupContainer();
				if (characterSetupContainer == null || !characterSetupContainer.Visible)
				{
					return;
				}
			}
			CivCharacterSetupGui item = EnsureCivGui().Item1;
			item.LoadCharacterProfile();
			return;
		}
		if (_pubgMode)
		{
			PubgCharacterSetupGui item2 = EnsurePubgGui().Item1;
			item2.LoadCharacterProfile();
			return;
		}
		if (_characterSetup == null)
		{
			Control? characterSetupContainer2 = GetCharacterSetupContainer();
			if (characterSetupContainer2 == null || !characterSetupContainer2.Visible)
			{
				return;
			}
		}
		var (characterSetupGui, humanoidProfileEditor) = EnsureGui();
		characterSetupGui.ReloadCharacterPickers();
		humanoidProfileEditor.SetProfile((HumanoidCharacterProfile)(_preferencesManager.Preferences?.SelectedCharacter), _preferencesManager.Preferences?.SelectedCharacterIndex);
	}

	private void RefreshLobbyPreview()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (PreviewPanel != null)
		{
			if (!(_preferencesManager.Preferences?.SelectedCharacter is HumanoidCharacterProfile humanoid))
			{
				PreviewPanel.SetSprite(EntityUid.Invalid);
				PreviewPanel.SetSummaryText(string.Empty);
			}
			else
			{
				SetLocalPreviewForPanel(PreviewPanel, humanoid);
			}
		}
	}

	private void SetLocalPreviewForPanel(LobbyCharacterPreviewPanel panel, HumanoidCharacterProfile? humanoid)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (humanoid == null)
		{
			panel.SetSprite(EntityUid.Invalid);
			panel.SetSummaryText(string.Empty);
			return;
		}
		EntityUid val = LoadProfileEntity(humanoid, null, jobClothes: false);
		Dictionary<string, string> localOutfitFromCaches = GetLocalOutfitFromCaches();
		if (localOutfitFromCaches.Count > 0)
		{
			ApplySkinsToDummy(val, localOutfitFromCaches);
		}
		panel.SetSprite(val);
		panel.SetSummaryText(humanoid.Summary);
	}

	public void UpdatePartyPreviews()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!(_stateManager.CurrentState is LobbyState lobbyState))
		{
			return;
		}
		LobbyGui lobby = lobbyState.Lobby;
		if (lobby == null)
		{
			return;
		}
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
		IReadOnlyList<PubgPreLobbyPartyMemberState> members = pubgPreLobbyPartyClientSystem.Members;
		int localIndex = -1;
		for (int i = 0; i < members.Count; i++)
		{
			NetUserId userId = members[i].UserId;
			NetUserId? val2 = val;
			if (val2.HasValue && userId == val2.GetValueOrDefault())
			{
				localIndex = i;
				break;
			}
		}
		int memberCount = Math.Max(members.Count, 1);
		UpdatePartyPreviewSlot(lobby.CharacterPreview, 0, localIndex, memberCount, members);
		UpdatePartyPreviewSlot(lobby.PartySlot2Preview, 1, localIndex, memberCount, members);
		UpdatePartyPreviewSlot(lobby.PartySlot3Preview, 2, localIndex, memberCount, members);
		UpdatePartyPreviewSlot(lobby.PartySlot4Preview, 3, localIndex, memberCount, members);
	}

	private void UpdatePartyPreviewSlot(LobbyCharacterPreviewPanel panel, int slotIndex, int localIndex, int memberCount, IReadOnlyList<PubgPreLobbyPartyMemberState> members)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (slotIndex >= memberCount)
		{
			panel.ClearSprite();
			panel.SetSummaryText(string.Empty);
			return;
		}
		bool flag = (localIndex >= 0 && slotIndex == localIndex) || (localIndex < 0 && slotIndex == 0);
		HumanoidCharacterProfile humanoidCharacterProfile = null;
		if (flag)
		{
			object obj = _preferencesManager.Preferences?.SelectedCharacter as HumanoidCharacterProfile;
			HumanoidCharacterProfile humanoidCharacterProfile2 = ((slotIndex < members.Count) ? members[slotIndex].Profile : null);
			if (obj == null)
			{
				obj = humanoidCharacterProfile2;
			}
			humanoidCharacterProfile = (HumanoidCharacterProfile)obj;
		}
		else
		{
			humanoidCharacterProfile = ((members.Count > slotIndex) ? members[slotIndex].Profile : null);
		}
		if (humanoidCharacterProfile == null)
		{
			humanoidCharacterProfile = HumanoidCharacterProfile.DefaultWithSpecies();
		}
		EntityUid val = LoadProfileEntity(humanoidCharacterProfile, null, jobClothes: false);
		Dictionary<string, string> outfitForPreviewSlot = GetOutfitForPreviewSlot(flag, slotIndex, members);
		if (outfitForPreviewSlot.Count > 0)
		{
			ApplySkinsToDummy(val, outfitForPreviewSlot);
		}
		panel.SetSprite(val);
		panel.SetSummaryText((!flag) ? string.Empty : (humanoidCharacterProfile?.Summary ?? string.Empty));
	}

	private Dictionary<string, string> GetOutfitForPreviewSlot(bool isLocalSlot, int slotIndex, IReadOnlyList<PubgPreLobbyPartyMemberState> members)
	{
		if (slotIndex < members.Count && members[slotIndex].CurrentOutfit.Count > 0)
		{
			return members[slotIndex].CurrentOutfit;
		}
		if (!isLocalSlot)
		{
			return new Dictionary<string, string>();
		}
		return GetLocalOutfitFromCaches();
	}

	private Dictionary<string, string> GetLocalOutfitFromCaches()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		if (val.HasValue)
		{
			foreach (PubgPreLobbyPartyMemberState member in pubgPreLobbyPartyClientSystem.Members)
			{
				NetUserId userId = member.UserId;
				NetUserId? val2 = val;
				if (val2.HasValue && userId == val2.GetValueOrDefault() && member.CurrentOutfit.Count > 0)
				{
					return member.CurrentOutfit;
				}
			}
		}
		return base.UIManager.GetUIController<MainMenuUIController>().GetCachedCurrentOutfit();
	}

	private void RefreshProfileEditor()
	{
		_profileEditor?.RefreshAntags();
		_profileEditor?.RefreshJobs();
		_profileEditor?.RefreshLoadouts();
		_profileEditor?.RefreshRMC(_linkAccount.Tier);
	}

	private void SaveProfile()
	{
		if (EditedProfile != null && EditedSlot.HasValue && (_preferencesManager.Preferences?.SelectedCharacterIndex).HasValue)
		{
			_preferencesManager.UpdateCharacter(EditedProfile, EditedSlot.Value);
			ReloadCharacterSetup();
		}
	}

	private void CloseProfileEditor()
	{
		if (_pubgMode)
		{
			if (_pubgProfileEditor == null)
			{
				return;
			}
			_pubgProfileEditor.SetProfile(null, null);
			((Control)_pubgCharacterSetup).Visible = false;
		}
		else if (_stateManager.CurrentState is CivLobbyState)
		{
			if (_profileEditor == null)
			{
				return;
			}
			_profileEditor.SetProfile(null, null);
			if (_civCharacterSetup != null)
			{
				((Control)_civCharacterSetup).Visible = false;
			}
		}
		else
		{
			if (_profileEditor == null)
			{
				return;
			}
			_profileEditor.SetProfile(null, null);
			((Control)_profileEditor).Visible = false;
		}
		State currentState = _stateManager.CurrentState;
		if (!(currentState is LobbyState lobbyState))
		{
			if (currentState is CivLobbyState civLobbyState)
			{
				civLobbyState.SwitchState(CivLobbyGui.LobbyGuiState.Default);
			}
		}
		else
		{
			lobbyState.SwitchState(LobbyGui.LobbyGuiState.Default);
		}
	}

	private void OpenSavePanel()
	{
		if (_pubgMode)
		{
			PubgCharacterSetupGuiSavePanel pubgSavePanel = _pubgSavePanel;
			if (pubgSavePanel == null || !((BaseWindow)pubgSavePanel).IsOpen)
			{
				_pubgSavePanel = new PubgCharacterSetupGuiSavePanel();
				((BaseButton)_pubgSavePanel.SaveButton).OnPressed += delegate
				{
					SaveProfile();
					((BaseWindow)_pubgSavePanel).Close();
					CloseProfileEditor();
				};
				((BaseButton)_pubgSavePanel.NoSaveButton).OnPressed += delegate
				{
					((BaseWindow)_pubgSavePanel).Close();
					CloseProfileEditor();
				};
				((BaseWindow)_pubgSavePanel).OpenCentered();
			}
			return;
		}
		CharacterSetupGuiSavePanel savePanel = _savePanel;
		if (savePanel == null || !((BaseWindow)savePanel).IsOpen)
		{
			_savePanel = new CharacterSetupGuiSavePanel();
			((BaseButton)_savePanel.SaveButton).OnPressed += delegate
			{
				SaveProfile();
				((BaseWindow)_savePanel).Close();
				CloseProfileEditor();
			};
			((BaseButton)_savePanel.NoSaveButton).OnPressed += delegate
			{
				((BaseWindow)_savePanel).Close();
				CloseProfileEditor();
			};
			((BaseWindow)_savePanel).OpenCentered();
		}
	}

	private (CharacterSetupGui, HumanoidProfileEditor) EnsureGui()
	{
		if (_characterSetup != null && _profileEditor != null)
		{
			((Control)_characterSetup).Visible = true;
			((Control)_profileEditor).Visible = true;
			return (_characterSetup, _profileEditor);
		}
		_profileEditor = CreateProfileEditor();
		_profileEditor.OnOpenGuidebook += _guide.OpenHelp;
		_characterSetup = new CharacterSetupGui(_profileEditor);
		((BaseButton)_characterSetup.CloseButton).OnPressed += delegate
		{
			if (_profileEditor.Profile != null && _profileEditor.IsDirty)
			{
				OpenSavePanel();
			}
			else
			{
				CloseProfileEditor();
			}
		};
		_profileEditor.Save += SaveProfile;
		_characterSetup.SelectCharacter += delegate(int args)
		{
			_preferencesManager.SelectCharacter(args);
			ReloadCharacterSetup();
		};
		_characterSetup.DeleteCharacter += delegate(int args)
		{
			_preferencesManager.DeleteCharacter(args);
			if (EditedSlot == args)
			{
				ReloadCharacterSetup();
			}
			else
			{
				_characterSetup?.ReloadCharacterPickers();
			}
		};
		Control? characterSetupContainer = GetCharacterSetupContainer();
		if (characterSetupContainer != null)
		{
			characterSetupContainer.AddChild((Control)(object)_characterSetup);
		}
		return (_characterSetup, _profileEditor);
	}

	private (CivCharacterSetupGui, HumanoidProfileEditor) EnsureCivGui()
	{
		if (_civCharacterSetup != null && _profileEditor != null)
		{
			((Control)_civCharacterSetup).Visible = true;
			((Control)_profileEditor).Visible = true;
			_profileEditor.UpdateCompactPermissions(_lobbyPermissions);
			return (_civCharacterSetup, _profileEditor);
		}
		_profileEditor = CreateProfileEditor();
		_profileEditor.EnableCompactLayout();
		_profileEditor.UpdateCompactPermissions(_lobbyPermissions);
		_profileEditor.OnOpenGuidebook += _guide.OpenHelp;
		_civCharacterSetup = new CivCharacterSetupGui(_profileEditor);
		((BaseButton)_civCharacterSetup.CloseButton).OnPressed += delegate
		{
			if (_profileEditor.Profile != null && _profileEditor.IsDirty)
			{
				OpenSavePanel();
			}
			else
			{
				CloseProfileEditor();
			}
		};
		_profileEditor.Save += SaveProfile;
		Control? characterSetupContainer = GetCharacterSetupContainer();
		if (characterSetupContainer != null)
		{
			characterSetupContainer.AddChild((Control)(object)_civCharacterSetup);
		}
		return (_civCharacterSetup, _profileEditor);
	}

	private (PubgCharacterSetupGui, PubgHumanoidProfileEditor) EnsurePubgGui()
	{
		if (_pubgCharacterSetup != null && _pubgProfileEditor != null)
		{
			((Control)_pubgCharacterSetup).Visible = true;
			_pubgProfileEditor.UpdatePermissions(_lobbyPermissions);
			return (_pubgCharacterSetup, _pubgProfileEditor);
		}
		_pubgProfileEditor = new PubgHumanoidProfileEditor(_markings);
		_pubgProfileEditor.UpdatePermissions(_lobbyPermissions);
		_pubgCharacterSetup = new PubgCharacterSetupGui(_pubgProfileEditor);
		((BaseButton)_pubgCharacterSetup.CloseButton).OnPressed += delegate
		{
			if (_pubgProfileEditor.Profile != null && _pubgProfileEditor.IsDirty)
			{
				OpenSavePanel();
			}
			else
			{
				CloseProfileEditor();
			}
		};
		_pubgProfileEditor.Save += SaveProfile;
		Control? characterSetupContainer = GetCharacterSetupContainer();
		if (characterSetupContainer != null)
		{
			characterSetupContainer.AddChild((Control)(object)_pubgCharacterSetup);
		}
		return (_pubgCharacterSetup, _pubgProfileEditor);
	}

	private HumanoidProfileEditor CreateProfileEditor()
	{
		return new HumanoidProfileEditor(_preferencesManager, _configurationManager, base.EntityManager, _dialogManager, base.LogManager, _playerManager, _prototypeManager, (IResourceManager)(object)_resourceCache, _requirements, _markings);
	}

	private Control? GetCharacterSetupContainer()
	{
		State currentState = _stateManager.CurrentState;
		if (!(currentState is LobbyState lobbyState))
		{
			if (currentState is CivLobbyState civLobbyState)
			{
				return civLobbyState.Lobby?.CharacterSetupState;
			}
			return null;
		}
		return lobbyState.Lobby?.CharacterSetupState;
	}

	public void GiveDummyJobClothesLoadout(EntityUid dummy, JobPrototype? jobProto, HumanoidCharacterProfile profile)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		JobPrototype jobPrototype = jobProto ?? GetPreferredJob(profile);
		GiveDummyJobClothes(dummy, profile, jobPrototype);
		if (_prototypeManager.HasIndex<RoleLoadoutPrototype>(LoadoutSystem.GetJobPrototype(jobPrototype.ID)))
		{
			RoleLoadout loadoutOrDefault = profile.GetLoadoutOrDefault(LoadoutSystem.GetJobPrototype(jobPrototype.ID), ((ISharedPlayerManager)_playerManager).LocalSession, profile.Species, base.EntityManager, _prototypeManager);
			GiveDummyLoadout(dummy, loadoutOrDefault);
		}
	}

	public JobPrototype GetPreferredJob(HumanoidCharacterProfile profile)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<JobPrototype> key = profile.JobPriorities.FirstOrDefault<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>((KeyValuePair<ProtoId<JobPrototype>, JobPriority> p) => p.Value == JobPriority.High).Key;
		return _prototypeManager.Index<JobPrototype>(key.Id ?? ProtoId<JobPrototype>.op_Implicit(SharedGameTicker.FallbackOverflowJob));
	}

	public void GiveDummyLoadout(EntityUid uid, RoleLoadout? roleLoadout)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (roleLoadout == null)
		{
			return;
		}
		LoadoutPrototype loadout = default(LoadoutPrototype);
		foreach (List<Loadout> value in roleLoadout.SelectedLoadouts.Values)
		{
			foreach (Loadout item in value)
			{
				if (_prototypeManager.TryIndex<LoadoutPrototype>(item.Prototype, ref loadout))
				{
					_spawn.EquipStartingGear(uid, loadout);
				}
			}
		}
	}

	public void GiveDummyJobClothes(EntityUid dummy, HumanoidCharacterProfile profile, JobPrototype job)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventory.TryGetSlots(dummy, out SlotDefinition[] slotDefinitions))
		{
			return;
		}
		SlotDefinition[] array;
		if (profile.Loadouts.TryGetValue(job.ID, out RoleLoadout value))
		{
			LoadoutPrototype loadoutPrototype = default(LoadoutPrototype);
			StartingGearPrototype startingGearPrototype = default(StartingGearPrototype);
			foreach (List<Loadout> value2 in value.SelectedLoadouts.Values)
			{
				foreach (Loadout item2 in value2)
				{
					if (!_prototypeManager.TryIndex<LoadoutPrototype>(item2.Prototype, ref loadoutPrototype))
					{
						continue;
					}
					array = slotDefinitions;
					foreach (SlotDefinition slotDefinition in array)
					{
						if (_prototypeManager.TryIndex<StartingGearPrototype>(loadoutPrototype.StartingGear, ref startingGearPrototype))
						{
							string gear = ((IEquipmentLoadout)startingGearPrototype).GetGear(slotDefinition.Name);
							if (_inventory.TryUnequip(dummy, slotDefinition.Name, out var removedItem, silent: true, force: true, predicted: false, null, null, reparent: false))
							{
								base.EntityManager.DeleteEntity((EntityUid?)removedItem.Value);
							}
							if (gear != string.Empty)
							{
								EntityUid itemUid = base.EntityManager.SpawnEntity(gear, MapCoordinates.Nullspace, (ComponentRegistry)null);
								_inventory.TryEquip(dummy, itemUid, slotDefinition.Name, silent: true, force: true);
							}
						}
						else
						{
							string gear2 = ((IEquipmentLoadout)loadoutPrototype).GetGear(slotDefinition.Name);
							if (_inventory.TryUnequip(dummy, slotDefinition.Name, out var removedItem2, silent: true, force: true, predicted: false, null, null, reparent: false))
							{
								base.EntityManager.DeleteEntity((EntityUid?)removedItem2.Value);
							}
							if (gear2 != string.Empty)
							{
								EntityUid itemUid2 = base.EntityManager.SpawnEntity(gear2, MapCoordinates.Nullspace, (ComponentRegistry)null);
								_inventory.TryEquip(dummy, itemUid2, slotDefinition.Name, silent: true, force: true);
							}
						}
					}
				}
			}
		}
		StartingGearPrototype startingGearPrototype2 = default(StartingGearPrototype);
		if (!_prototypeManager.TryIndex<StartingGearPrototype>(job.StartingGear, ref startingGearPrototype2))
		{
			return;
		}
		StartingGearPrototype startingGearPrototype3 = default(StartingGearPrototype);
		_prototypeManager.TryIndex<StartingGearPrototype>(job.DummyStartingGear, ref startingGearPrototype3);
		array = slotDefinitions;
		RMCArmorVariantComponent item = default(RMCArmorVariantComponent);
		foreach (SlotDefinition slotDefinition2 in array)
		{
			string gear3 = ((IEquipmentLoadout)startingGearPrototype2).GetGear(slotDefinition2.Name);
			if (gear3 == string.Empty && startingGearPrototype3 != null)
			{
				gear3 = ((IEquipmentLoadout)startingGearPrototype3).GetGear(slotDefinition2.Name);
			}
			if (_inventory.TryUnequip(dummy, slotDefinition2.Name, out var removedItem3, silent: true, force: true, predicted: false, null, null, reparent: false))
			{
				base.EntityManager.DeleteEntity((EntityUid?)removedItem3.Value);
			}
			if (gear3 != string.Empty)
			{
				EntityUid val = base.EntityManager.SpawnEntity(gear3, MapCoordinates.Nullspace, (ComponentRegistry)null);
				if (base.EntityManager.TryGetComponent<RMCArmorVariantComponent>(val, ref item))
				{
					EntProtoId armorVariant = _armorSystem.GetArmorVariant(Entity<RMCArmorVariantComponent>.op_Implicit((val, item)), profile.ArmorPreference);
					EntityUid itemUid3 = base.EntityManager.SpawnEntity(EntProtoId.op_Implicit(armorVariant), MapCoordinates.Nullspace, (ComponentRegistry)null);
					_inventory.TryEquip(dummy, itemUid3, slotDefinition2.Name, silent: true, force: true);
					base.EntityManager.QueueDeleteEntity((EntityUid?)val);
				}
				else
				{
					_inventory.TryEquip(dummy, val, slotDefinition2.Name, silent: true, force: true);
				}
			}
		}
	}

	public EntityUid LoadProfileEntity(HumanoidCharacterProfile? humanoid, JobPrototype? job, bool jobClothes)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? val = null;
		if (humanoid != null && jobClothes)
		{
			if (job == null)
			{
				job = GetPreferredJob(humanoid);
			}
			val = job.JobPreviewEntity ?? EntProtoId.op_Implicit(job?.JobEntity);
		}
		if (val.HasValue)
		{
			IEntityManager entityManager = base.EntityManager;
			EntProtoId? val2 = val;
			return entityManager.SpawnEntity(val2.HasValue ? EntProtoId.op_Implicit(val2.GetValueOrDefault()) : null, MapCoordinates.Nullspace, (ComponentRegistry)null);
		}
		EntityUid val3;
		if (humanoid != null)
		{
			EntProtoId dollPrototype = _prototypeManager.Index<SpeciesPrototype>(humanoid.Species).DollPrototype;
			val3 = base.EntityManager.SpawnEntity(EntProtoId.op_Implicit(dollPrototype), MapCoordinates.Nullspace, (ComponentRegistry)null);
		}
		else
		{
			val3 = base.EntityManager.SpawnEntity(EntProtoId.op_Implicit(_prototypeManager.Index<SpeciesPrototype>(SharedHumanoidAppearanceSystem.DefaultSpecies).DollPrototype), MapCoordinates.Nullspace, (ComponentRegistry)null);
		}
		_humanoid.LoadProfile(val3, humanoid);
		if (humanoid != null && jobClothes)
		{
			GiveDummyJobClothes(val3, humanoid, job);
			if (_prototypeManager.HasIndex<RoleLoadoutPrototype>(LoadoutSystem.GetJobPrototype(job.ID)))
			{
				RoleLoadout loadoutOrDefault = humanoid.GetLoadoutOrDefault(LoadoutSystem.GetJobPrototype(job.ID), ((ISharedPlayerManager)_playerManager).LocalSession, humanoid.Species, base.EntityManager, _prototypeManager);
				GiveDummyLoadout(val3, loadoutOrDefault);
			}
		}
		return val3;
	}

	private void ApplySkinsToDummy(EntityUid dummy, Dictionary<string, string> currentOutfit)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventory.TryGetSlots(dummy, out SlotDefinition[] slotDefinitions))
		{
			return;
		}
		SlotDefinition[] array = slotDefinitions;
		foreach (SlotDefinition slotDefinition in array)
		{
			if (_inventory.TryUnequip(dummy, slotDefinition.Name, out var removedItem, silent: true, force: true, predicted: false, null, null, reparent: false))
			{
				base.EntityManager.DeleteEntity((EntityUid?)removedItem.Value);
			}
		}
		foreach (var (text3, text4) in currentOutfit)
		{
			if (!string.IsNullOrEmpty(text4) && !(text3 == "equippedEmotes") && _prototypeManager.HasIndex<EntityPrototype>(text4))
			{
				EntityUid itemUid = base.EntityManager.SpawnEntity(text4, MapCoordinates.Nullspace, (ComponentRegistry)null);
				_inventory.TryEquip(dummy, itemUid, text3, silent: true, force: true);
			}
		}
	}
}
