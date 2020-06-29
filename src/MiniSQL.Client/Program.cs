using MiniSQL.Api.Controllers;
using MiniSQL.Library.Interfaces;
using MiniSQL.Client.Controllers;

namespace MiniSQL.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseBuilder builder = new DatabaseBuilder();
            IApi controller = new ApiController(builder);
            View view = new View(controller);
            view.Interactive();
        }
    }
}
