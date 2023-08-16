//------------------------------------------------------------
// Game Framework Extension
//------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace StarForce.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        public sealed class SingleListProcessor : GenericDataProcessor<List<float>>
        {
            public override bool IsSystem { get; }
            public override string LanguageKeyword { get; }
            public override string[] GetTypeStrings()
            {
                return new[]
                {
                    "List<float>"
                };
            }
            
            public override List<float> Parse(string value)
            {
                List<float> res = new List<float>();
                string[] splitedValue = value.Split(',');
                for (int i = 0; i < splitedValue.Length; i++)
                {
                    res.Add(float.Parse(splitedValue[i]));
                }
                return res;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                List<float> list = Parse(value);
                for (int i = 0; i < list.Count; i++)
                {
                    binaryWriter.Write(list[i]);
                }
            }
        }
    }   
}