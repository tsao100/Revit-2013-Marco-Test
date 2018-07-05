/*
 * 由SharpDevelop创建。
 * 用户： jack.tsao
 * 日期: 2018/7/4
 * 时间: 上午 11:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace MyAppMacro
{
	/// <summary>
	/// Description of LevelIsolator.
	/// </summary>
	public partial class LevelIsolator : System.Windows.Forms.Form
	{
		private Document m_doc;
		private UIDocument m_uidoc;
		private IList<Level> m_levels=null;
		public LevelIsolator(Document doc, UIDocument uidoc)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			m_doc = doc;
			m_uidoc = uidoc;
			
			m_levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(l => l.Elevation).ToList();
			foreach (Level level in m_levels) {
				cbLevel.Items.Add(level.Name);
			}
			
			cbLevel.SelectedIndex=0;
		}
		
		void BtnApplyClick(object sender, EventArgs e)
		{
			Level curLevel = m_levels.ElementAt(cbLevel.SelectedIndex);
			//cbLevel.Text;
			
//			TaskDialog.Show("Current selected Level", curLevel.Name);
			IEnumerable<Element> Allelem= JtElementExtensionMethods.SelectAllPhysicalElements(m_doc);
			List<ElementId> eIds= new List<ElementId>();
			Parameter a1,a2,a3, a4, a5, a6, a7;
			IEnumerator<Element> a = Allelem.GetEnumerator();
			while (a.MoveNext()) {
				Element e1 = a.Current;
				a1 = e1.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
				a2 = e1.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
				a3 = e1.get_Parameter(BuiltInParameter.LEVEL_PARAM);
				a4 = e1.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
				a5 = e1.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
				a6 = e1.get_Parameter(BuiltInParameter.STAIRS_BASE_LEVEL_PARAM);
				a7 = e1.get_Parameter(BuiltInParameter.STAIRS_RAILING_BASE_LEVEL_PARAM);
			try
			{

				if (null != a1)
				{
					Level a1_Level = m_doc.GetElement(a1.AsElementId())  as Level;
					if(a1_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
				if (null != a2)
				{
					Level a2_Level = m_doc.GetElement(a2.AsElementId())  as Level;
					if(a2_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
				if (null != a3)
				{
					Level a3_Level = m_doc.GetElement(a3.AsElementId())  as Level;
					if(a3_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
				if (null != a4)
				{
					Level a4_Level = m_doc.GetElement(a4.AsElementId())  as Level;
					if(a4_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
				if (null != a5)
				{
					Level a5_Level = m_doc.GetElement(a5.AsElementId())  as Level;
					if(a5_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
				if (null != a6)
				{
					Level a6_Level = m_doc.GetElement(a6.AsElementId())  as Level;
					if(a6_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
				if (null != a7)
				{
					Railing e2 = e1 as Railing;
					if(e2.HasHost)
					{
					Element e3 = m_doc.GetElement(e2.HostId);
					a7 = e3.get_Parameter(BuiltInParameter.STAIRS_BASE_LEVEL_PARAM);
					}					
					Level a7_Level = m_doc.GetElement(a7.AsElementId())  as Level;
					if(a7_Level.Id != curLevel.Id)
						//m_uidoc.ActiveView.HideElementTemporary(e1.Id);
						eIds.Add(e1.Id);
				}
			
//			try
//			{
			}
			catch (Exception)
			{
			     //TaskDialog.Show("Exception", ex.ToString());
			}
			}
			m_uidoc.ActiveView.HideElements(eIds);
			m_uidoc.RefreshActiveView();

			this.Close();
		}
	}

	public static class JtElementExtensionMethods
	{
		public static bool IsPhysicalElement(this Element e)
		{
			if( e.Category == null ) return false;
			if( e.ViewSpecific ) return false;
			// exclude specific unwanted categories
			if( ( (BuiltInCategory) e.Category.Id.IntegerValue ) == BuiltInCategory.OST_HVAC_Zones ) return false;
			
			return e.Category.CategoryType == CategoryType.Model && e.Category.CanAddSubcategory;
		}
	
		public static IEnumerable<Element> SelectAllPhysicalElements(Document doc)
		{
		  return new FilteredElementCollector( doc )
		    .WhereElementIsNotElementType()
		    .Where( e => e.IsPhysicalElement() );
		}
	}
}
