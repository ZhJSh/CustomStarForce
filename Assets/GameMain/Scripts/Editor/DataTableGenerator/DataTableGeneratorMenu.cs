//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Text;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace StarForce.Editor.DataTableTools
{
    public sealed class DataTableGeneratorMenu
    {
        [MenuItem("Star Force/Generate DataTables")]
        private static void GenerateDataTables()
        {
            foreach (string dataTableName in ProcedurePreload.DataTableNames)
            {
                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
                    break;
                }

                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName);
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName);
            }

            AssetDatabase.Refresh();
        }
        
        public static void GenerateCodeFile(string path, Encoding encoding)
        {
            DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(path, encoding);
            if (!DataTableGenerator.CheckRawData(dataTableProcessor, path))
            {
                Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", path));
            }
            
            DataTableGenerator.GenerateCodeFile(dataTableProcessor, path);
            AssetDatabase.Refresh();
        }
    }
}
