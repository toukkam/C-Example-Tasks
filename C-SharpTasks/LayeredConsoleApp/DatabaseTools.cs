using System;
using System.Collections.Generic;
using System.Linq;

namespace C_SharpTasks.LayeredConsoleApp
{
    /* DatabaseTools is a class that includes all of logic which include manipulating the database or fetching 
     * data from the database. All of the functions here assume that the input has been sanitized and no extra
     * error handling is required, to keep the functions as simple as possible.
     */
    class DatabaseTools
    {
        // Get all tasks as a list from the database.
        public List<Task> GetAllTasks()
        {
            using (var db = new DatabaseContext())
            {
                return db.Tasks.ToList();
            }
        }

        // Helper function to check if a task of given name exists in the database.
        public bool TaskExists(string name)
        {
            using (var db = new DatabaseContext())
            {
                // Linq.Any() function can be used to check if the list returned by the database is empty or not.
                // Function returns either true if the list contains a task
                // or false if the list is empty, i.e., the task does not exist.
                return db.Tasks.Where(t => t.Name == name).Any();
            }
        }

        // Add a single task to the database and save it.
        public void AddTaskToDatabase(string name, string description)
        {
            using (var db = new DatabaseContext())
            {
                // Create a new Task with the given name and description.
                // All tasks are created as 'in-progress', i.e., Done field as false.
                var task = new Task()
                {
                    Name = name,
                    Description = description,
                    Done = false
                };

                // Add the new task to Tasks table.
                db.Tasks.Add(task);

                // Save changes to the database.
                db.SaveChanges();

                // Inform user of a successful database manipulation.
                Console.WriteLine(string.Format("Task '{0}' added to database.", name));
            }
        }
        
        // Delete a single task from the database and save the database.
        public void DeleteTaskFromDatabase(string name)
        {
            using (var db = new DatabaseContext())
            {
                // Create a new Task which will be used to remove a task from the database with the same name.
                // Creating a new task saves us a database query as we do not need to find the original database entry. 
                var task = new Task { Name = name };

                // Remove a task with the name of given from the function parameters
                db.Tasks.Remove(task);

                // Save changes to the database.
                db.SaveChanges();

                // Inform user of a successful database manipulation.
                Console.WriteLine(string.Format("Removed task '{0}'.", name));
            }
        }

        // Update the description of a task.
        public void UpdateTaskDescription(string name, string description)
        {
            using (var db = new DatabaseContext())
            {
                // Find the task we should change the description of.
                Task task = db.Tasks.Find(name);

                // Update the description of the task returned from the database.
                task.Description = description;

                // Update the database entry values with the new description.
                db.Entry(task).CurrentValues.SetValues(task);

                // Save changes to the database.
                db.SaveChanges();

                // Inform user of a successful database manipulation.
                Console.WriteLine(string.Format("Updated description of '{0}'.", name));
            }
        }

        // Set a task as 'done' or 'in-progress'.
        public void ToggleTask(string name)
        {
            using (var db = new DatabaseContext())
            {
                // Find the task we should toggle.
                Task task = db.Tasks.Find(name);

                // Flip the done flag of the task, so if the task was 'done', set it as 'in-progress' and vice versa.
                task.Done = !task.Done;

                // Update the database entry with the new value.
                db.Entry(task).CurrentValues.SetValues(task);

                // Save changes to the database.
                db.SaveChanges();

                // Inform user of a successful database manipulation.
                Console.WriteLine(string.Format("Set task '{0}' as '{1}'.", name, task.Done ? "done" : "in-progress"));
            }
        }
    }
}
