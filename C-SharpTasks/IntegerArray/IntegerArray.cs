using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace C_SharpTasks
{
    class IntegerArray
    {
        public int[] GenerateArray(string version)
        {
            int n = 100000;
            Random r = new Random();

            if (version == "original")
            {
                // First version of the answer: No dependencies, explicit code.
                int[] intArray1 = new int[n];
        
                // First loop: Populate the array with integers from 0 to n.
                for (int i = 0; i < n; i++)
                {

                    intArray1[i] = i;
                }
                
                // Second loop: Randomize the order by swapping items with random index in the array.
                for (int i = 0; i < n; i++)
                {
                    int randomIndex = r.Next(0, n);
                    int temp = intArray1[i];
                    intArray1[i] = intArray1[randomIndex];
                    intArray1[randomIndex] = temp;
                }

                return intArray1;
            }
            else  // (version == "improved")
            {
                // Second version of the answer:
                // With a little googling, found a pretty effective functions to generate an array with n unique integers.
                // This version is basically the same as the above one, but is shorter code. Requires System.Linq namespace.
                int[] intArray2 = Enumerable.Range(0, n).ToArray();
                intArray2 = intArray2.OrderBy(x => r.Next()).ToArray();

                return intArray2;
            }

        }
    }
}
