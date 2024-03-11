using FireMath.NET.Activations;

namespace FireNeurons.NET.CNN;

public readonly struct ConvolutionLayer : CNN_Layer
{
    private readonly (Data3D Weights, double Bias)[] kernels;
    private readonly int stride;

    public ConvolutionLayer(int kernels, int kernelSize, int kernelDepth, int stride)
    {
        this.kernels = new (Data3D Weights, double Bias)[kernels];
        for (int i = 0; i < kernels; i++)
        {
            this.kernels[i] = (new Data3D(kernelSize, kernelSize, kernelDepth), Randoms.HeRandom(kernelSize * kernelSize * kernelDepth));
        }

        this.stride = stride;
    }

    public Data3D Execute(Data3D input)
    {
        int feature_shrink = (int)(this.kernels[0].Weights.Width / 2) * 2;
        var output = new Data3D((input.Width / this.stride) - feature_shrink, (input.Height / this.stride) - feature_shrink, this.kernels.Length);

        for (int z = 0; z < output.Depth; z++)
        {
            for (int y = 0; y < output.Height; y++)
            {
                for (int x = 0; x < output.Width; x++)
                {
                    output[z, y, x] = Activations.LeakyRelu(this.ConvolvePixel(input, x * this.stride, y * this.stride, z) + this.kernels[z].Bias);
                }
            }
        }

        return output;
    }

    private double ConvolvePixel(Data3D input, int x, int y, int z)
    {
        double sum = 0;
        for (int _z = 0; _z < this.kernels[z].Weights.Depth; _z++)
        {
            for (int _y = 0; _y < this.kernels[z].Weights.Height; _y++)
            {
                for (int _x = 0; _x < this.kernels[z].Weights.Width; _x++)
                {
                    sum += input[_z, y + _y, x + _x] * this.kernels[z].Weights[_z, _y, _x];
                }
            }
        }
        return sum;
    }
}
