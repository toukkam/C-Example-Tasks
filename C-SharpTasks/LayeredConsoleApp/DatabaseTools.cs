using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace C_SharpTasks.LayeredConsoleApp
{
    public struct Task
    {
        public string name;
        public string description;
        public bool done;
        public Task(string n, string desc, bool d)
        {
            name = n;
            description = desc;
            done = d;
        }
    }

    class DatabaseTools
    {
        /* A helper function for executing different kinds of SQL queries to the database.
         * Function opens a connection to the database, executes given queries and returns
         * what the database returns.
         */
        private (int, List<Task>) ExecuteCommand(string transactionName, string sqlCommand)
        {
            List<Task> output = new List<Task>();

            try
            {
                // Build connection string
                // NOTE! Requires system.data.sqlclient installed on the project, which can be found by right
                // clicking the project in solution explorer and then searching for "system.data.sqlclient"
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder()
                {
                    DataSource = "localhost",
                    InitialCatalog = "DemoDatabase" // Database called "DemoDatabase" should exist in the SQL server
                };

                // Use windows authentication so turn on integrated security
                builder.IntegratedSecurity = true;

                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    // Use try/catch blocks to handle possible errors
                    try
                    {
                        SqlTransaction transaction;
                        SqlCommand command = new SqlCommand(sqlCommand, connection);

                        transaction = connection.BeginTransaction(transactionName);
                        command.Connection = connection;
                        command.Transaction = transaction;

                        // GET tasks require SqlDataReader to return values from the database, handle them differently
                        if (transactionName == "Get Tasks")
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string name = reader.GetValue(0).ToString();
                                    string description = reader.GetValue(1).ToString();
                                    bool done = false;
                                    if (reader.GetValue(2).ToString() == "True")
                                    {
                                        done = true;
                                    }
                                    output.Add(new Task(name, description, done));
                                }
                            }
                        }

                        // Tasks that don't require return values from database
                        else
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    catch (Exception exception)
                    {
                        // If we catch an error, return code 1 and the exception description
                        Console.WriteLine("Error: " + exception.ToString());
                        return (1, output);
                    }

                    // We are done, close sql connection
                    connection.Close();
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine("SQL Error: " + exception.ToString());
                return (1, output);
            }

            // Everything went as expected, return code 0 and the output of the database query
            return (0, output);
        }

        /* Helper function for handling user errors, such as trying to add a task that alreay exists, or update a task
         * that doesn't exist.
         */
        private bool CheckIfTaskExists(string name)
        {
            // General case input
            if (name == "")
            {
                return false;
            }

            // Since no duplicate names are allowed, this query will return either 0 or 1 tasks which can be used
            // as a existence check.
            string commandStr = "SELECT name, description, done FROM DemoDatabase.dbo.Tasks WHERE name = '" + name + "';";

            List<Task> tasks;
            (_, tasks) = ExecuteCommand("Get Tasks", commandStr);

            // If database returns the task, return true as the task exists
            if (tasks.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* Helper function for fetching task data for other functions
         */
        private Task GetTask(string name)
        {
            // Since no duplicate names are allowed, this query will return either 0 or 1 tasks which can be used
            // as a existence check.
            string commandStr = "SELECT name, description, done FROM DemoDatabase.dbo.Tasks WHERE name = '" + name + "';";

            List<Task> tasks;
            (_, tasks) = ExecuteCommand("Get Tasks", commandStr);

            return tasks[0];
        }

        /* Reset and initialise a table 'Tasks'.
         */
        public void InitTable()
        {
            string commandStr = @"
            DROP TABLE IF EXISTS DemoDatabase.dbo.Tasks;
            CREATE TABLE Tasks(
                name varchar(32) UNIQUE,
                description text,
                done bit
            )";

            int code;
            (code, _) = ExecuteCommand("Create Tasks table", commandStr);

            if (code == 0)
            {
                Console.WriteLine("Table initialised.");
            }
        }

        /* Get and print the first 1000 tasks' names and descriptions
         */
        public void GetTasks()
        {
            string commandStr = @"SELECT TOP (1000) name, description, done FROM DemoDatabase.dbo.Tasks";

            int code;
            List<Task> tasks;
            (code, tasks) = ExecuteCommand("Get Tasks", commandStr);

            if (code == 0)
            {
                Console.WriteLine("Tasks\n--------------------------------------------");
                foreach (var task in tasks)
                {
                    string taskString = "";
                    if (task.done)
                    {
                        taskString += "X ";
                    }
                    else
                    {
                        taskString += "  ";
                    }
                    taskString += "'" + task.name + "': " + task.description;
                    Console.WriteLine(taskString);
                }
            }
        }

        /* Add a task. The function asks user to first specify a name and after that write the description for the task.
         * All tasks are inserted as "not done" into the database.
         */
        public void AddTask()
        {
            string name, description;

            Console.WriteLine("Enter task name:");
            name = Console.ReadLine();

            // Error handling
            if (name.Length == 0)
            {
                Console.WriteLine("Error: Name must be at least 1 character long.");
                return;
            }
            bool taskExists = CheckIfTaskExists(name);
            if (taskExists)
            {
                Console.WriteLine("Error: Task '" + name + "' already exist");
                return;
            }

            Console.WriteLine("Write a description for " + name + ":");
            description = Console.ReadLine();

            string commandStr = "INSERT INTO Tasks(name, description, done) VALUES ('" + name + "','" + description + "', 0)";

            int code;
            (code, _) = ExecuteCommand("Insert New Task", commandStr);

            if (code == 0)
            {
                Console.WriteLine("Task '" + name + "' inserted to database.");
            }
        }

        /* Update a task's information. First the user will be asked which task they want to update by entering the task name.
         * Then a new name, which can be skipped to retain current name.
         * Then a new description, which can also be skipped.
         * Finally, executes query with the user input in the databse.
         */
        public void UpdateTask()
        {
            string currentName, newName, description;

            Console.WriteLine("Enter the name of the task you want to update:");
            currentName = Console.ReadLine();

            // Error handling
            bool taskExists = CheckIfTaskExists(currentName);
            if (!taskExists)
            {
                Console.WriteLine("Error: Task '" + currentName + "' doesn't exist");
                return;
            }

            Console.WriteLine("Write a new name for the task " + currentName + " (or leave it blank to save the current one):");
            newName = Console.ReadLine();

            taskExists = CheckIfTaskExists(newName);
            if (taskExists)
            {
                Console.WriteLine("Error: Task '" + newName + "' already exists! Made no changes to the database.");
                return;
            }

            Console.WriteLine("Write a new description for " + newName+ " (or leave it blank to save the current one):");
            description = Console.ReadLine();

            // Case where user did not give a new name or a description
            string commandStr;
            if (newName == "" && description == "")
            {
                Console.WriteLine("User did not make any changes to the database.");
                return;
            }
            // Case where user gave only a new name
            if (description == "")
            {
                commandStr = "UPDATE Tasks SET name='" + newName + "' WHERE name= '" + currentName + "'";
            }
            // Case where user gave only a new description
            else if (newName == "")
            {
                commandStr = "UPDATE Tasks SET description='" + description + "' WHERE name= '" + currentName + "'";
            }
            // Case where user gave both a new name and a new description
            else
            {
                commandStr = "UPDATE Tasks SET name='" + newName + "', description='" + description + "' WHERE name= '" + currentName + "'";
            }

            int code;
            (code, _) = ExecuteCommand("Update Task", commandStr);

            if (code == 0)
            {
                if (newName == "")
                {
                    Console.WriteLine("Updated the description of '" + currentName + "'.");
                } 
                else
                {
                    Console.WriteLine("Updated '" + currentName + "' to '" + newName + "'.");
                }
            }
        }

        /* Set a task to be 'done' or 'in progress'.
        */
        public void ToggleTask()
        {
            string name, commandStr;

            Console.WriteLine("Enter task name to toggle:");
            name = Console.ReadLine();

            bool taskExists = CheckIfTaskExists(name);
            if (!taskExists)
            {
                Console.WriteLine("Error: Task '" + name + "' doesn't exist");
                return;
            }

            // Check if the task is already done. If toggling a done task, set it as 'in progress'
            bool taskDone = GetTask(name).done;
            commandStr = "UPDATE Tasks SET done=" + (taskDone ? "0" : "1") + " WHERE name= '" + name + "'";

            int code;
            (code, _) = ExecuteCommand("Toggle Task", commandStr);

            if (code == 0)
            {
                Console.WriteLine("Task toggled.");
            }
        }
    }
}
