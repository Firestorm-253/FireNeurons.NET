namespace FireNeurons.NET.Objects;

[Serializable]
public record Options
{
    public bool UseBias { get; init; } = true;
    public Activation Activation { get; init; } = Activation.Identity;
    public double Dropout { get; init; } = 0.0;
    public double L1_Reg { get; init; } = 0.0;
    public double L2_Reg { get; init; } = 0.0;
}
