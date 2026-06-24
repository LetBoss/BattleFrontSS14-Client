// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.RichText.SignatureTagHandler
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.RichText;

public sealed class SignatureTagHandler : IMarkupTagHandler
{
  private static int _signatureCounter = 0;

  public string Name => "signature";

  public static float FontLineHeight { get; set; } = 16f;

  private static int GetSignatureIndex(MarkupNode node) => SignatureTagHandler._signatureCounter++;

  public static void ResetSignatureCounter() => SignatureTagHandler._signatureCounter = 0;

  private static int CountSignatureButtonsBefore(Control clickedButton)
  {
    int count = 0;
    Control control = clickedButton;
    while (control.Parent != null)
      control = control.Parent;
    bool found = false;
    SignatureTagHandler.CountSignatureButtonsRecursive(control, clickedButton, ref count, ref found);
    return !found ? 0 : count;
  }

  private static void CountSignatureButtonsRecursive(
    Control control,
    Control target,
    ref int count,
    ref bool found)
  {
    if (found)
      return;
    if (control is Button button && button.Text == Loc.GetString("paper-signature-sign-button"))
    {
      if (control == target)
      {
        found = true;
        return;
      }
      ++count;
    }
    foreach (Control child in control.Children)
      SignatureTagHandler.CountSignatureButtonsRecursive(child, target, ref count, ref found);
  }

  public SignatureTagHandler() => IoCManager.InjectDependencies<SignatureTagHandler>(this);

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
    button.Text = "Sign";
    ((Control) button).MinSize = new Vector2(48f, SignatureTagHandler.FontLineHeight + 4f);
    ((Control) button).MaxSize = new Vector2(48f, SignatureTagHandler.FontLineHeight + 4f);
    ((Control) button).Margin = new Thickness(1f, 2f, 1f, 2f);
    ((Control) button).StyleClasses.Add("ButtonSquare");
    button.TextAlign = (Label.AlignMode) 1;
    Button btn = button;
    int signatureIndex1 = SignatureTagHandler.GetSignatureIndex(node);
    ((Control) btn).Name = $"signature_{signatureIndex1}";
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
      int signatureIndex2 = SignatureTagHandler.CountSignatureButtonsBefore((Control) btn);
      paperWindow2.SendSignatureRequest(signatureIndex2);
    });
    control = (Control) btn;
    return true;
  }
}
