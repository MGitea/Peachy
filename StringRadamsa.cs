using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using NLog;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Pro.Core.Mutators.Utility;

namespace Peach.Pro.Core.Mutators
{
    /// <summary>
    /// Picks a gaussian random number X centered on 1, with a
    /// sigma of 1/3 the string length.
    /// Then, pick X random indices in the string.
    /// At each selected index, toggle the case of the character.
    /// </summary>
    [Mutator("StringRadamsa")]
    [Description("Radamsa Mutator")]
    public class StringRadamsa : Mutator
    {
        static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        int total;

        public StringRadamsa(DataElement obj)
            : base(obj)
        {
            logger.Debug("Starting radamsa...");
            var str = (string)obj.InternalValue;
            logger.Debug("Converted to string...");
            // For sequential, use the length total number of mutations
            total = str.Length;
        }

        public new static bool supportedDataElement(DataElement obj)
        {
            if (obj is Peach.Core.Dom.String && obj.isMutable)
                return true;

            return false;
        }

        public override uint mutation
        {
            get;
            set;
        }

        public override int count
        {
            get
            {
                return total;
            }
        }

        public override void sequentialMutation(DataElement obj)
        {
            randomMutation(obj);
        }
        
        
        public override void randomMutation(DataElement obj)
        {
            try
            {
                var str = (string)obj.InternalValue;
                var sb = new StringBuilder(str);
                string output;
                File.WriteAllText("D:\\protocol-fuzzer-ce-main\\radamsa\\out.txt", sb.ToString());
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                    // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = Radamsa.FileName;
                p.StartInfo.Arguments = Radamsa.Arguments;

                p.Start();

                output = p.StandardOutput.ReadToEnd();
                var ret = new StringBuilder(output);
                p.WaitForExit();

                obj.MutatedValue = new Variant(ret.ToString());
                obj.mutationFlags = MutateOverride.Default;
            }
            catch (NotSupportedException ex)
            {
                logger.Debug("Skipping mutation of {0}, {1}", obj.debugName, ex.Message);
                return;
            }

            
        }
    }
}
