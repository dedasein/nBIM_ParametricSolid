//Пространства имён nanoCAD BIM Конструкции
using BIMStructureMgd.Common;
using BIMStructureMgd.DatabaseObjects;
using BIMStructureMgd.ObjectProperties;
using BIMStructureMgd.ParametricGraphics;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using NrxGate.DatabaseServices;

//Стандартные пространства имён платформы nanoCAD
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;
using WrapperPlatform = NrxGate;
using NativePlatform = Teigha;
using Platform = HostMgd;


namespace nBIM_ParametricSolid
{
    public class ParametricSolids
    {
        [CommandMethod("nBIM_CreateParametricBox")]
        public static void CreateParametricBox()
        {
            // Platform.ApplicationServices.Application.DocumentManager - возвращает коллекцию документов DocumentCollection
            // MdiActiveDocument - активный документ
            Document curDoc = Platform.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            //Teigha.DatabaseServices включает пространства имен и классы, предназначенные для работы с объектами базы данных чертежа.
            //Database - файл чертежа nanoCAD, содержит различные объекты, которые представляют собой файл чертежа 
            Database database = curDoc.Database;

            //Создает параметрический объект на базе объекта из библиотеки компонентов
            var entity = ParametricEntityFactory.Create();

            //GetElementData() - Возвращает дерево объектных свойств для данного параметрического объекта модели
            //AddParameter(s) - Создание нового параметра, который будет виден в редакторе свойств
            entity.GetElementData().AddParameter("DIM_LENGTH").Value = "1000";
            entity.GetElementData().AddParameter("DIM_WIDTH").Value = "1000";
            entity.GetElementData().AddParameter("DIM_HEIGHT").Value = "1000";

            //GetElementParametric - Возвращает параметрические данные объекта (геометрия)
            // ViewMode - выбор режима отображения
            var box = entity.GetElementParametric(ViewMode.Model3D).AddChild("BOX");

            //Добавление в геометрию примитива - параллелепипед
            box.AddParameter("Length", "", "Длина параллелепипеда", "[DIM_LENGTH]");
            box.AddParameter("Width", "", "Ширина параллелепипеда", "[DIM_WIDTH]");
            box.AddParameter("Height", "", "Высота параллелепипеда", "[DIM_HEIGHT]");
            //Если добавляемых элементов несколько, либо присутствуют вспомогательные элементы, можно воспользоваться методом
            //AddGroupToSubEntity, группирующим несколько элементов и позволяющим управлять группой элементов, как единым примитивом. 

            //В созданный примитив нужно добавить необходимые ему параметры, стандартный набор для 3д примитива (НАЙТИ ИНФУ ЧТО ЭТО)
            box.AddSubEntityMainParams(true, true, false);

            //Обновляем геометрию объекта после добавления свойств
            entity.UpdateElements();

            //Добавление ручек управления
            //GetGripData() - возвращает информацию о ручках для редактирования геометрии
            //var boxgrips = entity.GetGripData(ViewMode.Model3D);

            //Добавляем грип длины, вручную собирая свойства (вектор ориентации должен быть перпендикулярен вектору направления?)
            var gripLength = entity.AddGrip("LENGTH", "Длина", "DIM_LENGTH");
            gripLength.SetSplitParameter("Direction", new Vector3d(1, 0, 0));
            gripLength.SetSplitParameter("Orientation", new Vector3d(0, 0, 1));

            var gripWidth = entity.AddGrip("LENGTH", "Ширина", "DIM_WIDTH");
            gripWidth.SetSplitParameter("Direction", new Vector3d(0, 1, 0));
            gripWidth.SetSplitParameter("Orientation", new Vector3d(0, 0, 1));

            var gripHeight = entity.AddGrip("LENGTH", "Высота", "DIM_HEIGHT");
            gripHeight.SetSplitParameter("Direction", new Vector3d(0, 0, 1));
            gripHeight.SetSplitParameter("Orientation", new Vector3d(1, 0, 0));

            //Добавление объекта в модель
            //Transaction - транзакции помогают расширить спектр связанных с объектом операций и избежать нежелательных конфликтных ситуаций.
            //Кроме того, они повышают производительность, позволяя отложить операции закрытия всех открытых объектов до момента завершения транзакции.

            using Transaction transaction = database.TransactionManager.StartTransaction();

            //Добавляет новый объект в пространство модели nanoCAD BIM Structure
            BIMStructureMgd.Common.Utilities.AddEntityToDatabase(database, transaction, entity);

            transaction.Commit();

            //Вывод сообщений в командую строку с запросом к параметрам объекта
            curDoc.Editor.WriteMessage("Длина " + entity.GetElementData().GetValue("DIM_LENGTH", ""));
            curDoc.Editor.WriteMessage("Ширина " + entity.GetElementData().GetValue("DIM_WIDTH", ""));
            curDoc.Editor.WriteMessage("Высота " + entity.GetElementData().GetValue("DIM_HEIGHT", ""));

        }

        

