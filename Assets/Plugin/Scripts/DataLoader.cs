using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

//using Microsoft.Office.Interop.Excel;
//using Excel = Microsoft.Office.Interop.Excel;

using normtype = System.Single;
using disttype = System.Single;
using timetype = System.Single;

/**
 * 将Excel数据转换为玩家数据
 */
public class DataLoader
{
	//[UnityEditor.MenuItem("My/LoadData")]
	//public static void LoadPeopleDataByExcel()
	//{
	//	Excel.Application ap = new Excel.Application();
	//	ap.Visible = false;
	//	ap.UserControl = true;
	//	Excel.Workbook wb = ap.Workbooks.Open("Data/People.xls");
	//	Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
	//	int row_count = ws.UsedRange.Cells.Rows.Count;
	//	object[,] exceldata = (object[,])(ws.UsedRange.Cells.get_Value());
	//	for (int i = 1; i < row_count; ++i)
	//	{
	//		PeopleData p = new PeopleData();
	//		p.m_staticStatistic.name = exceldata[i, 2].ToString();
	//	}

	//	ap.Quit();
	//}

}