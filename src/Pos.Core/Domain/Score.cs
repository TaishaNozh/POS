namespace Pos.Core.Domain;

public readonly record struct Score
{
    public int Value { get; }

    public Score(int value)
    {
        if (value < 0)
            throw new DomainException("Балл не может быть отрицательным");

        Value = value;
    }

    public override string ToString() => Value.ToString();
}
