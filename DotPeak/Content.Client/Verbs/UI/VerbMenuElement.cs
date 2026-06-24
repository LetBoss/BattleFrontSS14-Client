// Decompiled with JetBrains decompiler
// Type: Content.Client.Verbs.UI.VerbMenuElement
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Verbs.UI;

public sealed class VerbMenuElement : ContextMenuElement
{
  public const string StyleClassVerbMenuConfirmationTexture = "verbMenuConfirmationTexture";
  public readonly Verb? Verb;

  public bool IconVisible
  {
    set => ((Control) this.Icon).Visible = value;
  }

  public bool TextVisible
  {
    set => ((Control) this.Label).Visible = value;
  }

  public VerbMenuElement(Verb verb)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    VerbMenuElement.\u003C\u003Ec__DisplayClass6_0 cDisplayClass60 = new VerbMenuElement.\u003C\u003Ec__DisplayClass6_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass60.verb = verb;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: explicit constructor call
    base.\u002Ector(cDisplayClass60.verb.Text);
    // ISSUE: method pointer
    ((Control) this).TooltipSupplier = new TooltipSupplier((object) cDisplayClass60, __methodptr(\u003C\u002Ector\u003Eb__0));
    // ISSUE: reference to a compiler-generated field
    ((BaseButton) this).Disabled = cDisplayClass60.verb.Disabled;
    // ISSUE: reference to a compiler-generated field
    this.Verb = cDisplayClass60.verb;
    // ISSUE: reference to a compiler-generated field
    ((Control) this.Label).SetOnlyStyleClass(cDisplayClass60.verb.TextStyleClass);
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass60.verb.ConfirmationPopup)
    {
      ((Control) this.ExpansionIndicator).SetOnlyStyleClass("verbMenuConfirmationTexture");
      ((Control) this.ExpansionIndicator).Visible = true;
    }
    IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass60.verb.Icon == null && cDisplayClass60.verb.IconEntity.HasValue)
    {
      SpriteView spriteView1 = new SpriteView();
      spriteView1.OverrideDirection = new Direction?((Direction) 0);
      ((Control) spriteView1).SetSize = new Vector2(32f, 32f);
      SpriteView spriteView2 = spriteView1;
      // ISSUE: reference to a compiler-generated field
      spriteView2.SetEntity(new EntityUid?(ientityManager.GetEntity(cDisplayClass60.verb.IconEntity.Value)));
      ((Control) this.Icon).AddChild((Control) spriteView2);
    }
    else
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ((Control) this.Icon).AddChild((Control) new TextureRect()
      {
        Texture = (cDisplayClass60.verb.Icon != null ? ientityManager.System<SpriteSystem>().Frame0(cDisplayClass60.verb.Icon) : (Texture) null),
        Stretch = (TextureRect.StretchMode) 7
      });
    }
  }

  public VerbMenuElement(VerbCategory category, string styleClass)
    : base(category.Text)
  {
    ((Control) this.Label).SetOnlyStyleClass(styleClass);
    ((Control) this.Icon).AddChild((Control) new TextureRect()
    {
      Texture = (category.Icon != null ? IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SpriteSystem>().Frame0(category.Icon) : (Texture) null),
      Stretch = (TextureRect.StretchMode) 7
    });
  }
}
