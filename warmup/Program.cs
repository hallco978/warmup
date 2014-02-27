using System;
using warmup.commands;
using warmup.infrastructure;
using warmup.infrastructure.settings;

namespace warmup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0) // || !WarmupConfiguration.settings.SourceControlWarmupLocationIsValid)
            {
                CommonHelp.ShowHelp();
                Environment.Exit(-1);
            }

            var command = args[0];

            ICommand cmd = null;

            switch (command.ToLower())
            {
                case "addtextreplacement":
                    cmd = new AddTextReplacement();
                    break;
                case "addtemplatefolder":
                    cmd = new AddTemplateFolder();
                    break;
                default:
                    cmd = new GenerateWarmup();
                    break;
            }

            cmd.Run(args);
        }
    }
}