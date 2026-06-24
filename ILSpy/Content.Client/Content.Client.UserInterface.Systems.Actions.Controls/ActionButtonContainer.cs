using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Actions;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Actions.Controls;

[Virtual]
public class ActionButtonContainer : GridContainer
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IInputManager _input;

	public ActionButton this[int index] => (ActionButton)(object)((Control)this).GetChild(index);

	public event Action<GUIBoundKeyEventArgs, ActionButton>? ActionPressed;

	public event Action<GUIBoundKeyEventArgs, ActionButton>? ActionUnpressed;

	public event Action<ActionButton>? ActionFocusExited;

	public ActionButtonContainer()
	{
		IoCManager.InjectDependencies<ActionButtonContainer>(this);
	}

	public void SetActionData(ActionsSystem system, params EntityUid?[] actionTypes)
	{
		int num = Math.Min(system.GetClientActions().Count(), actionTypes.Length + 1);
		BoundKeyFunction[] keys = ContentKeyFunctions.GetHotbarBoundKeys();
		EntityUid? actionId = default(EntityUid?);
		for (int i = 0; i < num; i++)
		{
			if (i >= ((Control)this).ChildCount)
			{
				((Control)this).AddChild((Control)(object)MakeButton(i));
			}
			if (!Extensions.TryGetValue<EntityUid?>((IList<EntityUid?>)actionTypes, i, ref actionId))
			{
				actionId = null;
			}
			((ActionButton)(object)((Control)this).GetChild(i)).UpdateData(actionId, system);
		}
		for (int num2 = ((Control)this).ChildCount - 1; num2 >= num; num2--)
		{
			((Control)this).RemoveChild(((Control)this).GetChild(num2));
		}
		ActionButton MakeButton(int index)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			ActionButton actionButton = new ActionButton(_entity);
			BoundKeyFunction val = default(BoundKeyFunction);
			if (!Extensions.TryGetValue<BoundKeyFunction>((IList<BoundKeyFunction>)keys, index, ref val))
			{
				return actionButton;
			}
			actionButton.KeyBind = val;
			IKeyBinding val2 = default(IKeyBinding);
			if (_input.TryGetKeyBinding(val, ref val2))
			{
				actionButton.Label.Text = val2.GetKeyString();
			}
			return actionButton;
		}
	}

	public unsafe void ClearActionData()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				((ActionButton)(object)((Enumerator)(ref enumerator)).Current).ClearData();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	protected override void ChildAdded(Control newChild)
	{
		((Control)this).ChildAdded(newChild);
		if (newChild is ActionButton actionButton)
		{
			actionButton.ActionPressed += this.ActionPressed;
			actionButton.ActionUnpressed += this.ActionUnpressed;
			actionButton.ActionFocusExited += this.ActionFocusExited;
		}
	}

	protected override void ChildRemoved(Control newChild)
	{
		if (newChild is ActionButton actionButton)
		{
			actionButton.ActionPressed -= this.ActionPressed;
			actionButton.ActionUnpressed -= this.ActionUnpressed;
			actionButton.ActionFocusExited -= this.ActionFocusExited;
		}
	}

	public bool TryGetButtonIndex(ActionButton button, out int position)
	{
		if ((object)((Control)button).Parent != this)
		{
			position = 0;
			return false;
		}
		position = ((Control)button).GetPositionInParent();
		return true;
	}

	public unsafe IEnumerable<ActionButton> GetButtons()
	{
		Enumerator val = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref val)).MoveNext())
			{
				if (((Enumerator)(ref val)).Current is ActionButton actionButton)
				{
					yield return actionButton;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
		val = default(Enumerator);
	}
}
