using System;
using System.Numerics;
using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.UserInterface;

public static class UIExtensions
{
	public static FloatSpinBox CreateDialSpinBox(float value = 0f, Action<FloatSpinBoxEventArgs>? onValueChanged = null, bool buttons = true, int minWidth = 130)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		FloatSpinBox val = new FloatSpinBox(1f, (byte)0)
		{
			MinWidth = minWidth
		};
		val.Value = value;
		val.OnValueChanged += onValueChanged;
		if (!buttons)
		{
			foreach (Button item in ((Control)(object)val).GetControlOfType<Button>())
			{
				((Control)item).Visible = false;
			}
		}
		return val;
	}

	public static T CreatePopOutableWindow<T>(this BoundUserInterface bui) where T : RMCPopOutWindow, new()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		T val = BoundUserInterfaceExt.CreateDisposableControl<T>(bui);
		val.OnFinalClose += bui.Close;
		Vector2 vector = default(Vector2);
		if (((SharedUserInterfaceSystem)IoCManager.Resolve<IEntityManager>().System<UserInterfaceSystem>()).TryGetPosition(Entity<UserInterfaceComponent>.op_Implicit(bui.Owner), bui.UiKey, ref vector))
		{
			((BaseWindow)val).Open(vector);
		}
		else
		{
			((BaseWindow)val).OpenCentered();
		}
		return val;
	}

	public static void RemoveChildExcept(this Control parent, Control except)
	{
		for (int num = parent.ChildCount - 1; num >= 0; num--)
		{
			if (parent.GetChild(num) != except)
			{
				parent.RemoveChild(num);
			}
		}
	}

	public static void RemoveChildrenAfter(this Control parent, int after)
	{
		for (int num = parent.ChildCount - 1; num >= after; num--)
		{
			parent.RemoveChild(num);
		}
	}

	public static void SetTabVisibleAfter(this Control parent, int after, bool visible)
	{
		for (int num = parent.ChildCount - 1; num >= after; num--)
		{
			TabContainer.SetTabVisible(parent.GetChild(num), visible);
		}
	}

	public static void SetVisibleAfter(this Control parent, int after, bool visible)
	{
		for (int num = parent.ChildCount - 1; num >= after; num--)
		{
			parent.GetChild(num).Visible = visible;
		}
	}
}
