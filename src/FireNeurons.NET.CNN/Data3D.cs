namespace FireNeurons.NET.CNN;

[Serializable]
public readonly struct Data3D
{
    private readonly double[][][] data;
    public readonly int Width;
    public readonly int Height;
    public readonly int Depth;

    public Data3D(double[][][] data)
    {
        this.Width = data[0][0].Length;
        this.Height = data[0].Length;
        this.Depth = data.Length;

        this.data = data;
    }
    public Data3D(int width, int height, int depth)
    {
        this.Width = width;
        this.Height = height;
        this.Depth = depth;

        this.data = new double[depth][][];
        for (int z = 0; z < depth; z++)
        {
            this.data[z] = new double[height][];
            for (int y = 0; y < height; y++)
            {
                this.data[z][y] = new double[width];
                for (int x = 0; x < width; x++)
                {
                    this.data[z][y][x] = Randoms.HeRandom(this.Width * this.Height * this.Depth);
                }
            }
        }
    }

    public double[][] this[int depth]
    {
        get => this.data[depth];
        set => this.data[depth] = value;
    }
    public double[] this[int depth, int height]
    {
        get => this.data[depth][height];
        set => this.data[depth][height] = value;
    }
    public double this[int depth, int height, int width]
    {
        get => this.data[depth][height][width];
        set => this.data[depth][height][width] = value;
    }

    public double[] Flatten()
    {
        var output = new double[this.Depth * this.Height * this.Width];
        for (int z = 0; z < this.Depth; z++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    output[(z * (this.Height * this.Width)) + (y * this.Width) + x] = this[z, y, x];
                }
            }
        }
        return output;
    }
}
