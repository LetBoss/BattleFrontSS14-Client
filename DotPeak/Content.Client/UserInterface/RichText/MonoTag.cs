// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.RichText.MonoTag
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.UserInterface.RichText;

public sealed class MonoTag : IMarkupTag, IMarkupTagHandler
{
  public static readonly ProtoId<FontPrototype> MonoFont = ProtoId<FontPrototype>.op_Implicit("Monospace");
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public string Name => "mono";

  public void PushDrawContext(MarkupNode node, MarkupDrawingContext context)
  {
    Font font = FontTag.CreateFont(context.Font, node, this._resourceCache, this._prototypeManager, ProtoId<FontPrototype>.op_Implicit(MonoTag.MonoFont));
    context.Font.Push(font);
  }

  public void PopDrawContext(MarkupNode node, MarkupDrawingContext context) => context.Font.Pop();
}
