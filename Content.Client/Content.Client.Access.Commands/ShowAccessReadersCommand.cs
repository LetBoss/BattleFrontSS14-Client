using System;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Access.Commands;

public sealed class ShowAccessReadersCommand : LocalizedEntityCommands
{
	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IResourceCache _cache;

	[Dependency]
	private SharedTransformSystem _xform;

	public override string Command => "showaccessreaders";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		bool flag = _overlay.RemoveOverlay<AccessOverlay>();
		if (!flag)
		{
			_overlay.AddOverlay((Overlay)(object)new AccessOverlay((IEntityManager)(object)base.EntityManager, _cache, _xform));
		}
		shell.WriteLine(((LocalizedCommands)this).Loc.GetString("cmd-showaccessreaders-status", (ValueTuple<string, object>)("status", !flag)));
	}
}
