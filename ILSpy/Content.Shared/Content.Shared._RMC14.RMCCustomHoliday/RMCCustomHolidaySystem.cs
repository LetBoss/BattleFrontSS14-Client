using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.RMCCustomHoliday;

public sealed class RMCCustomHolidaySystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	public IEnumerable<CustomHolidayPrototype> GetCustomHolidays()
	{
		return _prototypeManager.EnumeratePrototypes<CustomHolidayPrototype>();
	}
}
