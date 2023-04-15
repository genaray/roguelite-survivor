using System;
using System.IO;

try
{
    using (var game = new RogueliteSurvivor.Game1())
    {
        game.Run();
    }
}
catch (Exception ex)
{
    if (!Directory.Exists(Path.Combine("Logs")))
    {
        Directory.CreateDirectory(Path.Combine("Logs"));
    }

    File.WriteAllText(Path.Combine("Logs", string.Concat("error-", DateTime.Now.ToString("yyyyMMddHHmmss"), ".txt")), ex.ToString());
}
