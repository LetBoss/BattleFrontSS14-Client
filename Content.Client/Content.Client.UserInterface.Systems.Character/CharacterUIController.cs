using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Content.Client.UserInterface.Systems.Character;

public sealed class CharacterUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<CharacterInfoSystem>, IOnSystemLoaded<CharacterInfoSystem>, IOnSystemUnloaded<CharacterInfoSystem>
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

	public override void Initialize()
	{
		((UIController)this).Initialize();
		((UIController)this).SubscribeNetworkEvent<MindRoleTypeChangedEvent>((EntitySessionEventHandler<MindRoleTypeChangedEvent>)OnRoleTypeChanged, (Type[])null, (Type[])null);
	}

	public void OnStateEntered(GameplayState state)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		_window = base.UIManager.CreateWindow<CharacterWindow>();
		LayoutContainer.SetAnchorPreset((Control)(object)_window, (LayoutPreset)5, false);
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCharacterMenu, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleWindow();
		}, (StateInputCmdDelegate)null, true, true)).Register<CharacterUIController>();
	}

	public void OnStateExited(GameplayState state)
	{
		if (_window != null)
		{
			((BaseWindow)_window).Close();
			_window = null;
		}
		CommandBinds.Unregister<CharacterUIController>();
	}

	public void OnSystemLoaded(CharacterInfoSystem system)
	{
		system.OnCharacterUpdate += CharacterUpdated;
		_player.LocalPlayerDetached += CharacterDetached;
	}

	public void OnSystemUnloaded(CharacterInfoSystem system)
	{
		system.OnCharacterUpdate -= CharacterUpdated;
		_player.LocalPlayerDetached -= CharacterDetached;
	}

	public void UnloadButton()
	{
	}

	public void LoadButton()
	{
	}

	private void CharacterUpdated(CharacterInfoSystem.CharacterData data)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		if (_window == null)
		{
			return;
		}
		CharacterInfoSystem.CharacterData characterData = data;
		characterData.Deconstruct(out EntityUid Entity, out string Job, out Dictionary<string, List<ObjectiveInfo>> Objectives, out string Briefing, out string EntityName);
		EntityUid val = Entity;
		string text = Job;
		Dictionary<string, List<ObjectiveInfo>> dictionary = Objectives;
		string text2 = Briefing;
		string text3 = EntityName;
		_window.SpriteView.SetEntity((EntityUid?)val);
		UpdateRoleType();
		_window.NameLabel.Text = text3;
		_window.SubText.Text = text;
		((Control)_window.Objectives).RemoveAllChildren();
		((Control)_window.ObjectivesLabel).Visible = dictionary.Any();
		foreach (KeyValuePair<string, List<ObjectiveInfo>> item in dictionary)
		{
			item.Deconstruct(out EntityName, out var value);
			string text4 = EntityName;
			List<ObjectiveInfo> list = value;
			CharacterObjectiveControl characterObjectiveControl = new CharacterObjectiveControl();
			((BoxContainer)characterObjectiveControl).Orientation = (LayoutOrientation)1;
			((Control)characterObjectiveControl).Modulate = Color.Gray;
			CharacterObjectiveControl characterObjectiveControl2 = characterObjectiveControl;
			FormattedMessage val2 = new FormattedMessage();
			val2.TryAddMarkup(text4, ref EntityName);
			RichTextLabel val3 = new RichTextLabel
			{
				StyleClasses = { "tooltipActionTitle" }
			};
			val3.SetMessage(val2, (Color?)null);
			((Control)characterObjectiveControl2).AddChild((Control)(object)val3);
			foreach (ObjectiveInfo item2 in list)
			{
				ObjectiveConditionsControl objectiveConditionsControl = new ObjectiveConditionsControl();
				((TextureRect)objectiveConditionsControl.ProgressTexture).Texture = _sprite.Frame0(item2.Icon);
				objectiveConditionsControl.ProgressTexture.Progress = item2.Progress;
				FormattedMessage val4 = new FormattedMessage();
				FormattedMessage val5 = new FormattedMessage();
				val4.AddText(item2.Title);
				val5.AddText(item2.Description);
				objectiveConditionsControl.Title.SetMessage(val4, (Color?)null);
				objectiveConditionsControl.Description.SetMessage(val5, (Color?)null);
				((Control)characterObjectiveControl2).AddChild((Control)(object)objectiveConditionsControl);
			}
			((Control)_window.Objectives).AddChild((Control)(object)characterObjectiveControl2);
		}
		if (text2 != null)
		{
			ObjectiveBriefingControl objectiveBriefingControl = new ObjectiveBriefingControl();
			FormattedMessage val6 = new FormattedMessage();
			val6.PushColor(Color.Yellow);
			val6.AddText(text2);
			objectiveBriefingControl.Label.SetMessage(val6, (Color?)null);
			((Control)_window.Objectives).AddChild((Control)(object)objectiveBriefingControl);
		}
		List<Control> characterInfoControls = _characterInfo.GetCharacterInfoControls(val);
		foreach (Control item3 in characterInfoControls)
		{
			((Control)_window.Objectives).AddChild(item3);
		}
		((Control)_window.RolePlaceholder).Visible = text2 == null && !characterInfoControls.Any() && !dictionary.Any();
	}

	private void OnRoleTypeChanged(MindRoleTypeChangedEvent ev, EntitySessionEventArgs _)
	{
		UpdateRoleType();
	}

	private void UpdateRoleType()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		MindContainerComponent mindContainerComponent = default(MindContainerComponent);
		MindComponent mindComponent = default(MindComponent);
		if (_window != null && ((BaseWindow)_window).IsOpen && _ent.TryGetComponent<MindContainerComponent>(((ISharedPlayerManager)_player).LocalEntity, ref mindContainerComponent) && mindContainerComponent.Mind.HasValue && _ent.TryGetComponent<MindComponent>(mindContainerComponent.Mind.Value, ref mindComponent))
		{
			RoleTypePrototype roleTypePrototype = default(RoleTypePrototype);
			if (!_prototypeManager.TryIndex<RoleTypePrototype>(mindComponent.RoleType, ref roleTypePrototype))
			{
				((UIController)this).Log.Error($"Player '{((ISharedPlayerManager)_player).LocalSession}' has invalid Role Type '{mindComponent.RoleType}'. Displaying default instead");
			}
			_window.RoleType.Text = Loc.GetString(LocId.op_Implicit(roleTypePrototype?.Name ?? LocId.op_Implicit("role-type-crew-aligned-name")));
			_window.RoleType.FontColorOverride = roleTypePrototype?.Color ?? Color.White;
		}
	}

	private void CharacterDetached(EntityUid uid)
	{
		CloseWindow();
	}

	private void CloseWindow()
	{
		CharacterWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	private void ToggleWindow()
	{
		_consoleHost.ExecuteCommand("skin");
	}
}
