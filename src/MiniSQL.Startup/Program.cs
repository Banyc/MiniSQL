using MiniSQL.Startup.Controllers;

namespace MiniSQL.Startup
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseController controller = new DatabaseController();
            View view = new View(controller);
            view.Interactive();
        }
    }
}
