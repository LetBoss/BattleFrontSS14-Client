using System;
using System.Numerics;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelFilterButton : ChatPopupButton<ChannelFilterPopup>
{
	private static readonly Color ColorNormal = Color.FromHex((ReadOnlySpan<char>)"#7b7e9e", (Color?)null);

	private static readonly Color ColorHovered = Color.FromHex((ReadOnlySpan<char>)"#9699bb", (Color?)null);

	private static readonly Color ColorPressed = Color.FromHex((ReadOnlySpan<char>)"#789B8C", (Color?)null);

	private readonly TextureRect? _textureRect;

	private readonly ChatUIController _chatUIController;

	private const int FilterDropdownOffset = 120;

	public ChannelFilterButton()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_004a: Expected O, but got Unknown
		_chatUIController = ((Control)this).UserInterfaceManager.GetUIController<ChatUIController>();
		Texture texture = IoCManager.Resolve<IResourceCache>().GetTexture("/Textures/Interface/Nano/filter.svg.96dpi.png");
		TextureRect val = new TextureRect
		{
			Texture = texture,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		TextureRect val2 = val;
		_textureRect = val;
		((Control)this).AddChild((Control)(object)val2);
		_chatUIController.FilterableChannelsChanged += Popup.SetChannels;
		_chatUIController.UnreadMessageCountsUpdated += Popup.UpdateUnread;
		Popup.SetChannels(_chatUIController.FilterableChannels);
	}

	protected override UIBox2 GetPopupPosition()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector2 globalPosition = ((Control)this).GlobalPosition;
		float num = default(float);
		float num2 = default(float);
		Vector2Helpers.Deconstruct(((Control)Popup).MinSize, ref num, ref num2);
		float val = num;
		float y = num2;
		return UIBox2.FromDimensions(globalPosition - new Vector2(120f, 0f), new Vector2(Math.Max(val, ((Control)Popup).MinWidth), y));
	}

	private void UpdateChildColors()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (_textureRect != null)
		{
			DrawModeEnum drawMode = ((BaseButton)this).DrawMode;
			switch ((int)drawMode)
			{
			case 0:
				((Control)_textureRect).ModulateSelfOverride = ColorNormal;
				break;
			case 1:
				((Control)_textureRect).ModulateSelfOverride = ColorPressed;
				break;
			case 2:
				((Control)_textureRect).ModulateSelfOverride = ColorHovered;
				break;
			case 3:
				break;
			}
		}
	}

	protected override void DrawModeChanged()
	{
		((ContainerButton)this).DrawModeChanged();
		UpdateChildColors();
	}

	protected override void StylePropertiesChanged()
	{
		((Button)this).StylePropertiesChanged();
		UpdateChildColors();
	}

	[Obsolete]
	protected override void Dispose(bool disposing)
	{
		((BaseButton)this).Dispose(disposing);
		if (disposing)
		{
			_chatUIController.FilterableChannelsChanged -= Popup.SetChannels;
			_chatUIController.UnreadMessageCountsUpdated -= Popup.UpdateUnread;
		}
	}
}
