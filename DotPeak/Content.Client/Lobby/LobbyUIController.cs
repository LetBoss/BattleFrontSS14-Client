// Decompiled with JetBrains decompiler
// Type: Content.Client.Lobby.LobbyUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Content.Shared.Guidebook;
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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Lobby;

public sealed class LobbyUIController : 
  UIController,
  IOnStateEntered<LobbyState>,
  IOnStateExited<LobbyState>,
  IOnStateEntered<CivLobbyState>,
  IOnStateExited<CivLobbyState>
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

  private LobbyCharacterPreviewPanel? PreviewPanel => this.GetLobbyPreview();

  private HumanoidCharacterProfile? EditedProfile
  {
    get => !this._pubgMode ? this._profileEditor?.Profile : this._pubgProfileEditor?.Profile;
  }

  private int? EditedSlot
  {
    get
    {
      return !this._pubgMode ? this._profileEditor?.CharacterSlot : this._pubgProfileEditor?.CharacterSlot;
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._prototypeManager.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnProtoReload);
    this._preferencesManager.OnServerDataLoaded += new Action(this.PreferencesDataLoaded);
    this._requirements.Updated += new Action(this.OnRequirementsUpdated);
    this._configurationManager.OnValueChanged<bool>(CCVars.FlavorText, (Action<bool>) (args => this._profileEditor?.RefreshFlavorText()), false);
    this._configurationManager.OnValueChanged<bool>(CCVars.GameRoleTimers, (Action<bool>) (_ => this.RefreshProfileEditor()), false);
    this._configurationManager.OnValueChanged<bool>(CCVars.GameRoleWhitelist, (Action<bool>) (_ => this.RefreshProfileEditor()), false);
    this._linkAccount.Updated += new Action(this.RefreshProfileEditor);
    this.SubscribeNetworkEvent<PubgModeStatusEvent>(new EntitySessionEventHandler<PubgModeStatusEvent>(this.OnPubgModeStatus), (Type[]) null, (Type[]) null);
  }

  private void EnsurePermissionsSubscribed()
  {
    if (this._permissionsSubscribed)
      return;
    this.EntityManager.System<LobbyPermissionsSystem>().OnPermissionsReceived += new Action<Dictionary<string, int>>(this.OnLobbyPermissions);
    this._permissionsSubscribed = true;
  }

  private void OnLobbyPermissions(Dictionary<string, int> permissions)
  {
    this._lobbyPermissions = new Dictionary<string, int>((IDictionary<string, int>) permissions);
    this._pubgProfileEditor?.UpdatePermissions(this._lobbyPermissions);
    this._profileEditor?.UpdateCompactPermissions(this._lobbyPermissions);
  }

  private void OnPubgModeStatus(PubgModeStatusEvent msg, EntitySessionEventArgs args)
  {
    if (this._stateManager.CurrentState is CivLobbyState)
    {
      this._pubgMode = false;
    }
    else
    {
      this._pubgMode = msg.Enabled;
      bool flag;
      switch (this._stateManager.CurrentState)
      {
        case LobbyState _:
        case CivLobbyState _:
          flag = true;
          break;
        default:
          flag = false;
          break;
      }
      if (!flag)
        return;
      this.ReloadCharacterSetup();
    }
  }

  private LobbyCharacterPreviewPanel? GetLobbyPreview()
  {
    if (this._stateManager.CurrentState is CivLobbyState currentState1)
      return currentState1.Lobby?.CharacterPreview;
    if (!(this._stateManager.CurrentState is LobbyState currentState2))
      return (LobbyCharacterPreviewPanel) null;
    LobbyGui lobby = currentState2.Lobby;
    if (lobby == null)
      return (LobbyCharacterPreviewPanel) null;
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    NetUserId? userId1 = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
    if (partyClientSystem.Members.Count <= 0 || !userId1.HasValue)
      return lobby.CharacterPreview;
    int num = -1;
    for (int index = 0; index < partyClientSystem.Members.Count; ++index)
    {
      NetUserId userId2 = partyClientSystem.Members[index].UserId;
      NetUserId? nullable = userId1;
      if ((nullable.HasValue ? (NetUserId.op_Equality(userId2, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
      {
        num = index;
        break;
      }
    }
    LobbyCharacterPreviewPanel lobbyPreview;
    switch (num)
    {
      case 1:
        lobbyPreview = lobby.PartySlot2Preview;
        break;
      case 2:
        lobbyPreview = lobby.PartySlot3Preview;
        break;
      case 3:
        lobbyPreview = lobby.PartySlot4Preview;
        break;
      default:
        lobbyPreview = lobby.CharacterPreview;
        break;
    }
    return lobbyPreview;
  }

  private void OnRequirementsUpdated()
  {
    if (this._profileEditor == null)
      return;
    this._profileEditor.RefreshAntags();
    this._profileEditor.RefreshJobs();
  }

  private void OnProtoReload(PrototypesReloadedEventArgs obj)
  {
    if (this._profileEditor == null)
      return;
    if (obj.WasModified<AntagPrototype>())
      this._profileEditor.RefreshAntags();
    if (obj.WasModified<JobPrototype>() || obj.WasModified<DepartmentPrototype>())
      this._profileEditor.RefreshJobs();
    if (obj.WasModified<LoadoutPrototype>() || obj.WasModified<LoadoutGroupPrototype>() || obj.WasModified<RoleLoadoutPrototype>())
      this._profileEditor.RefreshLoadouts();
    if (obj.WasModified<SpeciesPrototype>())
      this._profileEditor.RefreshSpecies();
    if (!obj.WasModified<TraitPrototype>())
      return;
    this._profileEditor.RefreshTraits();
  }

  private void PreferencesDataLoaded()
  {
    this.PreviewPanel?.SetLoaded(true);
    if (!(this._stateManager.CurrentState is LobbyState) && !(this._stateManager.CurrentState is CivLobbyState))
      return;
    this.ReloadCharacterSetup();
  }

  public void OnStateEntered(LobbyState state)
  {
    this._pubgMode = true;
    this.PreviewPanel?.SetLoaded(this._preferencesManager.ServerDataLoaded);
    this.EnsurePermissionsSubscribed();
    this.EntityManager.System<LobbyPermissionsSystem>().RequestPermissions();
    this.ReloadCharacterSetup();
  }

  public void OnStateEntered(CivLobbyState state)
  {
    this._pubgMode = false;
    this.PreviewPanel?.SetLoaded(this._preferencesManager.ServerDataLoaded);
    this.EnsurePermissionsSubscribed();
    this.EntityManager.System<LobbyPermissionsSystem>().RequestPermissions();
    this.ReloadCharacterSetup();
  }

  public void OnStateExited(LobbyState state) => this.ResetLobbyState();

  public void OnStateExited(CivLobbyState state) => this.ResetLobbyState();

  private void ResetLobbyState()
  {
    this.PreviewPanel?.SetLoaded(false);
    ((Control) this._profileEditor)?.Orphan();
    this._characterSetup?.Orphan();
    this._civCharacterSetup?.Orphan();
    ((Control) this._pubgProfileEditor)?.Orphan();
    this._pubgCharacterSetup?.Orphan();
    this._characterSetup = (CharacterSetupGui) null;
    this._civCharacterSetup = (CivCharacterSetupGui) null;
    this._profileEditor = (HumanoidProfileEditor) null;
    this._pubgCharacterSetup = (PubgCharacterSetupGui) null;
    this._pubgProfileEditor = (PubgHumanoidProfileEditor) null;
    this._lobbyPermissions.Clear();
    this._permissionsSubscribed = false;
  }

  public void ReloadCharacterSetup()
  {
    if (this._entityManager.System<PubgPreLobbyPartyClientSystem>().Members.Count <= 1)
      this.RefreshLobbyPreview();
    if (this._stateManager.CurrentState is CivLobbyState)
    {
      if (this._civCharacterSetup == null)
      {
        Control characterSetupContainer = this.GetCharacterSetupContainer();
        if ((characterSetupContainer != null ? (!characterSetupContainer.Visible ? 1 : 0) : 1) != 0)
          return;
      }
      this.EnsureCivGui().Item1.LoadCharacterProfile();
    }
    else if (this._pubgMode)
    {
      this.EnsurePubgGui().Item1.LoadCharacterProfile();
    }
    else
    {
      if (this._characterSetup == null)
      {
        Control characterSetupContainer = this.GetCharacterSetupContainer();
        if ((characterSetupContainer != null ? (!characterSetupContainer.Visible ? 1 : 0) : 1) != 0)
          return;
      }
      (CharacterSetupGui characterSetupGui, HumanoidProfileEditor humanoidProfileEditor) = this.EnsureGui();
      characterSetupGui.ReloadCharacterPickers();
      HumanoidCharacterProfile selectedCharacter = (HumanoidCharacterProfile) this._preferencesManager.Preferences?.SelectedCharacter;
      int? selectedCharacterIndex = this._preferencesManager.Preferences?.SelectedCharacterIndex;
      humanoidProfileEditor.SetProfile(selectedCharacter, selectedCharacterIndex);
    }
  }

  private void RefreshLobbyPreview()
  {
    if (this.PreviewPanel == null)
      return;
    if (!(this._preferencesManager.Preferences?.SelectedCharacter is HumanoidCharacterProfile selectedCharacter))
    {
      this.PreviewPanel.SetSprite(EntityUid.Invalid);
      this.PreviewPanel.SetSummaryText(string.Empty);
    }
    else
      this.SetLocalPreviewForPanel(this.PreviewPanel, selectedCharacter);
  }

  private void SetLocalPreviewForPanel(
    LobbyCharacterPreviewPanel panel,
    HumanoidCharacterProfile? humanoid)
  {
    if (humanoid == null)
    {
      panel.SetSprite(EntityUid.Invalid);
      panel.SetSummaryText(string.Empty);
    }
    else
    {
      EntityUid entityUid = this.LoadProfileEntity(humanoid, (JobPrototype) null, false);
      Dictionary<string, string> outfitFromCaches = this.GetLocalOutfitFromCaches();
      if (outfitFromCaches.Count > 0)
        this.ApplySkinsToDummy(entityUid, outfitFromCaches);
      panel.SetSprite(entityUid);
      panel.SetSummaryText(humanoid.Summary);
    }
  }

  public void UpdatePartyPreviews()
  {
    if (!(this._stateManager.CurrentState is LobbyState currentState))
      return;
    LobbyGui lobby = currentState.Lobby;
    if (lobby == null)
      return;
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    NetUserId? userId1 = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
    IReadOnlyList<PubgPreLobbyPartyMemberState> members = partyClientSystem.Members;
    int localIndex = -1;
    for (int index = 0; index < members.Count; ++index)
    {
      NetUserId userId2 = members[index].UserId;
      NetUserId? nullable = userId1;
      if ((nullable.HasValue ? (NetUserId.op_Equality(userId2, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
      {
        localIndex = index;
        break;
      }
    }
    int memberCount = Math.Max(members.Count, 1);
    this.UpdatePartyPreviewSlot(lobby.CharacterPreview, 0, localIndex, memberCount, members);
    this.UpdatePartyPreviewSlot(lobby.PartySlot2Preview, 1, localIndex, memberCount, members);
    this.UpdatePartyPreviewSlot(lobby.PartySlot3Preview, 2, localIndex, memberCount, members);
    this.UpdatePartyPreviewSlot(lobby.PartySlot4Preview, 3, localIndex, memberCount, members);
  }

  private void UpdatePartyPreviewSlot(
    LobbyCharacterPreviewPanel panel,
    int slotIndex,
    int localIndex,
    int memberCount,
    IReadOnlyList<PubgPreLobbyPartyMemberState> members)
  {
    if (slotIndex >= memberCount)
    {
      panel.ClearSprite();
      panel.SetSummaryText(string.Empty);
    }
    else
    {
      bool isLocalSlot = localIndex >= 0 && slotIndex == localIndex || localIndex < 0 && slotIndex == 0;
      HumanoidCharacterProfile humanoid;
      if (isLocalSlot)
      {
        HumanoidCharacterProfile characterProfile = this._preferencesManager.Preferences?.SelectedCharacter as HumanoidCharacterProfile;
        HumanoidCharacterProfile profile = slotIndex < members.Count ? members[slotIndex].Profile : (HumanoidCharacterProfile) null;
        if (characterProfile == null)
          characterProfile = profile;
        humanoid = characterProfile;
      }
      else
        humanoid = members.Count > slotIndex ? members[slotIndex].Profile : (HumanoidCharacterProfile) null;
      if (humanoid == null)
        humanoid = HumanoidCharacterProfile.DefaultWithSpecies();
      EntityUid entityUid = this.LoadProfileEntity(humanoid, (JobPrototype) null, false);
      Dictionary<string, string> outfitForPreviewSlot = this.GetOutfitForPreviewSlot(isLocalSlot, slotIndex, members);
      if (outfitForPreviewSlot.Count > 0)
        this.ApplySkinsToDummy(entityUid, outfitForPreviewSlot);
      panel.SetSprite(entityUid);
      panel.SetSummaryText(isLocalSlot ? humanoid?.Summary ?? string.Empty : string.Empty);
    }
  }

  private Dictionary<string, string> GetOutfitForPreviewSlot(
    bool isLocalSlot,
    int slotIndex,
    IReadOnlyList<PubgPreLobbyPartyMemberState> members)
  {
    if (slotIndex < members.Count && members[slotIndex].CurrentOutfit.Count > 0)
      return members[slotIndex].CurrentOutfit;
    return !isLocalSlot ? new Dictionary<string, string>() : this.GetLocalOutfitFromCaches();
  }

  private Dictionary<string, string> GetLocalOutfitFromCaches()
  {
    NetUserId? userId1 = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    if (userId1.HasValue)
    {
      foreach (PubgPreLobbyPartyMemberState member in (IEnumerable<PubgPreLobbyPartyMemberState>) partyClientSystem.Members)
      {
        NetUserId userId2 = member.UserId;
        NetUserId? nullable = userId1;
        if ((nullable.HasValue ? (NetUserId.op_Equality(userId2, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0 && member.CurrentOutfit.Count > 0)
          return member.CurrentOutfit;
      }
    }
    return this.UIManager.GetUIController<MainMenuUIController>().GetCachedCurrentOutfit();
  }

  private void RefreshProfileEditor()
  {
    this._profileEditor?.RefreshAntags();
    this._profileEditor?.RefreshJobs();
    this._profileEditor?.RefreshLoadouts();
    this._profileEditor?.RefreshRMC(this._linkAccount.Tier);
  }

  private void SaveProfile()
  {
    if (this.EditedProfile == null || !this.EditedSlot.HasValue || !this._preferencesManager.Preferences?.SelectedCharacterIndex.HasValue)
      return;
    this._preferencesManager.UpdateCharacter((ICharacterProfile) this.EditedProfile, this.EditedSlot.Value);
    this.ReloadCharacterSetup();
  }

  private void CloseProfileEditor()
  {
    if (this._pubgMode)
    {
      if (this._pubgProfileEditor == null)
        return;
      this._pubgProfileEditor.SetProfile((HumanoidCharacterProfile) null, new int?());
      this._pubgCharacterSetup.Visible = false;
    }
    else if (this._stateManager.CurrentState is CivLobbyState)
    {
      if (this._profileEditor == null)
        return;
      this._profileEditor.SetProfile((HumanoidCharacterProfile) null, new int?());
      if (this._civCharacterSetup != null)
        this._civCharacterSetup.Visible = false;
    }
    else
    {
      if (this._profileEditor == null)
        return;
      this._profileEditor.SetProfile((HumanoidCharacterProfile) null, new int?());
      ((Control) this._profileEditor).Visible = false;
    }
    switch (this._stateManager.CurrentState)
    {
      case LobbyState lobbyState:
        lobbyState.SwitchState(LobbyGui.LobbyGuiState.Default);
        break;
      case CivLobbyState civLobbyState:
        civLobbyState.SwitchState(CivLobbyGui.LobbyGuiState.Default);
        break;
    }
  }

  private void OpenSavePanel()
  {
    if (this._pubgMode)
    {
      PubgCharacterSetupGuiSavePanel pubgSavePanel = this._pubgSavePanel;
      if (pubgSavePanel != null && ((BaseWindow) pubgSavePanel).IsOpen)
        return;
      this._pubgSavePanel = new PubgCharacterSetupGuiSavePanel();
      ((BaseButton) this._pubgSavePanel.SaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.SaveProfile();
        ((BaseWindow) this._pubgSavePanel).Close();
        this.CloseProfileEditor();
      });
      ((BaseButton) this._pubgSavePanel.NoSaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        ((BaseWindow) this._pubgSavePanel).Close();
        this.CloseProfileEditor();
      });
      ((BaseWindow) this._pubgSavePanel).OpenCentered();
    }
    else
    {
      CharacterSetupGuiSavePanel savePanel = this._savePanel;
      if (savePanel != null && ((BaseWindow) savePanel).IsOpen)
        return;
      this._savePanel = new CharacterSetupGuiSavePanel();
      ((BaseButton) this._savePanel.SaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.SaveProfile();
        ((BaseWindow) this._savePanel).Close();
        this.CloseProfileEditor();
      });
      ((BaseButton) this._savePanel.NoSaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        ((BaseWindow) this._savePanel).Close();
        this.CloseProfileEditor();
      });
      ((BaseWindow) this._savePanel).OpenCentered();
    }
  }

  private (CharacterSetupGui, HumanoidProfileEditor) EnsureGui()
  {
    if (this._characterSetup != null && this._profileEditor != null)
    {
      this._characterSetup.Visible = true;
      ((Control) this._profileEditor).Visible = true;
      return (this._characterSetup, this._profileEditor);
    }
    this._profileEditor = this.CreateProfileEditor();
    this._profileEditor.OnOpenGuidebook += new Action<List<ProtoId<GuideEntryPrototype>>>(this._guide.OpenHelp);
    this._characterSetup = new CharacterSetupGui(this._profileEditor);
    ((BaseButton) this._characterSetup.CloseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (this._profileEditor.Profile != null && this._profileEditor.IsDirty)
        this.OpenSavePanel();
      else
        this.CloseProfileEditor();
    });
    this._profileEditor.Save += new Action(this.SaveProfile);
    this._characterSetup.SelectCharacter += (Action<int>) (args =>
    {
      this._preferencesManager.SelectCharacter(args);
      this.ReloadCharacterSetup();
    });
    this._characterSetup.DeleteCharacter += (Action<int>) (args =>
    {
      this._preferencesManager.DeleteCharacter(args);
      int? editedSlot = this.EditedSlot;
      int num = args;
      if (editedSlot.GetValueOrDefault() == num & editedSlot.HasValue)
        this.ReloadCharacterSetup();
      else
        this._characterSetup?.ReloadCharacterPickers();
    });
    this.GetCharacterSetupContainer()?.AddChild((Control) this._characterSetup);
    return (this._characterSetup, this._profileEditor);
  }

  private (CivCharacterSetupGui, HumanoidProfileEditor) EnsureCivGui()
  {
    if (this._civCharacterSetup != null && this._profileEditor != null)
    {
      this._civCharacterSetup.Visible = true;
      ((Control) this._profileEditor).Visible = true;
      this._profileEditor.UpdateCompactPermissions(this._lobbyPermissions);
      return (this._civCharacterSetup, this._profileEditor);
    }
    this._profileEditor = this.CreateProfileEditor();
    this._profileEditor.EnableCompactLayout();
    this._profileEditor.UpdateCompactPermissions(this._lobbyPermissions);
    this._profileEditor.OnOpenGuidebook += new Action<List<ProtoId<GuideEntryPrototype>>>(this._guide.OpenHelp);
    this._civCharacterSetup = new CivCharacterSetupGui(this._profileEditor);
    ((BaseButton) this._civCharacterSetup.CloseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (this._profileEditor.Profile != null && this._profileEditor.IsDirty)
        this.OpenSavePanel();
      else
        this.CloseProfileEditor();
    });
    this._profileEditor.Save += new Action(this.SaveProfile);
    this.GetCharacterSetupContainer()?.AddChild((Control) this._civCharacterSetup);
    return (this._civCharacterSetup, this._profileEditor);
  }

  private (PubgCharacterSetupGui, PubgHumanoidProfileEditor) EnsurePubgGui()
  {
    if (this._pubgCharacterSetup != null && this._pubgProfileEditor != null)
    {
      this._pubgCharacterSetup.Visible = true;
      this._pubgProfileEditor.UpdatePermissions(this._lobbyPermissions);
      return (this._pubgCharacterSetup, this._pubgProfileEditor);
    }
    this._pubgProfileEditor = new PubgHumanoidProfileEditor(this._markings);
    this._pubgProfileEditor.UpdatePermissions(this._lobbyPermissions);
    this._pubgCharacterSetup = new PubgCharacterSetupGui(this._pubgProfileEditor);
    ((BaseButton) this._pubgCharacterSetup.CloseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (this._pubgProfileEditor.Profile != null && this._pubgProfileEditor.IsDirty)
        this.OpenSavePanel();
      else
        this.CloseProfileEditor();
    });
    this._pubgProfileEditor.Save += new Action(this.SaveProfile);
    this.GetCharacterSetupContainer()?.AddChild((Control) this._pubgCharacterSetup);
    return (this._pubgCharacterSetup, this._pubgProfileEditor);
  }

  private HumanoidProfileEditor CreateProfileEditor()
  {
    return new HumanoidProfileEditor(this._preferencesManager, this._configurationManager, this.EntityManager, this._dialogManager, this.LogManager, this._playerManager, this._prototypeManager, (IResourceManager) this._resourceCache, this._requirements, this._markings);
  }

  private Control? GetCharacterSetupContainer()
  {
    Control characterSetupContainer;
    switch (this._stateManager.CurrentState)
    {
      case LobbyState lobbyState:
        characterSetupContainer = lobbyState.Lobby?.CharacterSetupState;
        break;
      case CivLobbyState civLobbyState:
        characterSetupContainer = civLobbyState.Lobby?.CharacterSetupState;
        break;
      default:
        characterSetupContainer = (Control) null;
        break;
    }
    return characterSetupContainer;
  }

  public void GiveDummyJobClothesLoadout(
    EntityUid dummy,
    JobPrototype? jobProto,
    HumanoidCharacterProfile profile)
  {
    JobPrototype job = jobProto ?? this.GetPreferredJob(profile);
    this.GiveDummyJobClothes(dummy, profile, job);
    if (!this._prototypeManager.HasIndex<RoleLoadoutPrototype>(LoadoutSystem.GetJobPrototype(job.ID)))
      return;
    RoleLoadout loadoutOrDefault = profile.GetLoadoutOrDefault(LoadoutSystem.GetJobPrototype(job.ID), ((ISharedPlayerManager) this._playerManager).LocalSession, new ProtoId<SpeciesPrototype>?(profile.Species), this.EntityManager, this._prototypeManager);
    this.GiveDummyLoadout(dummy, loadoutOrDefault);
  }

  public JobPrototype GetPreferredJob(HumanoidCharacterProfile profile)
  {
    return this._prototypeManager.Index<JobPrototype>(profile.JobPriorities.FirstOrDefault<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>((Func<KeyValuePair<ProtoId<JobPrototype>, JobPriority>, bool>) (p => p.Value == JobPriority.High)).Key.Id ?? ProtoId<JobPrototype>.op_Implicit(SharedGameTicker.FallbackOverflowJob));
  }

  public void GiveDummyLoadout(EntityUid uid, RoleLoadout? roleLoadout)
  {
    if (roleLoadout == null)
      return;
    foreach (List<Loadout> loadoutList in roleLoadout.SelectedLoadouts.Values)
    {
      foreach (Loadout loadout1 in loadoutList)
      {
        LoadoutPrototype loadout2;
        if (this._prototypeManager.TryIndex<LoadoutPrototype>(loadout1.Prototype, ref loadout2))
          this._spawn.EquipStartingGear(uid, loadout2);
      }
    }
  }

  public void GiveDummyJobClothes(
    EntityUid dummy,
    HumanoidCharacterProfile profile,
    JobPrototype job)
  {
    SlotDefinition[] slotDefinitions;
    if (!this._inventory.TryGetSlots(dummy, out slotDefinitions))
      return;
    RoleLoadout roleLoadout;
    if (profile.Loadouts.TryGetValue(job.ID, out roleLoadout))
    {
      foreach (List<Loadout> loadoutList in roleLoadout.SelectedLoadouts.Values)
      {
        foreach (Loadout loadout in loadoutList)
        {
          LoadoutPrototype loadoutPrototype;
          if (this._prototypeManager.TryIndex<LoadoutPrototype>(loadout.Prototype, ref loadoutPrototype))
          {
            foreach (SlotDefinition slotDefinition in slotDefinitions)
            {
              StartingGearPrototype startingGearPrototype;
              if (this._prototypeManager.TryIndex<StartingGearPrototype>(loadoutPrototype.StartingGear, ref startingGearPrototype))
              {
                string gear = startingGearPrototype.GetGear(slotDefinition.Name);
                EntityUid? removedItem;
                if (this._inventory.TryUnequip(dummy, slotDefinition.Name, out removedItem, true, true, reparent: false))
                  this.EntityManager.DeleteEntity(new EntityUid?(removedItem.Value));
                if (gear != string.Empty)
                {
                  EntityUid itemUid = this.EntityManager.SpawnEntity(gear, MapCoordinates.Nullspace, (ComponentRegistry) null);
                  this._inventory.TryEquip(dummy, itemUid, slotDefinition.Name, true, true);
                }
              }
              else
              {
                string gear = loadoutPrototype.GetGear(slotDefinition.Name);
                EntityUid? removedItem;
                if (this._inventory.TryUnequip(dummy, slotDefinition.Name, out removedItem, true, true, reparent: false))
                  this.EntityManager.DeleteEntity(new EntityUid?(removedItem.Value));
                if (gear != string.Empty)
                {
                  EntityUid itemUid = this.EntityManager.SpawnEntity(gear, MapCoordinates.Nullspace, (ComponentRegistry) null);
                  this._inventory.TryEquip(dummy, itemUid, slotDefinition.Name, true, true);
                }
              }
            }
          }
        }
      }
    }
    StartingGearPrototype startingGearPrototype1;
    if (!this._prototypeManager.TryIndex<StartingGearPrototype>(job.StartingGear, ref startingGearPrototype1))
      return;
    StartingGearPrototype startingGearPrototype2;
    this._prototypeManager.TryIndex<StartingGearPrototype>(job.DummyStartingGear, ref startingGearPrototype2);
    foreach (SlotDefinition slotDefinition in slotDefinitions)
    {
      string gear = startingGearPrototype1.GetGear(slotDefinition.Name);
      if (gear == string.Empty && startingGearPrototype2 != null)
        gear = startingGearPrototype2.GetGear(slotDefinition.Name);
      EntityUid? removedItem;
      if (this._inventory.TryUnequip(dummy, slotDefinition.Name, out removedItem, true, true, reparent: false))
        this.EntityManager.DeleteEntity(new EntityUid?(removedItem.Value));
      if (gear != string.Empty)
      {
        EntityUid itemUid1 = this.EntityManager.SpawnEntity(gear, MapCoordinates.Nullspace, (ComponentRegistry) null);
        RMCArmorVariantComponent variantComponent;
        if (this.EntityManager.TryGetComponent<RMCArmorVariantComponent>(itemUid1, ref variantComponent))
        {
          EntityUid itemUid2 = this.EntityManager.SpawnEntity(EntProtoId.op_Implicit(this._armorSystem.GetArmorVariant(Entity<RMCArmorVariantComponent>.op_Implicit((itemUid1, variantComponent)), profile.ArmorPreference)), MapCoordinates.Nullspace, (ComponentRegistry) null);
          this._inventory.TryEquip(dummy, itemUid2, slotDefinition.Name, true, true);
          this.EntityManager.QueueDeleteEntity(new EntityUid?(itemUid1));
        }
        else
          this._inventory.TryEquip(dummy, itemUid1, slotDefinition.Name, true, true);
      }
    }
  }

  public EntityUid LoadProfileEntity(
    HumanoidCharacterProfile? humanoid,
    JobPrototype? job,
    bool jobClothes)
  {
    EntProtoId? nullable1 = new EntProtoId?();
    if (humanoid != null & jobClothes)
    {
      if (job == null)
        job = this.GetPreferredJob(humanoid);
      nullable1 = job.JobPreviewEntity ?? EntProtoId.op_Implicit(job?.JobEntity);
    }
    if (nullable1.HasValue)
    {
      IEntityManager entityManager = this.EntityManager;
      EntProtoId? nullable2 = nullable1;
      string str = nullable2.HasValue ? EntProtoId.op_Implicit(nullable2.GetValueOrDefault()) : (string) null;
      MapCoordinates nullspace = MapCoordinates.Nullspace;
      return entityManager.SpawnEntity(str, nullspace, (ComponentRegistry) null);
    }
    EntityUid entityUid = humanoid == null ? this.EntityManager.SpawnEntity(EntProtoId.op_Implicit(this._prototypeManager.Index<SpeciesPrototype>(SharedHumanoidAppearanceSystem.DefaultSpecies).DollPrototype), MapCoordinates.Nullspace, (ComponentRegistry) null) : this.EntityManager.SpawnEntity(EntProtoId.op_Implicit(this._prototypeManager.Index<SpeciesPrototype>(humanoid.Species).DollPrototype), MapCoordinates.Nullspace, (ComponentRegistry) null);
    this._humanoid.LoadProfile(entityUid, humanoid, (HumanoidAppearanceComponent) null);
    if (humanoid != null & jobClothes)
    {
      this.GiveDummyJobClothes(entityUid, humanoid, job);
      if (this._prototypeManager.HasIndex<RoleLoadoutPrototype>(LoadoutSystem.GetJobPrototype(job.ID)))
      {
        RoleLoadout loadoutOrDefault = humanoid.GetLoadoutOrDefault(LoadoutSystem.GetJobPrototype(job.ID), ((ISharedPlayerManager) this._playerManager).LocalSession, new ProtoId<SpeciesPrototype>?(humanoid.Species), this.EntityManager, this._prototypeManager);
        this.GiveDummyLoadout(entityUid, loadoutOrDefault);
      }
    }
    return entityUid;
  }

  private void ApplySkinsToDummy(EntityUid dummy, Dictionary<string, string> currentOutfit)
  {
    SlotDefinition[] slotDefinitions;
    if (!this._inventory.TryGetSlots(dummy, out slotDefinitions))
      return;
    foreach (SlotDefinition slotDefinition in slotDefinitions)
    {
      EntityUid? removedItem;
      if (this._inventory.TryUnequip(dummy, slotDefinition.Name, out removedItem, true, true, reparent: false))
        this.EntityManager.DeleteEntity(new EntityUid?(removedItem.Value));
    }
    foreach ((string str1, string str2) in currentOutfit)
    {
      if (!string.IsNullOrEmpty(str2) && !(str1 == "equippedEmotes") && this._prototypeManager.HasIndex<EntityPrototype>(str2))
      {
        EntityUid itemUid = this.EntityManager.SpawnEntity(str2, MapCoordinates.Nullspace, (ComponentRegistry) null);
        this._inventory.TryEquip(dummy, itemUid, str1, true, true);
      }
    }
  }
}
