// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.RichText.FormTagHandler
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.RichText;

public sealed class FormTagHandler : IMarkupTagHandler
{
  private static int _formCounter = 0;
  private static readonly Dictionary<string, int> _formPositions = new Dictionary<string, int>();
  private static string _lastText = "";

  public string Name => "form";

  public bool CanHandle(MarkupNode node)
  {
    if (node.Name == "form")
      return true;
    string stringValue = ((MarkupParameter) ref node.Value).StringValue;
    return stringValue != null && stringValue.StartsWith("__FORM_");
  }

  public static float FontLineHeight { get; set; } = 16f;

  private static int GetFormIndex(MarkupNode node) => FormTagHandler._formCounter++;

  public static void ResetFormCounter() => FormTagHandler._formCounter = 0;

  private static int CountFormButtonsBefore(Control clickedButton)
  {
    int count = 0;
    Control control = clickedButton;
    while (control.Parent != null)
      control = control.Parent;
    bool found = false;
    FormTagHandler.CountFormButtonsRecursive(control, clickedButton, ref count, ref found);
    return !found ? 0 : count;
  }

  private static void CountFormButtonsRecursive(
    Control control,
    Control target,
    ref int count,
    ref bool found)
  {
    if (found)
      return;
    if (control is Button button && button.Text == Loc.GetString("paper-form-fill-button"))
    {
      if (control == target)
      {
        found = true;
        return;
      }
      ++count;
    }
    foreach (Control child in control.Children)
      FormTagHandler.CountFormButtonsRecursive(child, target, ref count, ref found);
  }

  public static void SetFormText(string text)
  {
    if (!(FormTagHandler._lastText != text))
      return;
    FormTagHandler._formPositions.Clear();
    FormTagHandler._lastText = text;
    int startIndex = 0;
    int num1 = 0;
    int num2;
    for (; (num2 = text.IndexOf("[form]", startIndex)) != -1; startIndex = num2 + 6)
      FormTagHandler._formPositions[num2.ToString()] = num1++;
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
    button.Text = "Fill";
    ((Control) button).MinSize = new Vector2(32f, FormTagHandler.FontLineHeight + 2f);
    ((Control) button).MaxSize = new Vector2(32f, FormTagHandler.FontLineHeight + 2f);
    ((Control) button).Margin = new Thickness(1f, 0.0f, 1f, 0.0f);
    ((Control) button).StyleClasses.Add("ButtonSquare");
    button.TextAlign = (Label.AlignMode) 1;
    Button btn = button;
    int formIndex1 = FormTagHandler.GetFormIndex(node);
    ((Control) btn).Name = $"form_{formIndex1}";
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
      int formIndex2 = FormTagHandler.CountFormButtonsBefore((Control) btn);
      paperWindow2.OpenFormDialog(formIndex2);
    });
    control = (Control) btn;
    return true;
  }
}
