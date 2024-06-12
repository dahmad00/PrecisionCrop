namespace PrecisionCrop.Models
{
    public class RegressionData
    {
        public string Equation { get; set; }
        public double Production { get; set; }
        public double ahat { get; set; }
        public double bhat { get; set; }
        public double Area { get; set; }

        public string getEquation()
        {
            return "Production = " + ahat + " + " + bhat + " * Area";
        }
    }
}

