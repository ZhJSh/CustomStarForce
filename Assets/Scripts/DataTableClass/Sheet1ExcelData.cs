/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class Sheet1ExcelItem : ExcelItemBase
{
	/// <summary>
	/// id
	/// </summary>>
	public int id;
	/// <summary>
	/// 字段1
	/// </summary>>
	public int testInt1;
	/// <summary>
	/// 字段2
	/// </summary>>
	public string testStr1;
}


public class Sheet1ExcelData : ExcelDataBase<Sheet1ExcelItem>
{
	public Sheet1ExcelItem[] items;

	public Dictionary<int,Sheet1ExcelItem> itemDic = new Dictionary<int,Sheet1ExcelItem>();

	public void Init()
	{
		itemDic.Clear();
		if(items != null && items.Length > 0)
		{
			for(int i = 0; i < items.Length; i++)
			{
				itemDic.Add(items[i].id, items[i]);
			}
		}
	}

	public Sheet1ExcelItem GetSheet1ExcelItem(int id)
	{
		if(itemDic.ContainsKey(id))
			return itemDic[id];
		else
			return null;
	}
	#region --- Get Method ---

	public int GetTestInt1(int id)
	{
		var item = GetSheet1ExcelItem(id);
		if(item == null)
			return default;
		return item.testInt1;
	}

	public string GetTestStr1(int id)
	{
		var item = GetSheet1ExcelItem(id);
		if(item == null)
			return default;
		return item.testStr1;
	}

	#endregion
}



