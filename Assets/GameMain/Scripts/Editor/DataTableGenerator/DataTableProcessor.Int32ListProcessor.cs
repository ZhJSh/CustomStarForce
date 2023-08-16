//------------------------------------------------------------
// Game Framework Extension
//------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace StarForce.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        public sealed class Int32ListProcessor : GenericDataProcessor<List<int>>
        {
            public override bool IsSystem { get; }
            public override string LanguageKeyword { get; }
            public override string[] GetTypeStrings()
            {
                return new[]
                {
                    "List<int>"
                };
            }
            
            public override List<int> Parse(string value)
            {
                List<int> res = new List<int>();
                string[] splitedValue = value.Split(',');
                for (int i = 0; i < splitedValue.Length; i++)
                {
                    res.Add(int.Parse(splitedValue[i]));
                }
                return res;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                List<int> list = Parse(value);
                for (int i = 0; i < list.Count; i++)
                {
                    binaryWriter.Write(list[i]);
                }
            }
        }
    }   
}