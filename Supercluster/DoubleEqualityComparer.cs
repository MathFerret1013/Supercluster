// Copyright 2015 Eric Regina
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//     http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Supercluster
{
    using System;
    using System.Collections.Generic;

    public class DoubleEqualityComparer : IEqualityComparer<double>
    {
        public bool Equals(double x, double y)
        {
            return Math.Abs(x - y) <= 1E-13;
        }

        public int GetHashCode(double obj)
        {
            return base.GetHashCode();
        }
    }

    public class DoubleArrayEqualityComparer : IEqualityComparer<double[]>
    {
        public bool Equals(double[] x, double[] y)
        {
            // check count
            if (x.Length != y.Length)
            {
                return false;
            }

            // check elementwise
            for(int i = 0; i < x.Length; i++)
            {
                if(Math.Abs(x[i] - y[i]) > SuperclusterContants.DOUBLE_TOLERANCE)
                {
                    return false;
                }
            }

            // have same count and elements match
            return true;
        }

        public int GetHashCode(double[] obj)
        {
            return base.GetHashCode();
        }
    }
}