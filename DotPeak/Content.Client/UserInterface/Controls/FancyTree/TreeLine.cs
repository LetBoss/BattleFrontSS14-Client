// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.FancyTree.TreeLine
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.UserInterface.Controls.FancyTree;

public sealed class TreeLine : Control
{
  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    if (!(this.Parent is TreeItem parent) || !parent.Expanded || !parent.Tree.DrawLines || ((Control) parent.Body).ChildCount == 0)
      return;
    int num1 = Math.Max(1, (int) ((double) parent.Tree.LineWidth * (double) this.UIScale));
    int num2 = num1 / 2;
    int num3 = num1 - num2;
    Vector2i globalPixelPosition = ((Control) parent).GlobalPixelPosition;
    int num4 = Vector2i.op_Subtraction(((Control) parent.Icon).GlobalPixelPosition, globalPixelPosition).X + ((Control) parent.Icon).PixelSize.X / 2;
    int num5 = Vector2i.op_Subtraction(((Control) parent.Button).GlobalPixelPosition, globalPixelPosition).Y + ((Control) parent.Button).PixelSize.Y;
    TreeItem child1 = (TreeItem) ((Control) parent.Body).GetChild(((Control) parent.Body).ChildCount - 1);
    int num6 = Vector2i.op_Subtraction(((Control) child1.Button).GlobalPixelPosition, globalPixelPosition).Y + ((Control) child1.Button).PixelSize.Y / 2;
    UIBox2i uiBox2i1;
    // ISSUE: explicit constructor call
    ((UIBox2i) ref uiBox2i1).\u002Ector(Vector2i.op_Implicit((num4 - num2, num5)), Vector2i.op_Implicit((num4 + num3, num6)));
    handle.DrawRect(UIBox2i.op_Implicit(uiBox2i1), parent.Tree.LineColor, true);
    int num7 = Math.Max(1, (int) (16.0 * (double) this.UIScale / 2.0));
    foreach (TreeItem child2 in ((Control) parent.Body).Children)
    {
      int num8 = Vector2i.op_Subtraction(((Control) child2.Button).GlobalPixelPosition, globalPixelPosition).Y + ((Control) child2.Button).PixelSize.Y / 2;
      UIBox2i uiBox2i2 = new UIBox2i(Vector2i.op_Implicit((num4 - num2, num8 - num2)), Vector2i.op_Implicit((num4 + num7, num8 + num3)));
      handle.DrawRect(UIBox2i.op_Implicit(uiBox2i2), parent.Tree.LineColor, true);
    }
  }
}
