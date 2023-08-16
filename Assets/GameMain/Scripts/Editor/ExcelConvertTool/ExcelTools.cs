using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFramework;

namespace StarForce.Editor.DataTableTools
{
    public class ExcelTools : EditorWindow
    {
        /// <summary>
        /// 当前编辑器窗口实例
        /// </summary>
        private static ExcelTools instance;

        /// <summary>
        /// 用于显示所有Excel文件的列表(实际上是所有Excel文件的绝对路径)
        /// </summary>
        private static List<string> excelList;
        
        /// <summary>
        /// 标记表格是否需要转化
        /// </summary>
        private static Dictionary<string, int> excelDictionary;

        /// <summary>
        /// 项目根路径	
        /// </summary>
        private static string pathRoot;

        /// <summary>
        /// Excel文件所在目录
        /// </summary>
        private static string excelDir = "Excels";

        /// <summary>
        /// Excel文件的绝对路径
        /// </summary>
        private static string excelFullPath;

        /// <summary>
        /// 文件转换成json、xml、csv以后的存放路径
        /// </summary>
        private static string targetDir;

        /// <summary>
        /// 滚动窗口初始位置
        /// </summary>
        private static Vector2 scrollPos;

        /// <summary>
        /// 输出格式索引
        /// </summary>
        private static int indexOfFormat = 0;

        /// <summary>
        /// 输出格式
        /// </summary>
        private static string[] formatOption = new string[] { "CSV", "JSON", "XML" };

        /// <summary>
        /// 编码索引
        /// </summary>
        private static int indexOfEncoding = 0;

        /// <summary>
        /// 编码选项
        /// </summary>
        private static string[] encodingOption = new string[] { "UTF-8", "GB2312" };

        /// <summary>
        /// 是否保留原始文件
        /// </summary>
        private static bool isGenerateCode = true;
        
        /// <summary>
        /// 表示excel文件是否被选中的标记
        /// </summary>
        private bool flag;

        /// <summary>
        /// 显示当前窗口	
        /// </summary>
        [MenuItem("Plugins/ExcelTools")]
        static void ShowExcelTools()
        {
            Init();
            instance.Show();
        }

        void OnGUI()
        {
            DrawOptions();
            DrawExport();
        }

        /// <summary>
        /// 绘制插件界面配置项
        /// </summary>
        private void DrawOptions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));
            indexOfFormat = EditorGUILayout.Popup(indexOfFormat, formatOption, GUILayout.Width(125));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("请选择编码类型:", GUILayout.Width(85));
            indexOfEncoding = EditorGUILayout.Popup(indexOfEncoding, encodingOption, GUILayout.Width(125));
            GUILayout.EndHorizontal();

            if (indexOfFormat == 0)
            {
                GUILayout.BeginHorizontal();
                isGenerateCode = GUILayout.Toggle(isGenerateCode, "生成C#脚本");
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            excelDir = EditorGUILayout.TextField("Excel文件路径:", excelDir);
            GUILayout.EndHorizontal();

            excelList.Clear();
            excelFullPath = pathRoot + Utility.Path.GetRegularPath("/" + excelDir);
            if (!Directory.Exists(excelDir))
            {
                EditorGUILayout.LabelField($"不存在目录{excelFullPath}");
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(excelFullPath);
            FileInfo[] files = directoryInfo.GetFiles("*.xlsx");
            foreach (FileInfo file in files)
            {
                excelList.Add(file.Name);
                excelDictionary.TryGetValue(file.Name, out int contain);
                //如果字典中还没有某个excel表的数据，参数contain会返回0，那么久把它加到字典中去，并默认选中
                //”2“表示选中。”1“表示不选中
                if (contain < 1)
                {
                    excelDictionary[file.Name] = 2;
                }
            }
        }

        /// <summary>
        /// 绘制插件界面输出项
        /// </summary>
        private void DrawExport()
        {
            if (excelList == null) return;
            if (excelList.Count < 1)
            {
                EditorGUILayout.LabelField("目前没有Excel文件被选中哦!");
            }
            else
            {
                EditorGUILayout.LabelField("下列项目将被转换为" + formatOption[indexOfFormat] + ":");
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
                foreach (string s in excelList)
                {
                    flag = GUILayout.Toggle(excelDictionary[s] == 2, s);
                    excelDictionary[s] = flag ? 2 : 1;
                }
                GUILayout.EndScrollView();

                //输出
                if (GUILayout.Button("转换"))
                {
                    Convert(isGenerateCode);
                }
            }
        }

        /// <summary>
        /// 转换Excel文件
        /// </summary>
        private static void Convert(bool isGenerateCSCode)
        {
            //excel文件的绝对路径
            string absolutePath;
            //输出文件的全路径
            string output = "";
            foreach (string assetsPath in excelList)
            {
                //获取Excel文件的绝对路径
                absolutePath = excelFullPath + Utility.Path.GetRegularPath('/' + assetsPath);
                //构造Excel工具类
                ExcelUtility excel = new ExcelUtility(absolutePath);

                //判断编码类型
                Encoding encoding = null;
                if (indexOfEncoding == 0)
                {
                    encoding = Encoding.GetEncoding("utf-8");
                }
                else if (indexOfEncoding == 1)
                {
                    encoding = Encoding.GetEncoding("gb2312");
                }

                if (indexOfFormat == 0)
                {
                    output = targetDir + assetsPath.Replace(".xlsx", ".txt");
                    excel.ConvertToCSV(output, encoding);

                    if (isGenerateCSCode)
                    {
                        //生成CS代码
                        int index = assetsPath.LastIndexOf(".");
                        string fileName = assetsPath.Substring(0, assetsPath.Length - index);
                        DataTableGeneratorMenu.GenerateCodeFile(fileName, encoding);   
                    }
                }
                else if (indexOfFormat == 1)
                {
                    output = targetDir + assetsPath.Replace(".xlsx", ".json");
                    excel.ConvertToJson(output, encoding);
                }
                else if (indexOfFormat == 2)
                {
                    output = targetDir + assetsPath.Replace(".xlsx", ".xml");
                    excel.ConvertToXml(output);
                }

                //刷新本地资源
                AssetDatabase.Refresh();
            }

            //转换完后关闭插件
            //这样做是为了解决窗口
            //再次点击时路径错误的Bug
            instance.Close();
        }

        private static void Init()
        {
            //获取当前实例
            instance = EditorWindow.GetWindow<ExcelTools>();
            //初始化
            pathRoot = Application.dataPath;
            targetDir = Utility.Path.GetRegularPath(pathRoot + "/GameMain/DataTables/");
            //注意这里需要对路径进行处理
            //目的是去除Assets这部分字符以获取项目目录
            pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
            excelList = new List<string>();
            excelDictionary = new Dictionary<string, int>();
            scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        }

        void OnSelectionChange()
        {
            //当选择发生变化时重绘窗体
            Show();
            Repaint();
        }
    }
}