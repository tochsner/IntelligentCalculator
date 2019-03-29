using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentCalculator
{
    class Constants
    {
        public static double NoveltyFactor = 0.75;

        public static int PopulationSize = 10;
        public static int MinPopulationSize = 4;

        public static double FactorPertubationProbability = 0;

        public static int NumberOfSampleModules = 20;
        public static int NumberOfIterationsPerInput = 2;

        public static double EliteRatio = 0.4;        
        public static double EliteFitnessThreshold = 0.9;   // The threshold for stopping the creation of new modules.        

        public static double SimpleScalingPropability = 0.2;
        public static double LoopPropability = 0.5;

        public static int LoopComplexity = 3;

        public static double ComplexifyProbability = 0.2;
    } 
}
