using Robust.Shared.GameObjects;

namespace Content.Shared.Fax.Components;

[ByRefEvent]
public record struct DamageOnFaxecuteEvent(FaxMachineComponent? Action);
