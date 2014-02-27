using System;
using warmup.infrastructure.settings;

namespace warmup.infrastructure
{
    public static class CommonHelp
    {
        public static void ShowHelp()
        {
            Console.WriteLine("==========");
            Console.WriteLine("WarmuP");
            Console.WriteLine("==========");
            Console.WriteLine("current settings");
            Console.WriteLine("----------");
            Console.WriteLine("Your current configuration is set to {0} ({1}).",
                              WarmupConfiguration.settings.SourceControlType,
                              WarmupConfiguration.settings.SourceControlWarmupLocation
                );
            if (!WarmupConfiguration.settings.SourceControlWarmupLocationIsValid)
            {

                Console.WriteLine("----------");
                Console.WriteLine("The Source Control Warmup Location is not Valid");
                Console.WriteLine("Please ensure that '{0}' is accessible", WarmupConfiguration.settings.SourceControlWarmupLocation);
            }
            Console.WriteLine("----------");
            Console.WriteLine("usage");
            Console.WriteLine("----------");
            Console.WriteLine("warmup [command] [options]");
            Console.WriteLine("warmup [addTemplateFolder|addTextReplacement] [options]");
        }
    }
}