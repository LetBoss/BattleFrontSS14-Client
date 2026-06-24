// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Emotes.EmotesUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Emotes;

public sealed class EmotesUIController : 
  UIController,
  IOnStateChanged<GameplayState>,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  private SimpleRadialMenu? _menu;
  private static readonly Dictionary<EmoteCategory, (string Tooltip, SpriteSpecifier Sprite)> EmoteGroupingInfo = new Dictionary<EmoteCategory, (string, SpriteSpecifier)>()
  {
    [EmoteCategory.General] = ("emote-menu-category-general", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Clothing/Head/Soft/mimesoft.rsi/icon.png"))),
    [EmoteCategory.Hands] = ("emote-menu-category-hands", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Clothing/Hands/Gloves/latex.rsi/icon.png"))),
    [EmoteCategory.Vocal] = ("emote-menu-category-vocal", (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Emotes/vocal.png")))
  };

  private MenuButton? EmotesButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.EmotesButton;
  }

  public void OnStateEntered(GameplayState state)
  {
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenEmotesMenu, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__7_0)), (StateInputCmdDelegate) null, true, true)).Register<EmotesUIController>();
  }

  public void OnStateExited(GameplayState state) => CommandBinds.Unregister<EmotesUIController>();

  private void ToggleEmotesMenu(bool centered)
  {
    if (this._menu == null)
    {
      IEnumerable<RadialMenuOption> buttons = this.ConvertToButtons(this._prototypeManager.EnumeratePrototypes<EmotePrototype>());
      this._menu = new SimpleRadialMenu();
      this._menu.SetButtons(buttons);
      this._menu.Open();
      this._menu.OnClose += new Action(this.OnWindowClosed);
      this._menu.OnOpen += new Action(this.OnWindowOpen);
      if (this.EmotesButton != null)
        ((BaseButton) this.EmotesButton).SetClickPressed(true);
      if (centered)
        this._menu.OpenCentered();
      else
        this._menu.OpenOverMouseScreenPosition();
    }
    else
    {
      this._menu.OnClose -= new Action(this.OnWindowClosed);
      this._menu.OnOpen -= new Action(this.OnWindowOpen);
      if (this.EmotesButton != null)
        ((BaseButton) this.EmotesButton).SetClickPressed(false);
      this.CloseMenu();
    }
  }

  public void UnloadButton()
  {
    if (this.EmotesButton == null)
      return;
    ((BaseButton) this.EmotesButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.ActionButtonPressed);
  }

  public void LoadButton()
  {
    if (this.EmotesButton == null)
      return;
    ((BaseButton) this.EmotesButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.ActionButtonPressed);
  }

  private void ActionButtonPressed(BaseButton.ButtonEventArgs args) => this.ToggleEmotesMenu(true);

  private void OnWindowClosed()
  {
    if (this.EmotesButton != null)
      ((BaseButton) this.EmotesButton).Pressed = false;
    this.CloseMenu();
  }

  private void OnWindowOpen()
  {
    if (this.EmotesButton == null)
      return;
    ((BaseButton) this.EmotesButton).Pressed = true;
  }

  private void CloseMenu()
  {
    if (this._menu == null)
      return;
    ((Control) this._menu).Orphan();
    this._menu = (SimpleRadialMenu) null;
  }

  private IEnumerable<RadialMenuOption> ConvertToButtons(IEnumerable<EmotePrototype> emotePrototypes)
  {
    EntityWhitelistSystem entitySystem = this.EntitySystemManager.GetEntitySystem<EntityWhitelistSystem>();
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    Dictionary<EmoteCategory, List<RadialMenuOption>> dictionary = new Dictionary<EmoteCategory, List<RadialMenuOption>>();
    foreach (EmotePrototype emotePrototype in emotePrototypes)
    {
      SpeechComponent speechComponent;
      if (emotePrototype.Category != EmoteCategory.Invalid && emotePrototype.Category != EmoteCategory.Invalid && emotePrototype.ChatTriggers.Count != 0 && attachedEntity.HasValue && entitySystem.IsWhitelistPassOrNull(emotePrototype.Whitelist, attachedEntity.Value) && !entitySystem.IsBlacklistPass(emotePrototype.Blacklist, attachedEntity.Value) && (emotePrototype.Available || !this.EntityManager.TryGetComponent<SpeechComponent>(attachedEntity.Value, ref speechComponent) || speechComponent.AllowedEmotes.Contains(ProtoId<EmotePrototype>.op_Implicit(emotePrototype.ID))))
      {
        List<RadialMenuOption> radialMenuOptionList;
        if (!dictionary.TryGetValue(emotePrototype.Category, out radialMenuOptionList))
        {
          radialMenuOptionList = new List<RadialMenuOption>();
          dictionary.Add(emotePrototype.Category, radialMenuOptionList);
        }
        RadialMenuActionOption<EmotePrototype> menuActionOption1 = new RadialMenuActionOption<EmotePrototype>(new Action<EmotePrototype>(this.HandleRadialButtonClick), emotePrototype);
        menuActionOption1.Sprite = emotePrototype.Icon;
        menuActionOption1.ToolTip = Loc.GetString(emotePrototype.Name);
        RadialMenuActionOption<EmotePrototype> menuActionOption2 = menuActionOption1;
        radialMenuOptionList.Add((RadialMenuOption) menuActionOption2);
      }
    }
    RadialMenuOption[] buttons = new RadialMenuOption[dictionary.Count];
    int num = 0;
    foreach ((EmoteCategory key, List<RadialMenuOption> nested) in dictionary)
    {
      (string Tooltip, SpriteSpecifier Sprite) tuple = EmotesUIController.EmoteGroupingInfo[key];
      RadialMenuOption[] radialMenuOptionArray = buttons;
      int index = num;
      RadialMenuNestedLayerOption nestedLayerOption = new RadialMenuNestedLayerOption((IReadOnlyCollection<RadialMenuOption>) nested);
      nestedLayerOption.Sprite = tuple.Sprite;
      nestedLayerOption.ToolTip = Loc.GetString(tuple.Tooltip);
      radialMenuOptionArray[index] = (RadialMenuOption) nestedLayerOption;
      ++num;
    }
    return (IEnumerable<RadialMenuOption>) buttons;
  }

  private void HandleRadialButtonClick(EmotePrototype prototype)
  {
    this._entityManager.RaisePredictiveEvent<PlayEmoteMessage>(new PlayEmoteMessage(ProtoId<EmotePrototype>.op_Implicit(prototype.ID)));
  }
}
