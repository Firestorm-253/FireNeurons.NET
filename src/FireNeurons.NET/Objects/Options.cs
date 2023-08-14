namespace FireNeurons.NET.Objects;

public record Options
{
    public bool UseBias { get; init; } = true;
    public Activation Activation { get; init; } = Activation.Identity;
    public double Dropout { get; init; } = 0.0;
}
