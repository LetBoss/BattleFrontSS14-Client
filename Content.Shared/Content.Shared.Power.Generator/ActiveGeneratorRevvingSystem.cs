using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Power.Generator;

public sealed class ActiveGeneratorRevvingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActiveGeneratorRevvingComponent, AnchorStateChangedEvent>((ComponentEventHandler<ActiveGeneratorRevvingComponent, AnchorStateChangedEvent>)OnAnchorStateChanged, (Type[])null, (Type[])null);
	}

	private void OnAnchorStateChanged(EntityUid uid, ActiveGeneratorRevvingComponent component, AnchorStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((AnchorStateChangedEvent)(ref args)).Anchored)
		{
			StopAutoRevving(uid);
		}
	}

	public void StartAutoRevving(EntityUid uid, ActiveGeneratorRevvingComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActiveGeneratorRevvingComponent>(uid, ref component, false))
		{
			component.CurrentTime = TimeSpan.FromSeconds(0L);
		}
		else
		{
			((EntitySystem)this).AddComp<ActiveGeneratorRevvingComponent>(uid, new ActiveGeneratorRevvingComponent(), false);
		}
	}

	public bool StopAutoRevving(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).RemComp<ActiveGeneratorRevvingComponent>(uid);
	}

	private bool StartGenerator(EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AutoGeneratorStartedEvent ev = new AutoGeneratorStartedEvent();
		((EntitySystem)this).RaiseLocalEvent<AutoGeneratorStartedEvent>(uid, ref ev, false);
		return ev.Started;
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ActiveGeneratorRevvingComponent, PortableGeneratorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveGeneratorRevvingComponent, PortableGeneratorComponent>();
		EntityUid uid = default(EntityUid);
		ActiveGeneratorRevvingComponent activeGeneratorRevvingComponent = default(ActiveGeneratorRevvingComponent);
		PortableGeneratorComponent portableGeneratorComponent = default(PortableGeneratorComponent);
		while (query.MoveNext(ref uid, ref activeGeneratorRevvingComponent, ref portableGeneratorComponent))
		{
			activeGeneratorRevvingComponent.CurrentTime += TimeSpan.FromSeconds(frameTime);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)activeGeneratorRevvingComponent, (MetaDataComponent)null);
			if (!(activeGeneratorRevvingComponent.CurrentTime < portableGeneratorComponent.StartTime) && StartGenerator(uid))
			{
				StopAutoRevving(uid);
			}
		}
	}
}
