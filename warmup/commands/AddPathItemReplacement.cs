using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Configuration;
using warmup.infrastructure;
using warmup.infrastructure.settings;
using System.IO;


namespace warmup.commands
{

    [Command("addPathItemReplacement")]
    public class AddPathItemReplacement : ICommand
    {
        public void Run(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                ShowHelp();
                Environment.Exit(-1);
            }



            var find = args[1];
            var replace = args[2];
            var path = args[3];
            
            



            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            WarmupConfiguration warmupConfig = config.GetSection("warmup") as WarmupConfiguration;

            if (warmupConfig != null)
            {
                warmupConfig.SectionInformation.ForceSave = true;

                bool itemFound = false;

                foreach (TextReplaceItem replaceItem in warmupConfig.TextReplaceCollection)
                {
                    if (replaceItem.Find.ToLower() == find.ToLower())
                    {
                        Console.WriteLine("Replacing '{0}' value of '{1}' with '{2}'.", find, replaceItem.Replace, replace);
                        replaceItem.Replace = replace;
                        itemFound = true;
                    }
                }

                if (!itemFound)
                {
                    Console.WriteLine("Adding '{0}' with a replacement of '{1}' to the configuration.", find, replace.Replace("\"", string.Empty));
                    warmupConfig.TextReplaceCollection.Add(new TextReplaceItem { Find = find, Replace = replace });
                }

                // Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.Save(ConfigurationSaveMode.Full);
            }
        }

        public void ShowHelp()
        {
            CommonHelp.ShowHelp();
            Console.WriteLine("----------");
            Console.WriteLine("usage for addTextReplacement");
            Console.WriteLine("----------");
            Console.WriteLine("warmup addTextReplacement findName replacementName");
            Console.WriteLine("Example: warmup addTextReplacement __COMPANY__ \"somewheres, inc\"");
            Console.WriteLine("Example: '__COMPANY__' is the token to search for, \"somwheres, inc\" is the replacement text.");
        }
    }

    private class FileRenamer
    {
        /// <summary>
        /// It will rename filename into title case format.
        /// </summary>
        /// <param name="directoryName">Directory in where this rename process will take place to rename all the containing files.</param>
        /// <param name="predicate">
        /// If it requires to do extra formatting for example, replace special charecters from the filename
        /// then consumer of this method can pass filtering behavior via this predicate.
        /// </param>
        public static void RenameFilesToTitleCase(string directoryName, Func<string, string> predicate)
        {
            Rename(directoryName, predicate, System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase);
        }

        /// <summary>
        /// It will rename filename into lower case format.
        /// </summary>
        /// <param name="directoryName">Directory in where this rename process will take place to rename all the containing files.</param>
        /// <param name="predicate">
        /// If it requires to do extra formatting for example, replace special charecters from the filename
        /// then consumer of this method can pass filtering behavior via this predicate.
        /// </param>
        public static void RenameFilesToLowerCase(string directoryName, Func<string, string> predicate)
        {
            Rename(directoryName, predicate, System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower);
        }

        /// <summary>
        /// It will rename filename into lower case format.
        /// </summary>
        /// <param name="directoryName">Directory in where this rename process will take place to rename all the containing files.</param>
        /// <param name="predicate">
        /// If it requires to do extra formatting for example, replace special charecters from the filename
        /// then consumer of this method can pass filtering behavior via this predicate.
        /// </param>
        public static void RenameFiles(string directoryName, Func<string, string> predicate)
        {
            Rename(directoryName, predicate, s => s);
        }

        /// <summary>
        /// Rename the filenam with the new name based on the condition provided by predicate and casePredicate.
        /// </summary>
        /// <param name="directoryName">Directory in where this rename process will take place to rename all the containing files.</param>
        /// <param name="predicate">Extra formatting to the filname.</param>
        /// <param name="casePredicate">This will point to the case changing method passed by calling method.</param>
        private static void Rename(string directoryName, Func<string, string> predicate, Func<string, string> casePredicate)
        {
            Directory.GetDirectories(directoryName).AsParallel().ToList().ForEach(directory =>
            {
                if (Directory.GetDirectories(directory).Count() > 0)
                    Rename(directory, predicate, casePredicate);
                Directory.GetFiles(directory).AsParallel().ToList().ForEach(file =>
                {
                    File.Move(file, casePredicate(GetNewFileName(predicate, directory, file)));
                });
            });
        }



        /// <summary>
        /// To get the new renamed filename.
        /// </summary>
        /// <param name="predicate">Formatting condition</param>
        /// <param name="directory">The directory for which this rename will take place.</param>
        /// <param name="file">Formatted filename</param>
        /// <returns>This method will return formatted renamed filenamed combined with directoryname.</returns>
        private static string GetNewFileName(Func<string, string> predicate, string directory, string file)
        {
            return Path.Combine(Path.GetFullPath(directory), Filter(Path.GetFileName(file), predicate));
        }

        /// <summary>
        /// To execute calling method provided formatting method.
        /// </summary>
        /// <param name="data">Filename to format</param>
        /// <param name="predicate">The delegate of the filename formatter.</param>
        /// <returns>Formatted filename.</returns>
        private static string Filter(string data, Func<string, string> predicate)
        {
            return !ReferenceEquals(predicate, null) ? predicate(data) : data;
        }
    }
}
