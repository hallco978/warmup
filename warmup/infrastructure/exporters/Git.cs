using System;
using System.Diagnostics;
using warmup.infrastructure.settings;
using System.IO;
using System.Net;

namespace warmup.infrastructure.exporters
{
    public class Git : BaseExporter
    {
        public override void Export(string sourceControlWarmupLocation, string templateName, TargetDir targetDir)
        {
            var gitsrc = (new Uri(sourceControlWarmupLocation)).IsFile
                ? Path.Combine(sourceControlWarmupLocation, templateName)
                : NewUri(sourceControlWarmupLocation, templateName).AbsoluteUri;

            var destination = targetDir == null
                ? new TargetDir(Environment.CurrentDirectory)
                : targetDir;

            var gitSrcPath = Verifier.TestPath(gitsrc);

            var psi = new ProcessStartInfo("cmd", string.Format(" /c git clone {0} {1}", gitSrcPath, destination.FullPath));

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            //todo: better error handling
            Console.WriteLine("Running: {0} {1}", psi.FileName, psi.Arguments);
            string output, error = "";
            using (Process p = Process.Start(psi))
            {
                output = p.StandardOutput.ReadToEnd();
                error = p.StandardError.ReadToEnd();
            }

            RemoveGitConfig(destination);

            Console.WriteLine(output);
            Console.WriteLine(error);

        }

        private static void RemoveGitConfig(TargetDir destination)
        {
            Directory.Delete(path: Path.Combine(destination.FullPath, ".git"), recursive: true);
        }

        private static Uri NewUri(string baseUri, string relativeUri)
        {
            var r = CreateUri(baseUri, relativeUri);
            if (r.Item1)
            {
                return r.Item2;
            }
            else
            {
                r = CreateUri(baseUri, "");
                if (r.Item1)
                {
                    return r.Item2;
                }
                else
                {
                    throw new ArgumentException("The base is not valid");
                }
            }
        }

        private static Tuple<bool, Uri> CreateUri(string baseUri, string relativeUri)
        {
            return CreateUri(
                    baseUri.EndsWith("/")
                    ? new Uri(baseUri)
                    : new Uri(baseUri + "/"),
                    relativeUri);
        }

        private static Tuple<bool,Uri> CreateUri(Uri baseUri, string relativeUri) {
            Uri ret;
            return Tuple.Create(Uri.TryCreate(baseUri, relativeUri, out ret), ret); 
        } 

        //public static void Clone(Uri sourceLocation, TargetDir target)
        //{
        //}

        //public static void Clone(string sourceLocation, TargetDir target)
        //{
        //}
    }
}