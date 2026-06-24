using System;
using System.Numerics;
using Content.Client.Gameplay;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Popups;

public sealed class PopupUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private sealed class PopupRootControl : Control
	{
		private readonly PopupSystem? _popup;

		private readonly PopupUIController _controller;

		public PopupRootControl(PopupSystem? system, PopupUIController controller)
		{
			_popup = system;
			_controller = controller;
		}

		protected override void Draw(DrawingHandleScreen handle)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			((Control)this).Draw(handle);
			if (_popup == null)
			{
				return;
			}
			WindowId id = ((UIRoot)((Control)this).UserInterfaceManager.RootControl).Window.Id;
			foreach (PopupSystem.CursorPopupLabel cursorLabel in _popup.CursorLabels)
			{
				if (!(cursorLabel.InitialPos.Window != id))
				{
					_controller.DrawPopup(cursorLabel, handle, cursorLabel.InitialPos.Position, ((Control)this).UIScale);
				}
			}
		}
	}

	[UISystemDependency]
	private readonly PopupSystem? _popup;

	private Font _smallFont;

	private Font _mediumFont;

	private Font _largeFont;

	private PopupRootControl? _popupControl;

	public override void Initialize()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		((UIController)this).Initialize();
		IResourceCache val = IoCManager.Resolve<IResourceCache>();
		_smallFont = (Font)new VectorFont(val.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Italic.ttf", true), 10);
		_mediumFont = (Font)new VectorFont(val.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Italic.ttf", true), 12);
		_largeFont = (Font)new VectorFont(val.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-BoldItalic.ttf", true), 14);
	}

	public void OnStateEntered(GameplayState state)
	{
		_popupControl = new PopupRootControl(_popup, this);
		((Control)base.UIManager.RootControl).AddChild((Control)(object)_popupControl);
	}

	public void OnStateExited(GameplayState state)
	{
		if (_popupControl != null)
		{
			((Control)base.UIManager.RootControl).RemoveChild((Control)(object)_popupControl);
			_popupControl = null;
		}
	}

	public void DrawPopup(PopupSystem.PopupLabel popup, DrawingHandleScreen handle, Vector2 position, float scale)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		float popupLifetime = PopupSystem.GetPopupLifetime(popup);
		float num = MathF.Min(1f, 1f - MathF.Max(0f, popup.TotalTime - popupLifetime / 2f) * 2f / popupLifetime);
		Vector2 vector = position - new Vector2(0f, MathF.Min(8f, 12f * (popup.TotalTime * popup.TotalTime + popup.TotalTime)));
		Font val = _smallFont;
		Color white = Color.White;
		Color val2 = ((Color)(ref white)).WithAlpha(num);
		switch (popup.Type)
		{
		case PopupType.SmallCaution:
			val2 = Color.Red;
			break;
		case PopupType.Medium:
			val = _mediumFont;
			val2 = Color.LightGray;
			break;
		case PopupType.MediumCaution:
			val = _mediumFont;
			val2 = Color.Red;
			break;
		case PopupType.Large:
			val = _largeFont;
			val2 = Color.LightGray;
			break;
		case PopupType.LargeCaution:
			val = _largeFont;
			val2 = Color.Red;
			break;
		case PopupType.MediumXeno:
			val = _largeFont;
			val2 = Color.FromHex((ReadOnlySpan<char>)"#C400FF", (Color?)null);
			break;
		}
		Vector2 dimensions = handle.GetDimensions(val, (ReadOnlySpan<char>)popup.Text, scale);
		handle.DrawString(val, vector - dimensions / 2f, (ReadOnlySpan<char>)popup.Text, scale, ((Color)(ref val2)).WithAlpha(num));
	}
}
