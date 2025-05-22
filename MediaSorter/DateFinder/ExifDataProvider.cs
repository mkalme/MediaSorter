using System;
using System.Collections.Generic;
using System.Diagnostics;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class ExifDataProvider : INodeProvider<IDictionary<string, string>> {
        public IDictionary<string, string> ProvideNode(string nodePath) {
            return GetTags(Execute("exiftool.exe", $"\"{nodePath}\""));
        }

        private static string Execute(string exePath, string parameters) {
            string result = string.Empty;

            using (Process p = new Process()) {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = parameters;
                p.Start();
                p.WaitForExit(5000);

                result = p.StandardOutput.ReadToEnd();
            }

            return result;
        }
        private static Dictionary<string, string> GetTags(string output) {
            string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Dictionary<string, string> tags = new Dictionary<string, string>(lines.Length);

            foreach (string line in lines) {
                var pair = line.Split(new char[] { ':' }, 2);

                if (pair.Length >= 2) {
                    var key = pair[0].TrimEnd();
                    var value = pair[1].TrimStart();

                    tags[key] = value;
                }
            }

            return tags;
        }
    }
}
