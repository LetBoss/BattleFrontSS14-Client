using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls.FancyTree;

public sealed class TreeLine : Control
{
	protected unsafe override void Draw(DrawingHandleScreen handle)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Draw(handle);
		if (!(((Control)this).Parent is TreeItem { Expanded: not false } treeItem) || !treeItem.Tree.DrawLines || ((Control)treeItem.Body).ChildCount == 0)
		{
			return;
		}
		int num = Math.Max(1, (int)((float)treeItem.Tree.LineWidth * ((Control)this).UIScale));
		int num2 = num / 2;
		int num3 = num - num2;
		Vector2i globalPixelPosition = ((Control)treeItem).GlobalPixelPosition;
		Vector2i val = ((Control)treeItem.Icon).GlobalPixelPosition - globalPixelPosition;
		Vector2i pixelSize = ((Control)treeItem.Icon).PixelSize;
		int num4 = val.X + pixelSize.X / 2;
		Vector2i val2 = ((Control)treeItem.Button).GlobalPixelPosition - globalPixelPosition;
		Vector2i pixelSize2 = ((Control)treeItem.Button).PixelSize;
		int item = val2.Y + pixelSize2.Y;
		TreeItem treeItem2 = (TreeItem)(object)((Control)treeItem.Body).GetChild(((Control)treeItem.Body).ChildCount - 1);
		int item2 = (((Control)treeItem2.Button).GlobalPixelPosition - globalPixelPosition).Y + ((Control)treeItem2.Button).PixelSize.Y / 2;
		UIBox2i val3 = default(UIBox2i);
		((UIBox2i)(ref val3))._002Ector(Vector2i.op_Implicit((num4 - num2, item)), Vector2i.op_Implicit((num4 + num3, item2)));
		handle.DrawRect(UIBox2i.op_Implicit(val3), treeItem.Tree.LineColor, true);
		int num5 = Math.Max(1, (int)(16f * ((Control)this).UIScale / 2f));
		Enumerator enumerator = ((Control)treeItem.Body).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				TreeItem treeItem3 = (TreeItem)(object)((Enumerator)(ref enumerator)).Current;
				int num6 = (((Control)treeItem3.Button).GlobalPixelPosition - globalPixelPosition).Y + ((Control)treeItem3.Button).PixelSize.Y / 2;
				val3 = new UIBox2i(Vector2i.op_Implicit((num4 - num2, num6 - num2)), Vector2i.op_Implicit((num4 + num5, num6 + num3)));
				handle.DrawRect(UIBox2i.op_Implicit(val3), treeItem.Tree.LineColor, true);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
