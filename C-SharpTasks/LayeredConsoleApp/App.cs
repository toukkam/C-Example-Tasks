using System;
using System.Collections.Generic;

namespace C_SharpTasks.LayeredConsoleApp
{
    /* A command line app to demonstrate simple user input, data and service layers and database manipulation.
     * App is ran from Program.cs file in the root of the directory by enabling the flag for the LayeredConsoleApp.
     * When ran, console will appear displaying the available commands for the user:
     * Commands:
     *     H: Print this message
     *     A: Add a task
     *     L: List all tasks
     *     R: Remove a task
     *     U: Update a task's description
     *     T: Toggle a task's completion
     *     Q: Quit the program
     *     
     * The app uses local SQL Server, installed with Microsoft SQL Server 2017 Developer Edition.
     */
    class App
    {
        public void RunApp()
        {
            // When running the app, display available commands to the user right away.
            PrintHelp();

            // Create only one variable of dbTools, which all of the functions that need it use.
            DatabaseTools dbTools = new DatabaseTools();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey();
                Console.WriteLine(""); // After user input print a new line for user readability.

                // All of the database manipulation is hidden behind functions to help readability of logic.
                switch (keyInfo.Key)
                {
                    case ConsoleKey.H:
                        PrintHelp();
                        break;

                    case ConsoleKey.A:
                        AddTask(dbTools);
                        break;

                    case ConsoleKey.L:
                        ListTasks(dbTools);
                        break;

                    case ConsoleKey.R:
                        RemoveTask(dbTools);
                        break;

                    case ConsoleKey.U:
                        UpdateTask(dbTools);
                        break;

                    case ConsoleKey.T:
                        ToggleTask(dbTools);
                        break;
                    default:
                        break;
                }
            }
            while (keyInfo.Key != ConsoleKey.Q);
        }

        // Simple function to display available commands for the user.
        private void PrintHelp()
        {
            Console.WriteLine(String.Join(Environment.NewLine,
                "Commands: ",
                "H: Print this message",
                "A: Add a task",
                "L: List all tasks",
                "R: Remove a task",
                "U: Update a task's description",
                "T: Toggle a task's completion",
                "Q: Quit the program"
            ));
        }

        // Get tasks from the database and print the formatted in the console.
        private void ListTasks(DatabaseTools dbTools)
        {
            // Call databasetools to fetch data from the database.
            List<Task> tasks = dbTools.GetAllTasks();

            // Print a line for readability of tasks in the console.
            Console.WriteLine(new String('-', 25));

            // Iterate over tasks from the database and print them.
            foreach (var task in tasks)
            {
                Console.WriteLine(string.Format("{0} '{1}': '{2}'", task.Done ? "X" : " ", task.Name, task.Description));
            }
        }

        // Add a task to the database.
        private void AddTask(DatabaseTools dbTools)
        {
            // User writes desired name for the task.
            Console.WriteLine("Enter name of task to be added:");
            string name = Console.ReadLine();
            
            // Check that a name is given and the name doesn't already exist in the database.
            if (name.Length == 0)
            {
                Console.WriteLine("Error: Name must be at least 1 character.");
                return;
            }
            if (dbTools.TaskExists(name))
            {
                Console.WriteLine(string.Format("Error: Task named '{0}' already exists.", name));
                return;
            }

            // User writes desired description for the to be created task.
            Console.WriteLine(string.Format("Enter description for task '{0}':", name));
            string description = Console.ReadLine();

            // Call the database tools to make changes to the database.
            dbTools.AddTaskToDatabase(name, description);
        }

        // Remove a single task from the database.
        private void RemoveTask(DatabaseTools dbTools)
        {
            // User writes which task they want to remove.
            Console.WriteLine("Enter name of task to be removed:");
            string name = Console.ReadLine();

            // Check if the task exists
            if (!dbTools.TaskExists(name))
            {
                Console.WriteLine(string.Format("Error: Task named '{0}' doesn't exist.", name));
                return;
            }

            // Call the database tools to make changes to the database.
            dbTools.DeleteTaskFromDatabase(name);
        }

        // Update the description of a task in the database.
        private void UpdateTask(DatabaseTools dbTools)
        {
            // User writes which task they want to update the description of.
            Console.WriteLine("Enter name of task to be updated:");
            string name = Console.ReadLine();

            // Check if the task exists
            if (!dbTools.TaskExists(name))
            {
                Console.WriteLine(string.Format("Error: Task named '{0}' doesn't exist.", name));
                return;
            }

            // User writes new description.
            Console.WriteLine(string.Format("Enter a description for '{0}'.", name));
            string description = Console.ReadLine();

            // Call the database tools to make changes to the database.
            dbTools.UpdateTaskDescription(name, description);
        }

        // Toggle a task as 'done' or 'in-progress' in the database.
        private void ToggleTask(DatabaseTools dbTools)
        {
            // User writes which task they want to toggle.
            Console.WriteLine("Enter name of task to be toggled:");
            string name = Console.ReadLine();

            // Check if the task exists
            if (!dbTools.TaskExists(name))
            {
                Console.WriteLine(string.Format("Error: Task named '{0}' doesn't exist.", name));
                return;
            }

            // Call the database tools to make changes to the database.
            dbTools.ToggleTask(name);
        }
    }
}
