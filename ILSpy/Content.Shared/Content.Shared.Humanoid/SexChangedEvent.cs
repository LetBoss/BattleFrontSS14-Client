namespace Content.Shared.Humanoid;

public record struct SexChangedEvent(Sex OldSex, Sex NewSex);
