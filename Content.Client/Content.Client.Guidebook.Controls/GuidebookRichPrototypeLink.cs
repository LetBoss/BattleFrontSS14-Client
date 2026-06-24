using System;
using Content.Client.Guidebook.RichText;
using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.Controls;

public sealed class GuidebookRichPrototypeLink : Control, IPrototypeLinkControl
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("guidebook.links");

	private bool _linkActive;

	private FormattedMessage? _message;

	private readonly RichTextLabel _richTextLabel;

	public IPrototype? LinkedPrototype { get; set; }

	public void EnablePrototypeLink()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (_message != null)
		{
			_linkActive = true;
			((Control)this).DefaultCursorShape = (CursorShape)3;
			_richTextLabel.SetMessage(_message, (Type[])null, (Color?)TextLinkTag.LinkColor);
		}
	}

	public GuidebookRichPrototypeLink()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		((Control)this).MouseFilter = (MouseFilterMode)1;
		((Control)this).OnKeyBindDown += HandleClick;
		_richTextLabel = new RichTextLabel();
		((Control)this).AddChild((Control)(object)_richTextLabel);
	}

	public void SetMessage(FormattedMessage message)
	{
		_message = message;
		_richTextLabel.SetMessage(_message, (Color?)null);
	}

	private void HandleClick(GUIBoundKeyEventArgs args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (_linkActive && !(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick))
		{
			if (((Control)(object)this).TryGetParentHandler<IAnchorClickHandler>(out IAnchorClickHandler result))
			{
				result.HandleAnchor(this);
				((BoundKeyEventArgs)args).Handle();
			}
			else
			{
				Sawmill.Warning("Warning! No valid IAnchorClickHandler found.");
			}
		}
	}
}
