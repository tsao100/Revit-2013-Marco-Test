/*
 * Revit Macro created by SharpDevelop
 * 用户： 董萬吉
 * 日期: 2018/6/17
 * 时间: 下午 08:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace MyModule
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.UI.Macros.AddInId("1AC7CA8D-74D6-4012-9032-7172A9C04DA5")]
	public partial class ThisDocument
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		public void CreateABeam()
		{
        	using(Transaction trans = new Transaction(this.Document, "CreateABeam"))
        	{
        		trans.Start();
        		CreateBeam(this.Document);
        		trans.Commit();
        	}
		}
		private void CreateBeam(Document document)
		{
						// get the active view's level for beam creation
			Level level = document.ActiveView.Level;
			
			// load a family symbol from file
			FamilySymbol gotSymbol = null;
			String fileName = @"C:\ProgramData\Autodesk\RVT 2013\Libraries\Chinese_Trad_INTL\結構框架\鋼\M_W-寬凸緣.rfa";
			String name = "W250X80";
			
			FamilyInstance instance = null;
			
			if (document.LoadFamilySymbol(fileName, name, out gotSymbol))
			{
				// look for a model line in the list of selected elements
				UIDocument uidoc = new UIDocument(document);
				ElementSet sel = uidoc.Selection.Elements;
				ModelLine modelLine = null;
				foreach (Autodesk.Revit.DB.Element elem in sel)
				{
					if (elem is ModelLine)
					{
						modelLine = elem as ModelLine;
						if (null != modelLine)
						{
							// create a new beam
							instance = document.Create.NewFamilyInstance(modelLine.GeometryCurve, gotSymbol, level, StructuralType.Beam);
						}
						else
						{
							throw new Exception("Please select a model line before invoking this command");
						}
					}
				}
			
			}
			else
			{
				throw new Exception("Couldn't load " + fileName);
			}
		}
	}
}
