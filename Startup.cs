using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Task_Manager_Function.Data;
using Microsoft.EntityFrameworkCore;


[assembly: FunctionsStartup(typeof(Task_Manager_Function.Startup))]

namespace Task_Manager_Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<TaskContext>(
                options => options.UseInMemoryDatabase("TaskList"));
        }
    }
}
