using System;
using System.Text;
using Peach.Pro.Core.Mutators.Utility;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Peach.Core;
using Peach.Core.Dom;
using Peach.Core.IO;

namespace Peach.Pro.Core.Mutators
{
    /// <summary>
    /// Alter the blob by a random number of bytes between 1 and 100.
    /// Pick a random start position in the blob.
    /// Alter size bytes starting at position where each null byte is
    /// changed to different randomly selected non-null value.
    /// </summary>
    [Mutator("BlobRadamsa")]
    [Description("Mutating Blob with radamsa")]

    public class BlobRadamsa : Utility.BlobMutator
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public BlobRadamsa(DataElement obj)
            : base(obj, 100, true)
        {
        }

        protected override NLog.Logger Logger
        {
            get
            {
                return logger;
            }
        }

        
        protected override BitwiseStream PerformMutation(BitStream data, long start, long length)
        {
            var ret = new BitStreamList();
            var buf = new byte[length];
            var len = data.Read(buf, 0, buf.Length);

            using (BinaryWriter fp = new BinaryWriter(File.Open(Radamsa.Arguments, FileMode.Create)))
            {
                if (fp == null)
                {
                    logger.Debug("BlobRadamsa: Can't write to file");
                    return data;
                }
                fp.Write(buf); // Writing BitStream data to file
                fp.Close();
            }
            
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.FileName = Radamsa.FileName;
            p.StartInfo.Arguments = Radamsa.Blob.Arguments;
            

            p.Start();

            byte[] bytes;
            if (File.Exists(Radamsa.Blob.TmpFile))
            {
                FileInfo fi = new FileInfo(Radamsa.Blob.TmpFile);
                using (BinaryReader reader = new BinaryReader(File.Open(Radamsa.Blob.TmpFile, FileMode.Open)))
                {
                    bytes = reader.ReadBytes((int)fi.Length);
                    reader.Close();
                }
            }
            else
            {
                bytes = buf;
                logger.Debug("BlobRadamsa: Such file doesn't exists.");
            } // No such file exists
            
            ret.Add(new BitStream(bytes));

            return ret;
        }

        public new static bool supportedDataElement(DataElement obj)
        {
            // Don't attach to elements that are empty
            return Utility.BlobMutator.supportedNonEmptyDataElement(obj);
        }
    }
}
