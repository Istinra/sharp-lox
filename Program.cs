// See https://aka.ms/new-console-template for more information



if (args.Length > 1)
{
    Console.WriteLine("Usage: jlox [script]");
    return 64;
}
else if(args.Length == 1)
{
    return Lox.Lox.RunFile(args[0]);
}
else
{
    Console.Write("> ");
    while (Console.ReadLine() is { } line)
    {
        return Lox.Lox.Run(line);
    }
}

return 0;
