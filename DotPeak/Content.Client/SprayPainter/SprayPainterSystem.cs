// Decompiled with JetBrains decompiler
// Type: Content.Client.SprayPainter.SprayPainterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.SprayPainter;
using Content.Shared.SprayPainter.Prototypes;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.SprayPainter;

public sealed class SprayPainterSystem : SharedSprayPainterSystem
{
  [Dependency]
  private IResourceCache _resourceCache;

  public List<SprayPainterEntry> Entries { get; private set; } = new List<SprayPainterEntry>();

  protected override void CacheStyles()
  {
    base.CacheStyles();
    this.Entries.Clear();
    foreach (AirlockStyle style in this.Styles)
    {
      string name = style.Name;
      List<AirlockGroupPrototype> all = this.Groups.FindAll((Predicate<AirlockGroupPrototype>) (x => x.StylePaths.ContainsKey(name)));
      string stylePath = all != null ? all.MaxBy<AirlockGroupPrototype, int>((Func<AirlockGroupPrototype, int>) (x => x.IconPriority))?.StylePaths[name] : (string) null;
      if (stylePath == null)
      {
        this.Entries.Add(new SprayPainterEntry(name, (Texture) null));
      }
      else
      {
        RSI.State state;
        if (!this._resourceCache.GetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, new ResPath(stylePath)), true).RSI.TryGetState(RSI.StateId.op_Implicit("closed"), ref state))
          this.Entries.Add(new SprayPainterEntry(name, (Texture) null));
        else
          this.Entries.Add(new SprayPainterEntry(name, state.Frame0));
      }
    }
  }
}
