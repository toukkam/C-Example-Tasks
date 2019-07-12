using System;
using System.Collections.Generic;
using System.Text;

namespace C_SharpTasks
{
    public class FooBar
    {
        public void Run()
        {
            /* Basic implementation of FooBar or FizzBuzz.
             * First check if number is divisible by both 3 and 5, if so, print both Foo and Bar
             * Second: check if number is divisible by 3, if so, print Foo
             * Third: check if number is divisible by 5, if so, print Bar
             * Fourth: if not catched in above cases, print number
             * 
             * Implementation is done by if else, and handling the case with both 3 and 5 first
             * because I believe this way leads to the least repetition. If done by handling 3, then 5,
             * both cases have to be caught with "else" to avoid printing numbers with either Foo or Bar.
             */

            for (int i = 1; i < 100; i++)
            {
                if (i % 3 == 0 && i % 5 == 0)
                {
                    Console.WriteLine("FooBar");
                }
                else if (i % 3 == 0)
                {
                    Console.WriteLine("Foo");
                }
                else if (i % 5 == 0)
                {
                    Console.WriteLine("Bar");
                }
                else
                {
                    Console.WriteLine(i);
                }
            }
        }
    }
}
