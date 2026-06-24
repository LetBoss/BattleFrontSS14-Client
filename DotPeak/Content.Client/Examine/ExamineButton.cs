// Decompiled with JetBrains decompiler
// Type: Content.Client.Examine.ExamineButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.Examine;

public sealed class ExamineButton : ContainerButton
{
  public const string StyleClassExamineButton = "examine-button";
  public const int ElementHeight = 32 /*0x20*/;
  public const int ElementWidth = 32 /*0x20*/;
  private const int Thickness = 4;
  public TextureRect Icon;
  public ExamineVerb Verb;
  private SpriteSystem _sprite;

  public ExamineButton(ExamineVerb verb, SpriteSystem spriteSystem)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ExamineButton.\u003C\u003Ec__DisplayClass7_0 cDisplayClass70 = new ExamineButton.\u003C\u003Ec__DisplayClass7_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass70.verb = verb;
    // ISSUE: explicit constructor call
    base.\u002Ector();
    ((Control) this).Margin = new Robust.Shared.Maths.Thickness(4f, 4f, 4f, 4f);
    ((Control) this).SetOnlyStyleClass("examine-button");
    // ISSUE: reference to a compiler-generated field
    this.Verb = cDisplayClass70.verb;
    this._sprite = spriteSystem;
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass70.verb.Disabled)
      ((BaseButton) this).Disabled = true;
    // ISSUE: method pointer
    ((Control) this).TooltipSupplier = new TooltipSupplier((object) cDisplayClass70, __methodptr(\u003C\u002Ector\u003Eb__0));
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).SetWidth = 32f;
    ((Control) textureRect).SetHeight = 32f;
    this.Icon = textureRect;
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass70.verb.Icon == null)
      return;
    // ISSUE: reference to a compiler-generated field
    this.Icon.Texture = this._sprite.Frame0(cDisplayClass70.verb.Icon);
    this.Icon.Stretch = (TextureRect.StretchMode) 7;
    ((Control) this).AddChild((Control) this.Icon);
  }
}
