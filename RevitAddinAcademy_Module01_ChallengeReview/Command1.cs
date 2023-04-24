#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RevitAddinAcademy_Module01_ChallengeReview
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 1. set variables
            int numFloors = 250;
            double currentElev = 0;
            int floorHeight = 15;

            // 7. get titleblock
            FilteredElementCollector tbCollector = new FilteredElementCollector(doc);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            ElementId tblockId = tbCollector.FirstElementId();

            // 8. get view family types
            FilteredElementCollector vftCollector = new FilteredElementCollector(doc);
            vftCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType fpVFT = null;
            ViewFamilyType cpVFT = null;

            foreach(ViewFamilyType curVFT in vftCollector)
            {
                if(curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    fpVFT = curVFT;
                }
                else if(curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    cpVFT = curVFT;
                }
            }

            // 9. create transaction
            Transaction t = new Transaction(doc);
            t.Start("FIZZ BUZZ Challenge");

            int fizzbuzzCounter = 0;
            int fizzCounter = 0;
            int buzzCounter = 0;

            // 2. loop through floors and check FIZZBUZZ
            for (int i = 1; i <= numFloors; i++)
            {
                // 3. create level
                Level newLevel = Level.Create(doc, currentElev);
                newLevel.Name = "LEVEL " + i.ToString();
                currentElev += floorHeight;

                // 4. check for FIZZBUZZ
                if (i % 3 == 0 && i % 5 == 0)
                {
                    // FIZZBUZZ = create a sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.SheetNumber = "RAB-" + i.ToString();
                    newSheet.Name = "FIZZBUZZ-" + i.ToString();

                    // BONUS 
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZBUZZ-" + i.ToString();

                    XYZ insPoint = new XYZ(1.5, 1, 0);
                    Viewport newVP = Viewport.Create(doc, newSheet.Id, newFloorPlan.Id, insPoint);

                    fizzbuzzCounter++;

                }
                else if (i % 3 == 0)
                {
                    // FIZZ = create a floor plan
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZ-" + i.ToString();
                    fizzCounter++;
                }
                else if (i % 5 == 0)
                {
                    // BUZZ = create a ceiling plan
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, cpVFT.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ-" + i.ToString();
                    buzzCounter++;
                }
            }

            t.Commit();
            t.Dispose();

            // 11. alert user
            TaskDialog.Show("Complete", $"Created {numFloors} levels. Created {fizzbuzzCounter} FIZZBUZZ sheets. Created {fizzCounter} FIZZ floor plans. Created {buzzCounter}"
                + " BUZZ ceiling plans"); ;

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
