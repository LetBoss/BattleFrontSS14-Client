// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Sprite.RMCAltSpriteVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Sprite;

public sealed class RMCAltSpriteVisualizerSystem : VisualizerSystem<RMCAlternateSpriteComponent>
{
  [Dependency]
  private IConfigurationManager _configuration;
  [Dependency]
  private IResourceCache _resourceCache;
  private bool _useAlternateSprites;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<RMCAlternateSpriteComponent, ComponentStartup>(new EntityEventRefHandler<RMCAlternateSpriteComponent, ComponentStartup>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem) this).Subs, this._configuration, RMCCVars.RMCUseAlternateSprites, new CVarChanged<bool>((object) this, __methodptr(OnAlternateSpriteChange)), true);
  }

  private void OnAlternateSpriteChange(bool value, in CVarChangeInfo info)
  {
    this._useAlternateSprites = value;
    EntityQueryEnumerator<RMCAlternateSpriteComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<RMCAlternateSpriteComponent>();
    EntityUid entityUid;
    RMCAlternateSpriteComponent alternateSpriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref alternateSpriteComponent))
      this.ChangeSprite(Entity<RMCAlternateSpriteComponent>.op_Implicit((entityUid, alternateSpriteComponent)));
  }

  private void OnInit(Entity<RMCAlternateSpriteComponent> ent, ref ComponentStartup args)
  {
    this.ChangeSprite(Entity<RMCAlternateSpriteComponent>.op_Implicit((Entity<RMCAlternateSpriteComponent>.op_Implicit(ent), ent.Comp)));
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    RMCAlternateSpriteComponent component,
    ref AppearanceChangeEvent args)
  {
    base.OnAppearanceChange(uid, component, ref args);
    this.ChangeSprite(Entity<RMCAlternateSpriteComponent>.op_Implicit((uid, component)));
  }

  private void ChangeSprite(Entity<RMCAlternateSpriteComponent> ent)
  {
    SpriteComponent spriteComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(Entity<RMCAlternateSpriteComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    string str = this._useAlternateSprites ? ent.Comp.AlternateSprite : ent.Comp.NormalSprite;
    RSIResource rsiResource;
    if (!this._resourceCache.TryGetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, str), ref rsiResource))
    {
      ((EntitySystem) this).Log.Error("Unable to load RSI '{0}'. Trace:\n{1}", new object[2]
      {
        (object) str,
        (object) Environment.StackTrace
      });
    }
    else
    {
      if (spriteComponent.BaseRSI == rsiResource.RSI)
        return;
      this.SpriteSystem.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), rsiResource.RSI);
      for (int index = 0; index < spriteComponent.AllLayers.Count<ISpriteLayer>(); ++index)
        this.SpriteSystem.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), index, 0.0f);
    }
  }
}
