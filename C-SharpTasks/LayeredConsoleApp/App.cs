using System;

namespace C_SharpTasks.LayeredConsoleApp
{
    /* A command line app to demonstrate simple user input, data and service layers and database manipulation.
     * App is ran from Program.cs file in the root of the directory by enabling the flag for the LayeredConsoleApp.
     * When ran, console will appear displaying the available commands for the user:
     * Commands:
     *     H: Print this message
     *     I: Initialise a table for tasks
     *     A: Add a task
     *     U: Update a task
     *     L: List all tasks
     *     Q: Quit the program
     *     
     * The app uses local database installed with Microsoft SQL Server 2017 Developer Edition
     */
    class App
    {
        public void RunApp()
        {
            PrintHelp();

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                Console.WriteLine("");

                DatabaseTools dbTools = new DatabaseTools();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.H:
                        PrintHelp();
                        break;
                    case ConsoleKey.I:
                        dbTools.InitTable();
                        break;
                    case ConsoleKey.A:
                        dbTools.AddTask();
                        break;
                    case ConsoleKey.L:
                        dbTools.GetTasks();
                        break;
                    case ConsoleKey.U:
                        dbTools.UpdateTask();
                        break;
                    case ConsoleKey.T:
                        dbTools.ToggleTask();
                        break;
                    default:
                        break;
                }
            }
            while (keyInfo.Key != ConsoleKey.Q);
        }

        private void PrintHelp()
        {   
            Console.WriteLine(String.Join(Environment.NewLine,
                "Commands: ",
                "H: Print this message",
                "I: Initialise a table for tasks",
                "A: Add a task",
                "U: Update a task",
                "L: List all tasks",
                "T: Toggle a task as 'done' or 'in progress'",
                "Q: Quit the program"
            ));
        }
    }
}
