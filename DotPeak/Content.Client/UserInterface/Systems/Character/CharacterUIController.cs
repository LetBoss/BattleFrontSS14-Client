// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Character.CharacterUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CharacterInfo;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Character.Controls;
using Content.Client.UserInterface.Systems.Character.Windows;
using Content.Client.UserInterface.Systems.Objectives.Controls;
using Content.Shared.Input;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Objectives;
using Content.Shared.Roles;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Systems.Character;

public sealed class CharacterUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<CharacterInfoSystem>,
  IOnSystemLoaded<CharacterInfoSystem>,
  IOnSystemUnloaded<CharacterInfoSystem>
{
  [Dependency]
  private IEntityManager _ent;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConsoleHost _consoleHost;
  [UISystemDependency]
  private readonly CharacterInfoSystem _characterInfo;
  [UISystemDependency]
  private readonly SpriteSystem _sprite;
  private CharacterWindow? _window;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<MindRoleTypeChangedEvent>(new EntitySessionEventHandler<MindRoleTypeChangedEvent>(this.OnRoleTypeChanged), (Type[]) null, (Type[]) null);
  }

  public void OnStateEntered(GameplayState state)
  {
    this._window = this.UIManager.CreateWindow<CharacterWindow>();
    LayoutContainer.SetAnchorPreset((Control) this._window, (LayoutContainer.LayoutPreset) 5, false);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCharacterMenu, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__8_0)), (StateInputCmdDelegate) null, true, true)).Register<CharacterUIController>();
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._window != null)
    {
      ((BaseWindow) this._window).Close();
      this._window = (CharacterWindow) null;
    }
    CommandBinds.Unregister<CharacterUIController>();
  }

  public void OnSystemLoaded(CharacterInfoSystem system)
  {
    system.OnCharacterUpdate += new Action<CharacterInfoSystem.CharacterData>(this.CharacterUpdated);
    this._player.LocalPlayerDetached += new Action<EntityUid>(this.CharacterDetached);
  }

  public void OnSystemUnloaded(CharacterInfoSystem system)
  {
    system.OnCharacterUpdate -= new Action<CharacterInfoSystem.CharacterData>(this.CharacterUpdated);
    this._player.LocalPlayerDetached -= new Action<EntityUid>(this.CharacterDetached);
  }

  public void UnloadButton()
  {
  }

  public void LoadButton()
  {
  }

  private void CharacterUpdated(CharacterInfoSystem.CharacterData data)
  {
    if (this._window == null)
      return;
    (EntityUid entityUid, string Job, Dictionary<string, List<ObjectiveInfo>> dictionary, string Briefing, string str3) = data;
    string str2 = str3;
    this._window.SpriteView.SetEntity(new EntityUid?(entityUid));
    this.UpdateRoleType();
    this._window.NameLabel.Text = str2;
    this._window.SubText.Text = Job;
    ((Control) this._window.Objectives).RemoveAllChildren();
    ((Control) this._window.ObjectivesLabel).Visible = dictionary.Any<KeyValuePair<string, List<ObjectiveInfo>>>();
    List<ObjectiveInfo> objectiveInfoList2;
    foreach ((str3, objectiveInfoList2) in dictionary)
    {
      string str4 = str3;
      List<ObjectiveInfo> objectiveInfoList3 = objectiveInfoList2;
      CharacterObjectiveControl objectiveControl1 = new CharacterObjectiveControl();
      objectiveControl1.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) objectiveControl1).Modulate = Color.Gray;
      CharacterObjectiveControl objectiveControl2 = objectiveControl1;
      FormattedMessage formattedMessage1 = new FormattedMessage();
      formattedMessage1.TryAddMarkup(str4, ref str3);
      RichTextLabel richTextLabel1 = new RichTextLabel();
      ((Control) richTextLabel1).StyleClasses.Add("tooltipActionTitle");
      RichTextLabel richTextLabel2 = richTextLabel1;
      richTextLabel2.SetMessage(formattedMessage1, new Color?());
      ((Control) objectiveControl2).AddChild((Control) richTextLabel2);
      foreach (ObjectiveInfo objectiveInfo in objectiveInfoList3)
      {
        ObjectiveConditionsControl conditionsControl = new ObjectiveConditionsControl();
        conditionsControl.ProgressTexture.Texture = this._sprite.Frame0(objectiveInfo.Icon);
        conditionsControl.ProgressTexture.Progress = objectiveInfo.Progress;
        FormattedMessage formattedMessage2 = new FormattedMessage();
        FormattedMessage formattedMessage3 = new FormattedMessage();
        formattedMessage2.AddText(objectiveInfo.Title);
        formattedMessage3.AddText(objectiveInfo.Description);
        conditionsControl.Title.SetMessage(formattedMessage2, new Color?());
        conditionsControl.Description.SetMessage(formattedMessage3, new Color?());
        ((Control) objectiveControl2).AddChild((Control) conditionsControl);
      }
      ((Control) this._window.Objectives).AddChild((Control) objectiveControl2);
    }
    if (Briefing != null)
    {
      ObjectiveBriefingControl objectiveBriefingControl = new ObjectiveBriefingControl();
      FormattedMessage formattedMessage = new FormattedMessage();
      formattedMessage.PushColor(Color.Yellow);
      formattedMessage.AddText(Briefing);
      objectiveBriefingControl.Label.SetMessage(formattedMessage, new Color?());
      ((Control) this._window.Objectives).AddChild((Control) objectiveBriefingControl);
    }
    List<Control> characterInfoControls = this._characterInfo.GetCharacterInfoControls(entityUid);
    foreach (Control control in characterInfoControls)
      ((Control) this._window.Objectives).AddChild(control);
    ((Control) this._window.RolePlaceholder).Visible = Briefing == null && !characterInfoControls.Any<Control>() && !dictionary.Any<KeyValuePair<string, List<ObjectiveInfo>>>();
  }

  private void OnRoleTypeChanged(MindRoleTypeChangedEvent ev, EntitySessionEventArgs _)
  {
    this.UpdateRoleType();
  }

  private void UpdateRoleType()
  {
    MindContainerComponent containerComponent;
    if (this._window == null || !((BaseWindow) this._window).IsOpen || !this._ent.TryGetComponent<MindContainerComponent>(((ISharedPlayerManager) this._player).LocalEntity, ref containerComponent))
      return;
    EntityUid? mind = containerComponent.Mind;
    if (!mind.HasValue)
      return;
    IEntityManager ent = this._ent;
    mind = containerComponent.Mind;
    EntityUid entityUid = mind.Value;
    MindComponent mindComponent;
    ref MindComponent local = ref mindComponent;
    if (!ent.TryGetComponent<MindComponent>(entityUid, ref local))
      return;
    RoleTypePrototype roleTypePrototype;
    if (!this._prototypeManager.TryIndex<RoleTypePrototype>(mindComponent.RoleType, ref roleTypePrototype))
      this.Log.Error($"Player '{((ISharedPlayerManager) this._player).LocalSession}' has invalid Role Type '{mindComponent.RoleType}'. Displaying default instead");
    this._window.RoleType.Text = Loc.GetString(LocId.op_Implicit(roleTypePrototype != null ? roleTypePrototype.Name : LocId.op_Implicit("role-type-crew-aligned-name")));
    this._window.RoleType.FontColorOverride = new Color?(roleTypePrototype != null ? roleTypePrototype.Color : Color.White);
  }

  private void CharacterDetached(EntityUid uid) => this.CloseWindow();

  private void CloseWindow() => ((BaseWindow) this._window)?.Close();

  private void ToggleWindow() => this._consoleHost.ExecuteCommand("skin");
}
