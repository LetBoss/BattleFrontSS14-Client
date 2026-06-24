using System;
using Content.Shared.Examine;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class SealableSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SealableComponent, ExaminedEvent>((ComponentEventHandler<SealableComponent, ExaminedEvent>)OnExamined, (Type[])null, new Type[1] { typeof(OpenableSystem) });
		((EntitySystem)this).SubscribeLocalEvent<SealableComponent, OpenableOpenedEvent>((ComponentEventHandler<SealableComponent, OpenableOpenedEvent>)OnOpened, (Type[])null, (Type[])null);
	}

	private void OnExamined(EntityUid uid, SealableComponent comp, ExaminedEvent args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			string sealedText = (comp.Sealed ? base.Loc.GetString(LocId.op_Implicit(comp.ExamineTextSealed)) : base.Loc.GetString(LocId.op_Implicit(comp.ExamineTextUnsealed)));
			args.PushMarkup(sealedText);
		}
	}

	private void OnOpened(EntityUid uid, SealableComponent comp, OpenableOpenedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		comp.Sealed = false;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		UpdateAppearance(uid, comp);
	}

	public void UpdateAppearance(EntityUid uid, SealableComponent? comp = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SealableComponent>(uid, ref comp, true))
		{
			_appearance.SetData(uid, (Enum)SealableVisuals.Sealed, (object)comp.Sealed, appearance);
		}
	}

	public bool IsSealed(EntityUid uid, SealableComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SealableComponent>(uid, ref comp, false))
		{
			return false;
		}
		return comp.Sealed;
	}
}
