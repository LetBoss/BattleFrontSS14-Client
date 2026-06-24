namespace Content.Shared.Random;

public interface IBudgetEntry : IProbEntry
{
	float Cost { get; set; }

	string Proto { get; set; }
}
