using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Controls;

public static class PopupHelper
{
	public static Popup OpenContextPopup(IUserInterfaceManager uiManager, Control parent, Control content, Vector2 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Popup val = new Popup();
		((Control)val).AddChild(content);
		((Control)uiManager.ModalRoot).AddChild((Control)(object)val);
		Vector2i globalPixelPosition = parent.GlobalPixelPosition;
		Vector2i globalPixelPosition2 = ((Control)uiManager.ModalRoot).GlobalPixelPosition;
		Vector2 vector = Vector2i.op_Implicit(globalPixelPosition - globalPixelPosition2) + new Vector2(0f, parent.PixelHeight);
		val.Open((UIBox2?)UIBox2.FromDimensions(vector, size), (Vector2?)null, (Vector2?)null);
		return val;
	}
}
