// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.RichText.CheckTagHandler
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.RichText;

public sealed class CheckTagHandler : IMarkupTagHandler
{
  private static int _checkCounter = 0;

  public string Name => "check";

  public static float FontLineHeight { get; set; } = 16f;

  private static int GetCheckIndex(MarkupNode node) => CheckTagHandler._checkCounter++;

  public static void ResetCheckCounter() => CheckTagHandler._checkCounter = 0;

  private static int CountCheckButtonsBefore(Control clickedButton)
  {
    int count = 0;
    Control control = clickedButton;
    while (control.Parent != null)
      control = control.Parent;
    bool found = false;
    CheckTagHandler.CountCheckButtonsRecursive(control, clickedButton, ref count, ref found);
    return !found ? 0 : count;
  }

  private static void CountCheckButtonsRecursive(
    Control control,
    Control target,
    ref int count,
    ref bool found)
  {
    if (found)
      return;
    if (control is Button button && (button.Text == "☐" || button.Text == "✔" || button.Text == "✖"))
    {
      if (control == target)
      {
        found = true;
        return;
      }
      ++count;
    }
    foreach (Control child in control.Children)
      CheckTagHandler.CountCheckButtonsRecursive(child, target, ref count, ref found);
  }

  public void PushDrawContext(MarkupNode node, MarkupDrawingContext context)
  {
  }

  public void PopDrawContext(MarkupNode node, MarkupDrawingContext context)
  {
  }

  public string TextBefore(MarkupNode node) => "";

  public string TextAfter(MarkupNode node) => "";

  public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
  {
    Button button = new Button();
    button.Text = "☐";
    ((Control) button).MinSize = new Vector2(CheckTagHandler.FontLineHeight + 2f, CheckTagHandler.FontLineHeight + 2f);
    ((Control) button).MaxSize = new Vector2(CheckTagHandler.FontLineHeight + 2f, CheckTagHandler.FontLineHeight + 2f);
    ((Control) button).Margin = new Thickness(1f, 0.0f, 1f, 0.0f);
    ((Control) button).StyleClasses.Add("ButtonSquare");
    button.TextAlign = (Label.AlignMode) 1;
    Button btn = button;
    int checkIndex1 = CheckTagHandler.GetCheckIndex(node);
    ((Control) btn).Name = $"check_{checkIndex1}";
    ((BaseButton) btn).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Control parent = ((Control) btn).Parent;
      while (true)
      {
        switch (parent)
        {
          case null:
          case PaperWindow _:
            goto label_3;
          default:
            parent = parent.Parent;
            continue;
        }
      }
label_3:
      if (!(parent is PaperWindow paperWindow2))
        return;
      int checkIndex2 = CheckTagHandler.CountCheckButtonsBefore((Control) btn);
      paperWindow2.OpenCheckDialog(checkIndex2);
    });
    control = (Control) btn;
    return true;
  }

  private static string ReplaceNthCheckTag(string text, int index, string replacement)
  {
    int num = 0;
    int length;
    for (int startIndex = 0; startIndex < text.Length; startIndex = length + "[check]".Length)
    {
      length = text.IndexOf("[check]", startIndex);
      if (length != -1)
      {
        if (num == index)
          return text.Substring(0, length) + replacement + text.Substring(length + "[check]".Length);
        ++num;
      }
      else
        break;
    }
    return text;
  }
}
