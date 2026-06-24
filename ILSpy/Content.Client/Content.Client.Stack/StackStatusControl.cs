using Content.Client.Message;
using Content.Shared.Stacks;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Stack;

public sealed class StackStatusControl : Control
{
	private readonly StackComponent _parent;

	private readonly RichTextLabel _label;

	public StackStatusControl(StackComponent parent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		_parent = parent;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		_label.SetMarkup(Loc.GetString("comp-stack-status", new(string, object)[1] { ("count", _parent.Count) }));
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_parent.UiUpdateNeeded)
		{
			_parent.UiUpdateNeeded = false;
			_label.SetMarkup(Loc.GetString("comp-stack-status", new(string, object)[1] { ("count", _parent.Count) }));
		}
	}
}
