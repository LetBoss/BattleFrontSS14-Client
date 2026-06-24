using System;
using Content.Shared._RMC14.Dialog;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Megaphone;

public sealed class RMCMegaphoneSystem : EntitySystem
{
	[Dependency]
	private DialogSystem _dialog;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCMegaphoneComponent, UseInHandEvent>((EntityEventRefHandler<RMCMegaphoneComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMegaphoneComponent, ExaminedEvent>((EntityEventRefHandler<RMCMegaphoneComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<RMCMegaphoneComponent> ent, ref UseInHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		MegaphoneInputEvent ev = new MegaphoneInputEvent(((EntitySystem)this).GetNetEntity(args.User, (MetaDataComponent)null));
		_dialog.OpenInput(args.User, base.Loc.GetString("rmc-megaphone-ui-text"), ev, largeInput: false, 150);
	}

	private void OnExamined(Entity<RMCMegaphoneComponent> ent, ref ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("rmc-megaphone-examine"));
	}
}
