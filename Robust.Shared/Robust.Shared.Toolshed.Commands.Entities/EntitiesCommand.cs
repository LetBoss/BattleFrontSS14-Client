using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class EntitiesCommand : ToolshedCommand
{
	public sealed class AllEntityEnumerator(IEntityManager entMan) : IEnumerable<EntityUid>, IEnumerable
	{
		public EntityUid[]? _arr;

		public IEntityManager EntMan { get; } = entMan;

		public IEnumerator<EntityUid> GetEnumerator()
		{
			if (_arr == null)
			{
				_arr = EntMan.GetEntities().ToArray();
			}
			return ((IEnumerable<EntityUid>)_arr).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (_arr == null)
			{
				_arr = EntMan.GetEntities().ToArray();
			}
			return _arr.GetEnumerator();
		}
	}

	[CommandImplementation(null)]
	public IEnumerable<EntityUid> Entities()
	{
		return new AllEntityEnumerator(EntityManager);
	}
}
