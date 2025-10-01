using HTTP_Status;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Enter the path to the text file containing URLs:");
        HTTPStatus httpStatus = new HTTPStatus(Console.ReadLine()!);
    }
}