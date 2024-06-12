using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PrecisionCrop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RDotNet;

namespace PrecisionCrop.Controllers
{
    public class HomeController : Controller
    {
        private const string dataFilePath = "Data.csv";
        private const string boxSeasonPath = "boxplot_season.png";
        private const string boxYearPath = "boxplot_year.png";
        private const string boxYieldPath = "boxplot_yield.png";
        private const string histogramProductionPath = "histogram_Production.png";
        private const string histogramYieldPath = "histogram_Yield.png";
        private const string tablePath = "table.png";
        private const string regressionImagePath = "Regression.png";
        private const string updatedDataFilePath = "Data3 (Nuts).csv";
        private const string myPlotImagePath = "myplot5.png";
        private const string imagePath = "myplot5.png";
        private const string RPath = "C:\\Program Files\\R\\R-4.2.2\\bin\\x64\\R.dll";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {


            _logger = logger;

        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult BoxPlot()
        {

            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();


            //path vectors
            CharacterVector vectorDataPath = engine.CreateCharacterVector(new[] { dataFilePath });
            CharacterVector vectorBoxYear = engine.CreateCharacterVector(new[] { boxYearPath});
            CharacterVector vectorBoxYield = engine.CreateCharacterVector(new[] { boxYieldPath});
            CharacterVector vectorBoxSeason = engine.CreateCharacterVector(new[] { boxSeasonPath });

            //symbols
            engine.SetSymbol("dataFile", vectorDataPath);
            engine.SetSymbol("boxYearImage", vectorBoxYear);
            engine.SetSymbol("boxYieldImage", vectorBoxYield);
            engine.SetSymbol("boxSeasonImage", vectorBoxSeason);

            //plotting - read data
            engine.Evaluate("CropProd<-read.csv(dataFile)");
            engine.Evaluate("CropProd_ProdSort=CropProd[order(CropProd$Production),]");
            engine.Evaluate("CropProd_YieldSort=CropProd[order(CropProd$Yield),]");

            //Boxplot 1
            engine.Evaluate("png(filename=boxSeasonImage)");//image to plot to
            engine.Evaluate("boxplot(CropProd_ProdSort$Production ~CropProd_ProdSort$Season, main = \"BOXPLOT\", ylab = \"Production\", xlab = \"Season\")");
            engine.Evaluate("rect(1, 5, 3, 7, col=\"white\")");
            engine.Evaluate("dev.off()");

            //Boxplot 2
            engine.Evaluate("png(filename=boxYearImage)");//image to plot to
            engine.Evaluate("boxplot(CropProd_ProdSort$Production ~ CropProd_ProdSort$Year ,main=\"BOXPLOT\",ylab=\"Production\", xlab = \"Year\")");
            engine.Evaluate("rect(1, 5, 3, 7, col=\"white\")");
            engine.Evaluate("dev.off()");


            //Boxplot 3
            engine.Evaluate("png(filename=boxYieldImage)");//image to plot to
            engine.Evaluate("boxplot(CropProd_ProdSort$Production ~CropProd_ProdSort$Yield, main = \"BOXPLOT\", ylab = \"Production\", xlab = \"Yield\")");
            engine.Evaluate("rect(1, 5, 3, 7, col=\"green\")");
            engine.Evaluate("dev.off()");



            //R base library
            engine.Evaluate("library(base)");

            return View("/Views/Home/BoxPlot.cshtml");
        }

        public IActionResult Histogram()
        {
            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();


            //path vectors
            CharacterVector vectorDataPath = engine.CreateCharacterVector(new[] { dataFilePath });
            CharacterVector vectorProdution = engine.CreateCharacterVector(new[] { histogramProductionPath });
            CharacterVector vectorYield = engine.CreateCharacterVector(new[] { histogramYieldPath });

            //symbols
            engine.SetSymbol("dataFile", vectorDataPath);
            engine.SetSymbol("histProductionImage", vectorProdution);
            engine.SetSymbol("histYieldImage", vectorYield);

            //plotting - read data
            engine.Evaluate("CropProd<-read.csv(dataFile)");

            //Histogram 1
            engine.Evaluate("png(filename=histProductionImage)");//image to plot to
            engine.Evaluate("hist(CropProd$Production,main = \"Histogram\",ylim = c(0,100), xlim = c(0,1000000000),xlab = \"Production\")");
            engine.Evaluate("rect(1, 5, 3, 7, col=\"white\")");
            engine.Evaluate("dev.off()");

            //Histogram 2
            engine.Evaluate("png(filename=histYieldImage)");//image to plot to
            engine.Evaluate("hist(CropProd$Yield,main = \"Histogram\", ylim = c(0,100) ,xlab = \"Yield\")");
            engine.Evaluate("rect(1, 5, 3, 7, col=\"white\")");
            engine.Evaluate("dev.off()");


            //R base library
            engine.Evaluate("library(base)");

            return View("/Views/Home/Histogram.cshtml");
        }

        public IActionResult Table()
        {


            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();

            //R base library
            engine.Evaluate("library(base)");
            engine.Evaluate("library(sjmisc)");

            //path vectors
            CharacterVector vectorDataPath = engine.CreateCharacterVector(new[] { dataFilePath });
            CharacterVector vectorTable = engine.CreateCharacterVector(new[] { tablePath });

            //symbols
            engine.SetSymbol("dataFile", vectorDataPath);
            engine.SetSymbol("tableImage", vectorTable);

            //plotting - read data
            engine.Evaluate("CropProd<-read.csv(dataFile)");

            //variables
            engine.Evaluate("Frequency=table(CropProd$Season, exclude = NULL)");
            engine.Evaluate("Percentage=prop.table(Frequency)*100");
            engine.Evaluate("Cumlative_Frequency=cumsum(Frequency)");
            engine.Evaluate("Cumlative_Relative_Frequency=cumsum(Percentage)");
            engine.Evaluate("Freq_Table=cbind(Frequency,Percentage,Cumlative_Frequency,Cumlative_Relative_Frequency)");


            //Frequency Table
            engine.Evaluate("freq<-frq(CropProd$Year, out=\"viewer\",title = \"FREQ TABLE\")");
        
            return View("/Views/Home/Table.cshtml");
        }

        public IActionResult Regression()
        {
            RegressionData regData = new RegressionData();

            regData.Area = 10;
            if (Request.HasFormContentType)
            {
                try
                {
                    regData.Area = Double.Parse(Request.Form["area"]);
                }
                catch (Exception)
                {

                    regData.Area = 10;
                }
            }


            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();

            //R base library
            engine.Evaluate("library(base)");
            //path vectors
            CharacterVector vectorDataPath = engine.CreateCharacterVector(new[] { dataFilePath });
            CharacterVector vectorImagePath = engine.CreateCharacterVector(new[] { regressionImagePath });

            //symbols
            engine.SetSymbol("dataFile", vectorDataPath);
            engine.SetSymbol("regressionImage", vectorImagePath);

            //plotting - read data
            engine.Evaluate("CropProd<-read.csv(dataFile)");

            //Regression Function
            engine.Evaluate("Regression_Model<-function(xhat) {" +
                "x=CropProd$Area; " +
                "y = CropProd$Production; " +
                "png(filename=regressionImage);" +
                "plot(CropProd$Area,CropProd$Production, xlab = \"Area\", ylab = \"Production\");" +
                "rect(1, 5, 3, 7, col =\"white\");" +
                "dev.off();" +
                "sdx=sd(x,na.rm = TRUE);" +
                "sdy=sd(y,na.rm = TRUE);" +
                "Corelation=cor(x,y, use=\"complete.obs\")/  (sdx * sdy);" +
                "bhat=sum((x-mean(x, na.rm = TRUE))*(y-mean(y, na.rm = TRUE)), na.rm = TRUE)/sum((x-mean(x, na.rm = TRUE))^2,na.rm = TRUE);" +
                "ahat=mean(y, na.rm = TRUE)-(bhat*mean(x, na.rm = TRUE));" +
                "yhat=ahat+(bhat*xhat); " +
                "yhat; }");
            
            
            //ahat Function
            engine.Evaluate("getahat<-function(xhat) {" +
                "x=CropProd$Area; " +
                "y = CropProd$Production; " +
                
                "bhat=sum((x-mean(x, na.rm = TRUE))*(y-mean(y, na.rm = TRUE)), na.rm = TRUE)/sum((x-mean(x, na.rm = TRUE))^2,na.rm = TRUE);" +
                "ahat=mean(y, na.rm = TRUE)-(bhat*mean(x, na.rm = TRUE));" +
                "yhat=ahat+(bhat*xhat); " +
                "return(ahat); }"); 
            
            //bhat Function
            engine.Evaluate("getbhat<-function(xhat) {" +
                "x=CropProd$Area; " +
                "y = CropProd$Production; " +
                
                "bhat=sum((x-mean(x, na.rm = TRUE))*(y-mean(y, na.rm = TRUE)), na.rm = TRUE)/sum((x-mean(x, na.rm = TRUE))^2,na.rm = TRUE);" +
                "ahat=mean(y, na.rm = TRUE)-(bhat*mean(x, na.rm = TRUE));" +
                "yhat=ahat+(bhat*xhat); " +
                "return(bhat); }");

            regData.Production = engine.Evaluate("Regression_Model(" + regData.Area + ")").AsInteger().ToArray()[0];
            regData.ahat = engine.Evaluate("getahat(" + regData.Area + ")").AsInteger().ToArray()[0];
            regData.bhat = engine.Evaluate("getbhat(" + regData.Area + ")").AsInteger().ToArray()[0];

            if (!Request.HasFormContentType)
            {
                regData.Production = 0;
            }


            return View("/Views/Home/Regression.cshtml", regData);
        }
        public IActionResult Descriptive()
        {



            DescriptiveData data = new DescriptiveData();

            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();

            //R base library
            engine.Evaluate("library(base)");
            engine.Evaluate("library(sjmisc)");

            //path vectors
            CharacterVector vectorDataPath = engine.CreateCharacterVector(new[] { dataFilePath });

            //symbols
            engine.SetSymbol("dataFile", vectorDataPath);

            //plotting - read data
            engine.Evaluate("CropProd<-read.csv(dataFile)");

            //sorted data
            engine.Evaluate("CropProd_ProdSort=CropProd[order(CropProd$Production),]");
            engine.Evaluate("CropProd_YieldSort=CropProd[order(CropProd$Yield),]");

            //mean,median
            data.ProductionMean = engine.Evaluate("ProductionMean = mean(CropProd$Production, na.rm = TRUE)").AsInteger().ToArray()[0];
            data.ProductionMedian = engine.Evaluate("ProductionMedian = median(CropProd_YieldSort$Production)").AsInteger().ToArray()[0];
            data.YieldMean = engine.Evaluate("YieldMean=mean(CropProd$Yield, na.rm=TRUE)").AsInteger().ToArray()[0];
            data.YieldMean = engine.Evaluate("YieldMeadian=median(CropProd_YieldSort$Yield , na.rm=TRUE)").AsInteger().ToArray()[0];


            engine.Evaluate("quantile(CropProd_ProdSort$Production, na.rm = TRUE)");
            data.FirstQuartile = engine.Evaluate("FirstQuantile=quantile(CropProd_ProdSort$Production ,0.25, na.rm = TRUE)").AsInteger().ToArray()[0];
            data.ThirdQuartile = engine.Evaluate("ThirdQuantile=quantile(CropProd_ProdSort$Production ,0.75, na.rm = TRUE)").AsInteger().ToArray()[0];
            data.IQR = engine.Evaluate("IQR= ThirdQuantile - FirstQuantile").AsInteger().ToArray()[0]; ;


            return View("/Views/Home/Descriptive.cshtml", data);
        }

        public IActionResult Binomial()
        {
            BinomialData data = new BinomialData();
            if (!Request.HasFormContentType)
            {
                data.result = -1;
                return View("/Views/Home/Binomial.cshtml", data);
            }

            data.N = Int32.Parse(Request.Form["n"]);
            data.x = Int32.Parse(Request.Form["x"]);

            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();

            //R base library
            engine.Evaluate("library(base)");
            engine.Evaluate("library(sjmisc)");

            //path vectors
            CharacterVector vectorDataPath = engine.CreateCharacterVector(new[] { dataFilePath });

            //symbols
            engine.SetSymbol("dataFile", vectorDataPath);

            //read data
            engine.Evaluate("CropProd<-read.csv(dataFile)");

            data.result = engine.Evaluate("x<-sum(dbinom( " + data.x + "," + data.N + ",0.775))").AsRaw().ToArray()[0];

            data.result = BinomialProbability(data.N, data.x, 0.775); 

            return View("/Views/Home/Binomial.cshtml", data);
        }
        public IActionResult Plot()
        {

            //R initalization
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance(RPath);
            engine.Initialize();

            //R base library
            engine.Evaluate("library(base)");

            //R File names
            CharacterVector vectorImagePath = engine.CreateCharacterVector(new[] { imagePath });
            CharacterVector vectorDatasetPath = engine.CreateCharacterVector(new[] { dataFilePath });
            engine.SetSymbol("imageFile",vectorImagePath);
            engine.SetSymbol("dataFile",vectorDatasetPath);


            

            //Read Data
            engine.Evaluate("dataset <- read.csv(dataFile)");
            engine.Evaluate("x<- dataset$Area");
            engine.Evaluate("y<- dataset$Production");

            //Plot Data
            engine.Evaluate("png(filename=imageFile)");//image to plot to
            engine.Evaluate("plot(x,y)");
            engine.Evaluate("rect(1, 5, 3, 7, col=\"white\")");
            engine.Evaluate("dev.off()");

            return View("/Views/Home/Plot.cshtml");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string RegressionFunction { get; set; }

        public double BinomialProbability(int trials, int successes,
                           double probabilityOfSuccess)
        {
            double probOfFailures = 1 - probabilityOfSuccess;

            double c = Combination(trials, successes);
            double px = Math.Pow(probabilityOfSuccess, successes);
            double qnx = Math.Pow(probOfFailures, trials - successes);

            return c * px * qnx;
        }

        public static long Combination(long a, long b)
        {
            if (a <= 1)
                return 1;



            return Factorial(a) / ((a == b) ? 1: ( (Factorial(b) * Factorial(a - b))));
        }

        public static long Factorial(long x)
        {
            if (x <= 1)
                return 1;
            else
                return x * Factorial(x - 1);
        }

    }
}
