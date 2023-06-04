namespace FireNeurons.NET.Optimizers;

using Objects;

public readonly struct TrainingData
{
    public Data InputData { get; }
    public Data LossDerivativeArgs { get; }

    public TrainingData()
    {
        this.InputData = new Data();
        this.LossDerivativeArgs = new Data();
    }
    public TrainingData(Data inputData, Data lossDerivativeArgs)
    {
        this.InputData = inputData;
        this.LossDerivativeArgs = lossDerivativeArgs;
    }
}
