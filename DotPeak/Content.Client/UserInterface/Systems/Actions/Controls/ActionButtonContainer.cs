// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Actions.Controls.ActionButtonContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Systems.Actions.Controls;

[Virtual]
public class ActionButtonContainer : GridContainer
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IInputManager _input;

  public event Action<GUIBoundKeyEventArgs, ActionButton>? ActionPressed;

  public event Action<GUIBoundKeyEventArgs, ActionButton>? ActionUnpressed;

  public event Action<ActionButton>? ActionFocusExited;

  public ActionButtonContainer() => IoCManager.InjectDependencies<ActionButtonContainer>(this);

  public ActionButton this[int index] => (ActionButton) ((Control) this).GetChild(index);

  public void SetActionData(ActionsSystem system, params EntityUid?[] actionTypes)
  {
    int num = Math.Min(system.GetClientActions().Count<Entity<ActionComponent>>(), actionTypes.Length + 1);
    BoundKeyFunction[] keys = ContentKeyFunctions.GetHotbarBoundKeys();
    for (int index = 0; index < num; ++index)
    {
      if (index >= ((Control) this).ChildCount)
        ((Control) this).AddChild((Control) MakeButton(index));
      EntityUid? actionId;
      if (!Extensions.TryGetValue<EntityUid?>((IList<EntityUid?>) actionTypes, index, ref actionId))
        actionId = new EntityUid?();
      ((ActionButton) ((Control) this).GetChild(index)).UpdateData(actionId, system);
    }
    for (int index = ((Control) this).ChildCount - 1; index >= num; --index)
      ((Control) this).RemoveChild(((Control) this).GetChild(index));

    ActionButton MakeButton(int index)
    {
      ActionButton actionButton = new ActionButton(this._entity);
      BoundKeyFunction boundKeyFunction;
      if (!Extensions.TryGetValue<BoundKeyFunction>((IList<BoundKeyFunction>) keys, index, ref boundKeyFunction))
        return actionButton;
      actionButton.KeyBind = new BoundKeyFunction?(boundKeyFunction);
      IKeyBinding ikeyBinding;
      if (this._input.TryGetKeyBinding(boundKeyFunction, ref ikeyBinding))
        actionButton.Label.Text = ikeyBinding.GetKeyString();
      return actionButton;
    }
  }

  public void ClearActionData()
  {
    foreach (ActionButton child in ((Control) this).Children)
      child.ClearData();
  }

  protected virtual void ChildAdded(Control newChild)
  {
    ((Control) this).ChildAdded(newChild);
    if (!(newChild is ActionButton actionButton))
      return;
    actionButton.ActionPressed += this.ActionPressed;
    actionButton.ActionUnpressed += this.ActionUnpressed;
    actionButton.ActionFocusExited += this.ActionFocusExited;
  }

  protected virtual void ChildRemoved(Control newChild)
  {
    if (!(newChild is ActionButton actionButton))
      return;
    actionButton.ActionPressed -= this.ActionPressed;
    actionButton.ActionUnpressed -= this.ActionUnpressed;
    actionButton.ActionFocusExited -= this.ActionFocusExited;
  }

  public bool TryGetButtonIndex(ActionButton button, out int position)
  {
    if (button.Parent != this)
    {
      position = 0;
      return false;
    }
    position = button.GetPositionInParent();
    return true;
  }

  public IEnumerable<ActionButton> GetButtons()
  {
    foreach (Control child in ((Control) this).Children)
    {
      if (child is ActionButton button)
        yield return button;
    }
  }
}
