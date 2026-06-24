using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Guidebook.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.ControlExtensions;

public static class ControlExtension
{
	public static List<T> GetControlOfType<T>(this Control parent) where T : Control
	{
		return parent.GetControlOfType<T>(typeof(T).Name, fullTreeSearch: false);
	}

	public static List<T> GetControlOfType<T>(this Control parent, string childType) where T : Control
	{
		return parent.GetControlOfType<T>(childType, fullTreeSearch: false);
	}

	public static List<T> GetControlOfType<T>(this Control parent, bool fullTreeSearch) where T : Control
	{
		return parent.GetControlOfType<T>(typeof(T).Name, fullTreeSearch);
	}

	public unsafe static List<T> GetControlOfType<T>(this Control parent, string childType, bool fullTreeSearch) where T : Control
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		List<T> list = new List<T>();
		Enumerator enumerator = parent.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				bool flag = ((object)current).GetType().Name == childType;
				bool flag2 = current.ChildCount > 0 && !flag;
				if (flag)
				{
					list.Add((T)(object)current);
				}
				if (fullTreeSearch || flag2)
				{
					list.AddRange(current.GetControlOfType<T>(childType, fullTreeSearch));
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public unsafe static List<ISearchableControl> GetSearchableControls(this Control parent, bool fullTreeSearch = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		List<ISearchableControl> list = new List<ISearchableControl>();
		Enumerator enumerator = parent.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				bool flag = current.ChildCount > 0 && !(current is ISearchableControl);
				if (current is ISearchableControl item)
				{
					list.Add(item);
				}
				if (fullTreeSearch || flag)
				{
					list.AddRange(current.GetSearchableControls(fullTreeSearch));
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static bool TryGetParentHandler<T>(this Control child, [NotNullWhen(true)] out T? result)
	{
		for (Control val = child; val != null; val = val.Parent)
		{
			if (val is T val2)
			{
				result = val2;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public static Vector2? GetControlScrollPosition(this Control child)
	{
		if (!child.VisibleInTree)
		{
			return null;
		}
		Vector2 value = default(Vector2);
		Control val = child;
		while (val != null && !(val.Parent is ScrollContainer))
		{
			value += val.Position;
			val = val.Parent;
		}
		return value;
	}

	public static bool ChildrenContainText(this Control parent, string search)
	{
		List<Label> controlOfType = parent.GetControlOfType<Label>();
		List<RichTextLabel> controlOfType2 = parent.GetControlOfType<RichTextLabel>();
		foreach (Label item in controlOfType)
		{
			if (item.Text != null && item.Text.Contains(search, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		foreach (RichTextLabel item2 in controlOfType2)
		{
			string message = item2.GetMessage();
			if (message != null && message.Contains(search, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