        [CommandMethod("nBIM_BOLTmy")]
        public static void CreateBOLT()
        {
            Document curDoc = Platform.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database database = curDoc.Database;

            var entity = ParametricEntityFactory.Create();

            //Параметры лестницы
            entity.GetElementData().AddParameter("DIM_DIAMETER").Value = "6";
            entity.GetElementData().AddParameter(sName: "bolt_e").Value = "10.9";
            


            var boltGroup = entity.getElementParametric().AddGroup();
            boltGroup.GetParameter("Name").Value = "Шляпка";

            var boltBase = boltGroup.AddChild("EXTRUSION");
            boltBase.AddParameter("Height", "", "", "4");

            var contourLine1 = boltBase.AddChild("LINE");
            contourLine1.AddParameter("ProfilePointX", "0", "", sValueExpr: "");
            contourLine1.AddParameter("ProfilePointY", "", "", "bolt_e/2");

            var contourLine2 = boltBase.AddChild("LINE");
            contourLine2.AddParameter("ProfilePointX", "", "", "sqrt(((bolt_e/2)^2)+((bolt_e/4)^2)-2*(bolt_e/2)*(bolt_e/4)*0.5)");
            contourLine2.AddParameter("ProfilePointY", "", "", "bolt_e/4");

            var contourLine3 = boltBase.AddChild("LINE");
            contourLine3.AddParameter("ProfilePointX", "", "", sValueExpr: "sqrt(((bolt_e/2)^2)+((bolt_e/4)^2)-2*(bolt_e/2)*(bolt_e/4)*0.5)");
            contourLine3.AddParameter("ProfilePointY", "", "", "-1*bolt_e/4");

            var contourLine4 = boltBase.AddChild("LINE");
            contourLine4.AddParameter("ProfilePointX", "0", "", sValueExpr: "");
            contourLine4.AddParameter("ProfilePointY", "", "", "-1*bolt_e/2");

            var contourLine5 = boltBase.AddChild("LINE");
            contourLine5.AddParameter("ProfilePointX", "", "", sValueExpr: "-1*sqrt(((bolt_e/2)^2)+((bolt_e/4)^2)-2*(bolt_e/2)*(bolt_e/4)*0.5)");
            contourLine5.AddParameter("ProfilePointY", "", "", "-1*bolt_e/4");

            var contourLine6 = boltBase.AddChild("LINE");
            contourLine6.AddParameter("ProfilePointX", "", "", sValueExpr: "-1*sqrt(((bolt_e/2)^2)+((bolt_e/4)^2)-2*(bolt_e/2)*(bolt_e/4)*0.5)");
            contourLine6.AddParameter("ProfilePointY", "", "", "bolt_e/4");

            entity.UpdateElements();

            using Transaction transaction = database.TransactionManager.StartTransaction();

            //Добавляет новый объект в пространство модели nanoCAD BIM Structure
            BIMStructureMgd.Common.Utilities.AddEntityToDatabase(database, transaction, entity);

            transaction.Commit();

        }
    }
}

//sqrt(((bolt_e/2)^2)+((bolt_e/4)^2)-2*(bolt_e/2)*(bolt_e/4)*0.5)