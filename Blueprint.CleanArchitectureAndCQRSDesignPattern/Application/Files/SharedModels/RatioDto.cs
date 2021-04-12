namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.SharedModels
{
    public struct RatioDto
    {
        public int? Width { set; get; }
        public int? Height { set; get; }

        public RatioDto(int? width, int? height)
        {
            Width = width;
            Height = height;
        }

        public static bool HasLength(int? @long)
        {
            return @long.HasValue && @long.Value > 0;
        }

        public void Deconstruct(out int? width, out int? height)
        {
            width = Width;
            height = Height;
        }
    }
}
