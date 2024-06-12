namespace PrecisionCrop.Models
{
    public class DescriptiveData
    {
        public double ProductionMean { get; set; }
        public double ProductionMedian { get; set; }
        public double YieldMean { get; set; }
        public double YieldMedian { get; set; }
        public double FirstQuartile { get; set; }
        public double ThirdQuartile { get; set; }
        public double IQR { get; set; }
    }
}
