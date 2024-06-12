# PrecisionCrop
This is my project of Probability and Statistics Course (Fall 2022). It is a web-based application that provides an analysis of the Agricultural crops of 13 states of India. 
## Tools 
 1. The Web Frontend is designed using HTML5, CSS3 and JavaScript.
 2. The Web Backend is designed using ASP.Net MVC.
 3. The Analysis is done using R language. R is integrated with the backend using RDotNet NuGet Package.
 4. The project is built using .Net 5.0.
## How to Run This App
 1. Make Sure you have R installed correctly.
 2. In home controller, you will find this line of code:
    private const string RPath = "C:\\Program Files\\R\\R-4.2.2\\bin\\x64\\R.dll";
    Make sure it points to the actual path of R on your PC
4. Make sure you have the R package "sjmisc" installed.
5. Run the App

## Features and Data Analysis
The dataset contains following attributes:
- State
- District
- Crop
- Year
- Season
- Area
- Area Units
- Production
- Production Units
- Yield

### Box Plots
There are three Box plots:
* Production W.r.t Year
* Production W.r.t. Season
* Production W.r.t. Yield

### Histograms
There are two histograms:
* Histogram of Production
* Histogram of Yield
Both variables are left skewed

### Frequency Table
A frequency table is plotted, where intervals are made according to year.

### Descriptive Statistics
Following descriptive statistics are calculated for production:
- Mean: 108182609
- Median: 17100000
- First Quartile: 3135750
- Third Quartile: 94818500
- Interquartile Range: 91682750

### Regression Modelling
Linear regression is used to calculate Production as a function of area. A scatter plot is also generated. The form allows you to input a value of area, and uses regression equation to predict Production.
