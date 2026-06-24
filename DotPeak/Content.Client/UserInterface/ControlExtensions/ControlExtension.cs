// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.ControlExtensions.ControlExtension
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Guidebook.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.ControlExtensions;

public static class ControlExtension
{
  public static List<T> GetControlOfType<T>(this Control parent) where T : Control
  {
    return parent.GetControlOfType<T>(typeof (T).Name, false);
  }

  public static List<T> GetControlOfType<T>(this Control parent, string childType) where T : Control
  {
    return parent.GetControlOfType<T>(childType, false);
  }

  public static List<T> GetControlOfType<T>(this Control parent, bool fullTreeSearch) where T : Control
  {
    return parent.GetControlOfType<T>(typeof (T).Name, fullTreeSearch);
  }

  public static List<T> GetControlOfType<T>(
    this Control parent,
    string childType,
    bool fullTreeSearch)
    where T : Control
  {
    List<T> controlOfType = new List<T>();
    foreach (Control child in parent.Children)
    {
      bool flag1 = child.GetType().Name == childType;
      bool flag2 = child.ChildCount > 0 && !flag1;
      if (flag1)
        controlOfType.Add((T) child);
      if (fullTreeSearch | flag2)
        controlOfType.AddRange((IEnumerable<T>) child.GetControlOfType<T>(childType, fullTreeSearch));
    }
    return controlOfType;
  }

  public static List<ISearchableControl> GetSearchableControls(
    this Control parent,
    bool fullTreeSearch = false)
  {
    List<ISearchableControl> searchableControls = new List<ISearchableControl>();
    foreach (Control child in parent.Children)
    {
      bool flag = child.ChildCount > 0 && !(child is ISearchableControl);
      if (child is ISearchableControl searchableControl)
        searchableControls.Add(searchableControl);
      if (fullTreeSearch | flag)
        searchableControls.AddRange((IEnumerable<ISearchableControl>) child.GetSearchableControls(fullTreeSearch));
    }
    return searchableControls;
  }

  public static bool TryGetParentHandler<T>(this Control child, [NotNullWhen(true)] out T? result)
  {
    for (Control control = child; control != null; control = control.Parent)
    {
      if (control is T)
      {
        T obj = control as T;
        result = obj;
        return true;
      }
    }
    result = default (T);
    return false;
  }

  public static Vector2? GetControlScrollPosition(this Control child)
  {
    if (!child.VisibleInTree)
      return new Vector2?();
    Vector2 vector2 = new Vector2();
    for (Control control = child; control != null && !(control.Parent is ScrollContainer); control = control.Parent)
      vector2 += control.Position;
    return new Vector2?(vector2);
  }

  public static bool ChildrenContainText(this Control parent, string search)
  {
    List<Label> controlOfType1 = parent.GetControlOfType<Label>();
    List<RichTextLabel> controlOfType2 = parent.GetControlOfType<RichTextLabel>();
    foreach (Label label in controlOfType1)
    {
      if (label.Text != null && label.Text.Contains(search, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    foreach (RichTextLabel richTextLabel in controlOfType2)
    {
      string message = richTextLabel.GetMessage();
      if (message != null && message.Contains(search, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }
}
