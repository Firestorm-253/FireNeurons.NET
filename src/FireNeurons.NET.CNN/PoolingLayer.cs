namespace FireNeurons.NET.CNN;

// Max Pooling
public readonly struct PoolingLayer : CNN_Layer
{
    private readonly int stride;

    public PoolingLayer(int stride)
    {
        this.stride = stride;
    }

    public Data3D Execute(Data3D input)
    {
        int width = (input.Width % this.stride == 0) ? input.Width / this.stride : Convert.ToInt32((input.Width / this.stride) + 0.5);
        int height = (input.Height % this.stride == 0) ? input.Height / this.stride : Convert.ToInt32((input.Height / this.stride) + 0.5);

        var output = new Data3D(width, height, input.Depth);

        for (int z = 0; z < output.Depth; z++)
        {
            for (int y = 0; y < output.Height; y++)
            {
                for (int x = 0; x < output.Width; x++)
                {
                    output[z, y, x] = this.PoolPixel(input, x * this.stride, y * this.stride, z);
                }
            }
        }

        return output;
    }

    private double PoolPixel(Data3D input, int x, int y, int z)
    {
        double max = int.MinValue;
        for (int _y = 0; _y < this.stride; _y++)
        {
            if (y + _y >= input.Height)
            {
                break;
            }

            for (int _x = 0; _x < this.stride; _x++)
            {
                if (x + _x >= input.Width)
                {
                    break;
                }
                
                max = input[z, y + _y, x + _x] > max ? input[z, y + _y, x + _x] : max;
            }
        }
        return max;
    }
}
