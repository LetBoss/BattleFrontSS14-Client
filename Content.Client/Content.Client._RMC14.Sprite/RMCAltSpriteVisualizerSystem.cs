using System;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client._RMC14.Sprite;

public sealed class RMCAltSpriteVisualizerSystem : VisualizerSystem<RMCAlternateSpriteComponent>
{
	[Dependency]
	private IConfigurationManager _configuration;

	[Dependency]
	private IResourceCache _resourceCache;

	private bool _useAlternateSprites;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCAlternateSpriteComponent, ComponentStartup>((EntityEventRefHandler<RMCAlternateSpriteComponent, ComponentStartup>)OnInit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, RMCCVars.RMCUseAlternateSprites, (CVarChanged<bool>)OnAlternateSpriteChange, true);
	}

	private void OnAlternateSpriteChange(bool value, in CVarChangeInfo info)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		_useAlternateSprites = value;
		EntityQueryEnumerator<RMCAlternateSpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RMCAlternateSpriteComponent>();
		EntityUid item = default(EntityUid);
		RMCAlternateSpriteComponent item2 = default(RMCAlternateSpriteComponent);
		while (val.MoveNext(ref item, ref item2))
		{
			ChangeSprite(Entity<RMCAlternateSpriteComponent>.op_Implicit((item, item2)));
		}
	}

	private void OnInit(Entity<RMCAlternateSpriteComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ChangeSprite(Entity<RMCAlternateSpriteComponent>.op_Implicit((Entity<RMCAlternateSpriteComponent>.op_Implicit(ent), ent.Comp)));
	}

	protected override void OnAppearanceChange(EntityUid uid, RMCAlternateSpriteComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.OnAppearanceChange(uid, component, ref args);
		ChangeSprite(Entity<RMCAlternateSpriteComponent>.op_Implicit((uid, component)));
	}

	private void ChangeSprite(Entity<RMCAlternateSpriteComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCAlternateSpriteComponent>.op_Implicit(ent), ref val))
		{
			return;
		}
		string text = (_useAlternateSprites ? ent.Comp.AlternateSprite : ent.Comp.NormalSprite);
		RSIResource val2 = default(RSIResource);
		if (!_resourceCache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / text, ref val2))
		{
			((EntitySystem)this).Log.Error("Unable to load RSI '{0}'. Trace:\n{1}", new object[2]
			{
				text,
				Environment.StackTrace
			});
		}
		else if (val.BaseRSI != val2.RSI)
		{
			base.SpriteSystem.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), val2.RSI);
			for (int i = 0; i < val.AllLayers.Count(); i++)
			{
				base.SpriteSystem.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), i, 0f);
			}
		}
	}
}
