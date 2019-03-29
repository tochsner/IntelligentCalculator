using System;
using System.Collections.Generic;

namespace IntelligentCalculator
{
    public abstract class BaseModule : IEquatable<BaseModule>
    {
        public int TimesUsed = 0;
        public string Identifier;
        public int InputsCount;

        public int Complexity = 0;

        public double AverageRelativeError = 0;
        
        public abstract double Fitness { get; }

        public abstract double Compute(List<double> inputs);        

        public abstract void Complexify();
             
        public abstract BaseModule GetMutatedModule();
        public abstract BaseModule GetComplexifiedModule();

        public abstract void SaveError(double relativeError);
       
        public bool Equals(BaseModule obj)
        {
            return ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}