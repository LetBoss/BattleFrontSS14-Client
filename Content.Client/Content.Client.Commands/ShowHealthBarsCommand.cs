using System;
using System.Linq;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Overlays;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Commands;

public sealed class ShowHealthBarsCommand : LocalizedEntityCommands
{
	public override string Command => "showhealthbars";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession player = shell.Player;
		if (player == null)
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("shell-only-players-can-run-this-command"));
			return;
		}
		EntityUid? attachedEntity = player.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
			if (!base.EntityManager.HasComponent<ShowHealthBarsComponent>(valueOrDefault))
			{
				ShowHealthBarsComponent obj = new ShowHealthBarsComponent
				{
					DamageContainers = args.Select((string arg) => new ProtoId<DamageContainerPrototype>(arg)).ToList(),
					HealthStatusIcon = null
				};
				((Component)obj).NetSyncEnabled = false;
				ShowHealthBarsComponent showHealthBarsComponent = obj;
				base.EntityManager.AddComponent<ShowHealthBarsComponent>(valueOrDefault, showHealthBarsComponent, true, (MetaDataComponent)null);
				shell.WriteLine(((LocalizedCommands)this).Loc.GetString("cmd-showhealthbars-notify-enabled", (ValueTuple<string, object>)("args", string.Join(", ", args))));
			}
			else
			{
				base.EntityManager.RemoveComponentDeferred<ShowHealthBarsComponent>(valueOrDefault);
				shell.WriteLine(((LocalizedCommands)this).Loc.GetString("cmd-showhealthbars-notify-disabled"));
			}
		}
		else
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("shell-must-be-attached-to-entity"));
		}
	}
}
