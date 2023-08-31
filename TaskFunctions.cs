using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Task_Manager_Function.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;



namespace Task_Manager_Function
{
    public class TaskFunctions
    {
        private readonly TaskContext _context;

        public TaskFunctions(TaskContext context)
        {
            _context = context;
        }

        //Create a new task
        [FunctionName("CreateTask")]
        public static async Task<IActionResult> CreateTask(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "post", Route = "task")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new task");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var inputTask = JsonConvert.DeserializeObject<TaskModel>(requestBody);

            using (var _context = new TaskContext())
            {
                _context.Tasks.Add(inputTask);
                await _context.SaveChangesAsync();
            }

            return new OkObjectResult(inputTask);
        }


        // Retrieve a specific task by ID
        [FunctionName("GetTaskById")]
        public IActionResult GetTaskById(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "get", Route = "task/{id}")] HttpRequest req, int id)
        {
            var taskItem = _context.Tasks.Find(id);
            if (taskItem == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(taskItem);
        }

        // Retrieve all tasks
        [FunctionName("GetAllTasks")]
        public IActionResult GetAllTasks(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "get", Route = "tasks")] HttpRequest req)
        {
            return new OkObjectResult(_context.Tasks.ToList());
        }

        // Update a task
        [FunctionName("UpdateTask")]
        public async Task<IActionResult> UpdateTask(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "put", Route = "task/{id}")] HttpRequest req, int id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedTask = JsonConvert.DeserializeObject<Task_Manager_Function.Data.TaskModel>(requestBody);

            TaskModel existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null)
            {
                return new NotFoundResult();
            }

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            // ... (update other properties as needed)

            await _context.SaveChangesAsync();

            return new OkObjectResult(existingTask);
        }

        //Delete a task
        [FunctionName("DeleteTask")]
        public IActionResult DeleteTask(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "delete", Route = "task/{id}")] HttpRequest req, int id)
        {
            var taskToDelete = _context.Tasks.Find(id);
            if (taskToDelete == null)
            {
                return new NotFoundResult();
            }

            _context.Tasks.Remove(taskToDelete);
            _context.SaveChanges();

            return new OkResult();
        }

    }
}

