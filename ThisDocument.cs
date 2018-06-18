/*
 * Revit Macro created by SharpDevelop
 * 用户： 曹政忠
 * 日期: 2018/6/17
 * 时间: 下午 08:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Linq;

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
		public void ViewLevel()
		{
			Autodesk.Revit.DB.Document doc = this.Document;			
			View active = doc.ActiveView;
			ElementId levelId = null;
			
//			Parameter level = active.get_Parameter("關聯的樓層");
			Parameter level = active.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL);
			
			
			FilteredElementCollector lvlCollector = new FilteredElementCollector(doc);
			ICollection<Element> lvlCollection = lvlCollector.OfClass(typeof(Level)).ToElements();
			
			foreach (Element l in lvlCollection)
			{
				Level lvl = l as Level;				
				if(lvl.Name == level.AsString())
				{
					levelId = lvl.Id;
					TaskDialog.Show("test", lvl.Name + "\n"  + lvl.Id.ToString() + level.Id.ToString());
				}
			}
		
		}
		
		public Level  getViewLevel()
		{
			Autodesk.Revit.DB.Document doc = this.Document;			
			View active = doc.ActiveView;
			ElementId levelId = null;
			
//			Parameter level = active.get_Parameter("關聯的樓層");
			Parameter level = active.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL);
			
			
			FilteredElementCollector lvlCollector = new FilteredElementCollector(doc);
			ICollection<Element> lvlCollection = lvlCollector.OfClass(typeof(Level)).ToElements();
			Level lvl=null;
			foreach (Element l in lvlCollection)
			{
				lvl = l as Level;				
				if(lvl.Name == level.AsString())
				{
					levelId = lvl.Id;
					break;
					//TaskDialog.Show("test", lvl.Name + "\n"  + lvl.Id.ToString() + level.Id.ToString());
				}
			}
			return lvl;
		}
		
		private void CreateBeam(Autodesk.Revit.DB.Document document)
		{
			// get the active view's level for beam creation
			Level level = getViewLevel();
			
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
		
		public void CreateManyColumns()
		{
        	using(Transaction trans = new Transaction(this.Document, "CreateManyColumns"))
        	{
        		trans.Start();
        		BatchCreateColumns(this.Document, getViewLevel());
        		trans.Commit();
        	}
		}
		
		private	ICollection<ElementId> BatchCreateColumns(Autodesk.Revit.DB.Document document, Level level)
{
    List<FamilyInstanceCreationData> fiCreationDatas = new List<FamilyInstanceCreationData>();

    ICollection<ElementId> elementSet = null;

    //Try to get a FamilySymbol
    FamilySymbol familySymbol = null;
    FilteredElementCollector collector = new FilteredElementCollector(document);
    ICollection<Element> collection = collector.OfClass(typeof(FamilySymbol)).ToElements();
    foreach (Element e in collection)
    {
        familySymbol = e as FamilySymbol;
        if (null != familySymbol.Category)
        {
            if ("結構柱" == familySymbol.Category.Name)
            {
                break;
            }
        }
    }

    if (null != familySymbol)
    {
        //Create 10 FamilyInstanceCreationData items for batch creation 
        for (int i = 1; i < 11; i++)
        {
            XYZ location = new XYZ(i * 10, 100, 0);
            FamilyInstanceCreationData fiCreationData =
                new FamilyInstanceCreationData(location, familySymbol, level,  StructuralType.Column);

            if (null != fiCreationData)
            {
                fiCreationDatas.Add(fiCreationData);
            }
        }

        if (fiCreationDatas.Count > 0)
        {
            // Create Columns
            elementSet = document.Create.NewFamilyInstances2(fiCreationDatas);
        }
        else
        {
            throw new Exception("Batch creation failed.");
        }
    }
    else
    {
        throw new Exception("No column types found.");
    }

    return elementSet;
}
		public void CreateFloor(){
			Autodesk.Revit.DB.Document doc = this.Document;
 
		    FilteredElementCollector levels 
		      = new FilteredElementCollector( doc )
		        .OfClass( typeof( Level ) );
		 
		    FloorType floorType
		      = new FilteredElementCollector( doc )
		        .OfClass( typeof( FloorType ) )
		        .FirstElement() 
		          as FloorType;
		 
		    Element profileElement
		      = new FilteredElementCollector( doc )
		        .OfClass( typeof( FamilyInstance ) )
		        .OfCategory( BuiltInCategory.OST_GenericModel )
		        .FirstElement() as Element;
		 
		    CurveArray slabCurves = new CurveArray();
		 
		    GeometryElement geo = profileElement.get_Geometry( new Options() );
		 
		    foreach( GeometryInstance inst in geo.Objects )
		    {
		      foreach( GeometryObject obj in inst.SymbolGeometry.Objects )
		      {
		        if( obj is Curve )
		        {
		          slabCurves.Append( obj as Curve );
		        }
		      }
		    }
		 
		    XYZ normal = XYZ.BasisZ;
		 
		    Transaction trans = new Transaction( doc );
		    trans.Start( "Create Floors" );
		 
		    foreach( Level level in levels )
		    {
		      Floor newFloor = doc.Create.NewFloor( slabCurves, 
		        floorType, level, false, normal );
		 
		      newFloor.get_Parameter( 
		        BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM ).Set( 0 );
		    }
		 
		    trans.Commit();
		}

	}
}
