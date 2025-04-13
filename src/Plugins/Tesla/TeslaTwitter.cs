using ORBIT9000.Core.Plugin;

namespace ORBIT9000.Plugins.Tesla
{
    public class TeslaTwitter : IScraper
    {
        public void Execute()
        {
            Console.WriteLine(this.GetType().Name);
        }
    }
    public class TeslaNews : IScraper
    {
        public void Execute()
        {
            Console.WriteLine(this.GetType().Name);
        }
    }
}