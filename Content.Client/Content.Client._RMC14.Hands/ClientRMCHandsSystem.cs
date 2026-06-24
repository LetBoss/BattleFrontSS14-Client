using System.Linq;
using Content.Client.Hands.Systems;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Input;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Hands;

public sealed class ClientRMCHandsSystem : RMCHandsSystem
{
	[Dependency]
	private HandsSystem _hands;

	public override void Initialize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		base.Initialize();
		CommandBinds.Builder.Bind(CMKeyFunctions.RMCInteractWithOtherHand, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				HandsComponent item = default(HandsComponent);
				string handName = default(string);
				if (((EntitySystem)this).TryComp<HandsComponent>(valueOrDefault, ref item) && Extensions.TryFirstOrDefault<string>(_hands.EnumerateHands(Entity<HandsComponent>.op_Implicit((valueOrDefault, item))).Skip(1), ref handName))
				{
					_hands.UIHandClick(Entity<HandsComponent>.op_Implicit((valueOrDefault, item)), handName, switchHand: false);
				}
			}
		}, (StateInputCmdDelegate)null, true, true)).Register<ClientRMCHandsSystem>();
	}
}
