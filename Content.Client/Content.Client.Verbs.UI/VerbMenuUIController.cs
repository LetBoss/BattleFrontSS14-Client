using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.CombatMode;
using Content.Client.ContextMenu.UI;
using Content.Client.Gameplay;
using Content.Client.Mapping;
using Content.Shared.Input;
using Content.Shared.Verbs;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client.Verbs.UI;

public sealed class VerbMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnStateEntered<MappingState>, IOnStateExited<MappingState>
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private ContextMenuUIController _context;

	[UISystemDependency]
	private readonly CombatModeSystem _combatMode;

	[UISystemDependency]
	private readonly VerbSystem _verbSystem;

	public NetEntity CurrentTarget;

	public SortedSet<Verb> CurrentVerbs = new SortedSet<Verb>();

	public List<VerbCategory> ExtraCategories = new List<VerbCategory>();

	public ContextMenuPopup? OpenMenu;

	public void OnStateEntered(GameplayState state)
	{
		ContextMenuUIController context = _context;
		context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Combine(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(OnKeyBindDown));
		ContextMenuUIController context2 = _context;
		context2.OnContextClosed = (Action)Delegate.Combine(context2.OnContextClosed, new Action(Close));
		VerbSystem verbSystem = _verbSystem;
		verbSystem.OnVerbsResponse = (Action<VerbsResponseEvent>)Delegate.Combine(verbSystem.OnVerbsResponse, new Action<VerbsResponseEvent>(HandleVerbsResponse));
	}

	public void OnStateExited(GameplayState state)
	{
		ContextMenuUIController context = _context;
		context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Remove(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(OnKeyBindDown));
		ContextMenuUIController context2 = _context;
		context2.OnContextClosed = (Action)Delegate.Remove(context2.OnContextClosed, new Action(Close));
		if (_verbSystem != null)
		{
			VerbSystem verbSystem = _verbSystem;
			verbSystem.OnVerbsResponse = (Action<VerbsResponseEvent>)Delegate.Remove(verbSystem.OnVerbsResponse, new Action<VerbsResponseEvent>(HandleVerbsResponse));
		}
		Close();
	}

	public void OnStateEntered(MappingState state)
	{
		ContextMenuUIController context = _context;
		context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Combine(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(OnKeyBindDown));
		ContextMenuUIController context2 = _context;
		context2.OnContextClosed = (Action)Delegate.Combine(context2.OnContextClosed, new Action(Close));
		VerbSystem verbSystem = _verbSystem;
		verbSystem.OnVerbsResponse = (Action<VerbsResponseEvent>)Delegate.Combine(verbSystem.OnVerbsResponse, new Action<VerbsResponseEvent>(HandleVerbsResponse));
	}

	public void OnStateExited(MappingState state)
	{
		ContextMenuUIController context = _context;
		context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Remove(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(OnKeyBindDown));
		ContextMenuUIController context2 = _context;
		context2.OnContextClosed = (Action)Delegate.Remove(context2.OnContextClosed, new Action(Close));
		if (_verbSystem != null)
		{
			VerbSystem verbSystem = _verbSystem;
			verbSystem.OnVerbsResponse = (Action<VerbsResponseEvent>)Delegate.Remove(verbSystem.OnVerbsResponse, new Action<VerbsResponseEvent>(HandleVerbsResponse));
		}
		Close();
	}

	public void OpenVerbMenu(EntityUid target, bool force = false, ContextMenuPopup? popup = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		OpenVerbMenu(base.EntityManager.GetNetEntity(target, (MetaDataComponent)null), force, popup);
	}

	public void OpenVerbMenu(NetEntity target, bool force = false, ContextMenuPopup? popup = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (((EntityUid)(ref valueOrDefault)).Valid && (force || !_combatMode.IsInCombatMode(valueOrDefault)))
		{
			Close();
			ContextMenuPopup contextMenuPopup = popup ?? _context.RootMenu;
			((Control)contextMenuPopup.MenuBody).DisposeAllChildren();
			CurrentTarget = target;
			CurrentVerbs = _verbSystem.GetVerbs(target, valueOrDefault, Verb.VerbTypes, out ExtraCategories, force);
			OpenMenu = contextMenuPopup;
			FillVerbPopup(contextMenuPopup);
			if (popup == null)
			{
				((Control)contextMenuPopup).SetPositionLast();
				UIBox2 value = UIBox2.FromDimensions(base.UIManager.MousePositionScaled.Position, new Vector2(1f, 1f));
				((Popup)contextMenuPopup).Open((UIBox2?)value, (Vector2?)null, (Vector2?)null);
			}
		}
	}

	private void FillVerbPopup(ContextMenuPopup popup)
	{
		HashSet<string> hashSet = new HashSet<string>();
		ValueList<string> val = default(ValueList<string>);
		val._002Ector(ExtraCategories.Count);
		foreach (VerbCategory extraCategory in ExtraCategories)
		{
			val.Add(extraCategory.Text);
		}
		foreach (Verb currentVerb in CurrentVerbs)
		{
			if (currentVerb.Category == null)
			{
				VerbMenuElement element = new VerbMenuElement(currentVerb);
				_context.AddElement(popup, element);
			}
			else if (!val.Contains(currentVerb.Category.Text) && hashSet.Add(currentVerb.Category.Text))
			{
				AddVerbCategory(currentVerb.Category, popup);
			}
		}
		foreach (VerbCategory extraCategory2 in ExtraCategories)
		{
			if (hashSet.Add(extraCategory2.Text))
			{
				AddVerbCategory(extraCategory2, popup);
			}
		}
		((Control)popup).InvalidateMeasure();
	}

	public void AddVerbCategory(VerbCategory category, ContextMenuPopup popup)
	{
		List<Verb> list = new List<Verb>();
		bool flag = false;
		foreach (Verb currentVerb in CurrentVerbs)
		{
			if (currentVerb.Category?.Text == category.Text)
			{
				list.Add(currentVerb);
				flag = flag || currentVerb.Icon != null || currentVerb.IconEntity.HasValue;
			}
		}
		if (list.Count == 0 && !ExtraCategories.Contains(category))
		{
			return;
		}
		string styleClass = list.FirstOrDefault()?.TextStyleClass ?? Verb.DefaultTextStyleClass;
		VerbMenuElement verbMenuElement = new VerbMenuElement(category, styleClass);
		_context.AddElement(popup, verbMenuElement);
		verbMenuElement.SubMenu = new ContextMenuPopup(_context, verbMenuElement);
		foreach (Verb item in list)
		{
			VerbMenuElement element = new VerbMenuElement(item)
			{
				IconVisible = flag,
				TextVisible = !category.IconsOnly
			};
			_context.AddElement(verbMenuElement.SubMenu, element);
		}
		verbMenuElement.SubMenu.MenuBody.Columns = category.Columns;
	}

	public void AddServerVerbs(List<Verb>? verbs, ContextMenuPopup popup)
	{
		((Control)popup.MenuBody).DisposeAllChildren();
		if (verbs == null)
		{
			_context.AddElement(popup, new ContextMenuElement(Loc.GetString("verb-system-null-server-response")));
			return;
		}
		CurrentVerbs.UnionWith(verbs);
		FillVerbPopup(popup);
	}

	public void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args).Function != EngineKeyFunctions.Use && ((BoundKeyEventArgs)args).Function != ContentKeyFunctions.ActivateItemInWorld)
		{
			return;
		}
		if (!(element is VerbMenuElement verbMenuElement))
		{
			if (element is ConfirmationMenuElement confirmationMenuElement)
			{
				((BoundKeyEventArgs)args).Handle();
				ExecuteVerb(confirmationMenuElement.Verb);
			}
			return;
		}
		((BoundKeyEventArgs)args).Handle();
		Verb verb = verbMenuElement.Verb;
		if (verb == null)
		{
			if (verbMenuElement.SubMenu == null || ((Control)verbMenuElement.SubMenu).ChildCount == 0)
			{
				return;
			}
			if (((Control)verbMenuElement.SubMenu.MenuBody).ChildCount != 1 || !(((IEnumerable<Control>)((Control)verbMenuElement.SubMenu.MenuBody).Children).First() is VerbMenuElement verbMenuElement2))
			{
				_context.OpenSubMenu(verbMenuElement);
				return;
			}
			verb = verbMenuElement2.Verb;
			if (verb == null)
			{
				return;
			}
		}
		if (!verb.ConfirmationPopup)
		{
			ExecuteVerb(verb);
			return;
		}
		if (verbMenuElement.SubMenu == null)
		{
			ConfirmationMenuElement element2 = new ConfirmationMenuElement(verb, "Confirm");
			verbMenuElement.SubMenu = new ContextMenuPopup(_context, verbMenuElement);
			_context.AddElement(verbMenuElement.SubMenu, element2);
		}
		_context.OpenSubMenu(verbMenuElement);
	}

	private void Close()
	{
		if (OpenMenu != null)
		{
			((Popup)OpenMenu).Close();
			OpenMenu = null;
		}
	}

	private void HandleVerbsResponse(VerbsResponseEvent msg)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (OpenMenu != null && ((Control)OpenMenu).Visible && !(CurrentTarget != msg.Entity))
		{
			AddServerVerbs(msg.Verbs, OpenMenu);
		}
	}

	private void ExecuteVerb(Verb verb)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.UIManager.ClickSound();
		_verbSystem.ExecuteVerb(CurrentTarget, verb);
		if (verb.CloseMenu ?? verb.CloseMenuDefault)
		{
			_context.Close();
		}
	}
}
