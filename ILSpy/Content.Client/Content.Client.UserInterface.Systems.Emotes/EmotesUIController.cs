using System.Collections.Generic;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Input;
using Content.Shared.Speech;
using Content.Shared.Whitelist;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Emotes;

public sealed class EmotesUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	private SimpleRadialMenu? _menu;

	private static readonly Dictionary<EmoteCategory, (string Tooltip, SpriteSpecifier Sprite)> EmoteGroupingInfo = new Dictionary<EmoteCategory, (string, SpriteSpecifier)>
	{
		[EmoteCategory.General] = ("emote-menu-category-general", (SpriteSpecifier)new Texture(new ResPath("/Textures/Clothing/Head/Soft/mimesoft.rsi/icon.png"))),
		[EmoteCategory.Hands] = ("emote-menu-category-hands", (SpriteSpecifier)new Texture(new ResPath("/Textures/Clothing/Hands/Gloves/latex.rsi/icon.png"))),
		[EmoteCategory.Vocal] = ("emote-menu-category-vocal", (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Emotes/vocal.png")))
	};

	private MenuButton? EmotesButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.EmotesButton;

	public void OnStateEntered(GameplayState state)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenEmotesMenu, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleEmotesMenu(centered: false);
		}, (StateInputCmdDelegate)null, true, true)).Register<EmotesUIController>();
	}

	public void OnStateExited(GameplayState state)
	{
		CommandBinds.Unregister<EmotesUIController>();
	}

	private void ToggleEmotesMenu(bool centered)
	{
		if (_menu == null)
		{
			IEnumerable<EmotePrototype> emotePrototypes = _prototypeManager.EnumeratePrototypes<EmotePrototype>();
			IEnumerable<RadialMenuOption> models = ConvertToButtons(emotePrototypes);
			_menu = new SimpleRadialMenu();
			_menu.SetButtons(models);
			((BaseWindow)_menu).Open();
			((BaseWindow)_menu).OnClose += OnWindowClosed;
			((BaseWindow)_menu).OnOpen += OnWindowOpen;
			if (EmotesButton != null)
			{
				((BaseButton)EmotesButton).SetClickPressed(true);
			}
			if (centered)
			{
				((BaseWindow)_menu).OpenCentered();
			}
			else
			{
				_menu.OpenOverMouseScreenPosition();
			}
		}
		else
		{
			((BaseWindow)_menu).OnClose -= OnWindowClosed;
			((BaseWindow)_menu).OnOpen -= OnWindowOpen;
			if (EmotesButton != null)
			{
				((BaseButton)EmotesButton).SetClickPressed(false);
			}
			CloseMenu();
		}
	}

	public void UnloadButton()
	{
		if (EmotesButton != null)
		{
			((BaseButton)EmotesButton).OnPressed -= ActionButtonPressed;
		}
	}

	public void LoadButton()
	{
		if (EmotesButton != null)
		{
			((BaseButton)EmotesButton).OnPressed += ActionButtonPressed;
		}
	}

	private void ActionButtonPressed(ButtonEventArgs args)
	{
		ToggleEmotesMenu(centered: true);
	}

	private void OnWindowClosed()
	{
		if (EmotesButton != null)
		{
			((BaseButton)EmotesButton).Pressed = false;
		}
		CloseMenu();
	}

	private void OnWindowOpen()
	{
		if (EmotesButton != null)
		{
			((BaseButton)EmotesButton).Pressed = true;
		}
	}

	private void CloseMenu()
	{
		if (_menu != null)
		{
			((Control)_menu).Orphan();
			_menu = null;
		}
	}

	private IEnumerable<RadialMenuOption> ConvertToButtons(IEnumerable<EmotePrototype> emotePrototypes)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		EntityWhitelistSystem entitySystem = base.EntitySystemManager.GetEntitySystem<EntityWhitelistSystem>();
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		Dictionary<EmoteCategory, List<RadialMenuOption>> dictionary = new Dictionary<EmoteCategory, List<RadialMenuOption>>();
		SpeechComponent speechComponent = default(SpeechComponent);
		foreach (EmotePrototype emotePrototype in emotePrototypes)
		{
			if (emotePrototype.Category != EmoteCategory.Invalid && emotePrototype.Category != EmoteCategory.Invalid && emotePrototype.ChatTriggers.Count != 0 && val.HasValue && entitySystem.IsWhitelistPassOrNull(emotePrototype.Whitelist, val.Value) && !entitySystem.IsBlacklistPass(emotePrototype.Blacklist, val.Value) && (emotePrototype.Available || !base.EntityManager.TryGetComponent<SpeechComponent>(val.Value, ref speechComponent) || speechComponent.AllowedEmotes.Contains(ProtoId<EmotePrototype>.op_Implicit(emotePrototype.ID))))
			{
				if (!dictionary.TryGetValue(emotePrototype.Category, out var value))
				{
					value = new List<RadialMenuOption>();
					dictionary.Add(emotePrototype.Category, value);
				}
				RadialMenuActionOption<EmotePrototype> item = new RadialMenuActionOption<EmotePrototype>(HandleRadialButtonClick, emotePrototype)
				{
					Sprite = emotePrototype.Icon,
					ToolTip = Loc.GetString(emotePrototype.Name)
				};
				value.Add(item);
			}
		}
		RadialMenuOption[] array = new RadialMenuOption[dictionary.Count];
		int num = 0;
		foreach (KeyValuePair<EmoteCategory, List<RadialMenuOption>> item2 in dictionary)
		{
			item2.Deconstruct(out var key, out var value2);
			EmoteCategory key2 = key;
			List<RadialMenuOption> nested = value2;
			(string, SpriteSpecifier) tuple = EmoteGroupingInfo[key2];
			array[num] = new RadialMenuNestedLayerOption(nested)
			{
				Sprite = tuple.Item2,
				ToolTip = Loc.GetString(tuple.Item1)
			};
			num++;
		}
		return array;
	}

	private void HandleRadialButtonClick(EmotePrototype prototype)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_entityManager.RaisePredictiveEvent<PlayEmoteMessage>(new PlayEmoteMessage(ProtoId<EmotePrototype>.op_Implicit(prototype.ID)));
	}
}
