using System;
using Content.Shared.CCVar;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC;
using Content.Shared.SSDIndicator;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.SSDIndicator;

public sealed class SSDIndicatorSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private MobStateSystem _mobState;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SSDIndicatorComponent, GetStatusIconsEvent>((ComponentEventRefHandler<SSDIndicatorComponent, GetStatusIconsEvent>)OnGetStatusIcon, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIcon(EntityUid uid, SSDIndicatorComponent component, ref GetStatusIconsEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		MindContainerComponent mindContainerComponent = default(MindContainerComponent);
		if (component.IsSSD && _cfg.GetCVar<bool>(CCVars.ICShowSSDIndicator) && !_mobState.IsDead(uid) && !((EntitySystem)this).HasComp<ActiveNPCComponent>(uid) && ((EntitySystem)this).TryComp<MindContainerComponent>(uid, ref mindContainerComponent) && mindContainerComponent.ShowExamineInfo)
		{
			args.StatusIcons.Add(_prototype.Index<SsdIconPrototype>(component.Icon));
		}
	}
}
