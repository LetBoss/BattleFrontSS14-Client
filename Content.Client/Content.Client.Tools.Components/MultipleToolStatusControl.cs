using Content.Client.Message;
using Content.Shared.Tools.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.Tools.Components;

public sealed class MultipleToolStatusControl : Control
{
	private readonly MultipleToolComponent _parent;

	private readonly RichTextLabel _label;

	public MultipleToolStatusControl(MultipleToolComponent parent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		_parent = parent;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		_label.SetMarkup(_parent.StatusShowBehavior ? _parent.CurrentQualityName : string.Empty);
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_parent.UiUpdateNeeded)
		{
			_parent.UiUpdateNeeded = false;
			Update();
		}
	}

	public void Update()
	{
		_label.SetMarkup(_parent.StatusShowBehavior ? _parent.CurrentQualityName : string.Empty);
	}
}
