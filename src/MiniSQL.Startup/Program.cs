using MiniSQL.Startup.Controllers;

namespace MiniSQL.Startup
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiPagerBuilder builder = new ApiPagerBuilder();
            DatabaseController controller = new DatabaseController(builder);
            View view = new View(controller);
            view.Interactive();
        }
    }
}
