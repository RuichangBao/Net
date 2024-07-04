namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Test1Request test1Request = new Test1Request
            {
                ResultPerPage = 10,
                PageNumber = 1,
                Query = "safdf"
            };
            Console.WriteLine(test1Request.ToString());
            Console.WriteLine("Hello, World!");
        }
    }
}