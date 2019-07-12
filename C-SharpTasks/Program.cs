using System;
using System.Linq;
using C_SharpTasks.LayeredConsoleApp;

namespace C_SharpTasks
{
    class Program
    {
        static void Main()
        {
            // Set here which tasks will be run
            bool runFooBar = true;
            bool runIntegerArray = false;
            bool runLayeredConsoleApp = false;

            /* Task 1:
             * Print numbers 1-100 so that if the number can be divided by 3 print Foo 
             * (instead of the number); if it can be divided by 5 print Bar (instead of the number)
             */
            if (runFooBar)
            {
                FooBar foobar = new FooBar();
                foobar.Run();
            }

            /* Task 2:
             * Create an array with unique numbers 1 - 100 000 in random order in it.
             */
            if (runIntegerArray)
            {
                IntegerArray integerArrayGenerator = new IntegerArray();

                // Generate integer arrays
                int[] intArray1 = integerArrayGenerator.GenerateArray("original");
                int[] intArray2 = integerArrayGenerator.GenerateArray("improved");

                // Test if the arrays have duplicate values
                if (intArray1.Length != intArray1.Distinct().Count())
                {
                    Console.WriteLine("Array 1 contains duplicates");
                }
                if (intArray2.Length != intArray2.Distinct().Count())
                {
                    Console.WriteLine("Array 2 contains duplicates");
                }
            }

            /* Task 3:
             * Create a (.NET, Visual Studio) console app which has simple data and service layers 
             * and reads something and writes something to the database.
             */
            if (runLayeredConsoleApp)
            {
                App consoleApp = new App();
                consoleApp.RunApp();
            }
        }
    }
}
