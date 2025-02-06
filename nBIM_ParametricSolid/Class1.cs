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

    public class Bolt
    {
        public string Bolt_d;
        public string Bolt_e;
        public string Bolt_k;
        public Bolt(string bolt_d, string bolt_e, string bolt_k)
        {
            Bolt_d = bolt_d;
            Bolt_e = bolt_e;
            Bolt_k = bolt_k;
        }
    }

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




        [CommandMethod("nBIM_myBOLT")]
        public static void CreateBOLT()
        {
            Document curDoc = Platform.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database database = curDoc.Database;

            var entity = ParametricEntityFactory.Create();

            Bolt boltM6 = new Bolt("6", "10.9", "4");
            Bolt boltM8 = new Bolt("8", "14.2", "5.3");
            Bolt boltM10 = new Bolt("10", "17.6", "6.4");
            Bolt boltM12 = new Bolt("12", "19.9", "7.5");
            Bolt boltM14 = new Bolt("14", "22.8", "8.8");
            Bolt boltM16 = new Bolt("16", "26.2", "10");
            Bolt boltM18 = new Bolt("18", "29.6", "12");
            Bolt boltM20 = new Bolt("20", "33.0", "12.2");
            Bolt boltM22 = new Bolt("22", "37.3", "14");
            Bolt boltM24 = new Bolt("24", "39.6", "15");
            Bolt boltM27 = new Bolt("27", "45.2", "17");
            Bolt boltM30 = new Bolt("30", "50.9", "18.7");
            Bolt boltM36 = new Bolt("36", "60.8", "22.5");
            Bolt boltM42 = new Bolt("42", "71.3", "26");
            Bolt boltM48 = new Bolt("48", "82.6", "30");


            string stringForBolt_e = $"=case([DIM_DIAMETER] " +
                $"when {boltM6.Bolt_d} then {boltM6.Bolt_e}, " +
                $"when {boltM8.Bolt_d} then {boltM8.Bolt_e}, " +
                $"when {boltM10.Bolt_d} then {boltM10.Bolt_e}, " +
                $"when {boltM12.Bolt_d} then {boltM12.Bolt_e}, " +
                $"when {boltM14.Bolt_d} then {boltM14.Bolt_e}, " +
                $"when {boltM16.Bolt_d} then {boltM16.Bolt_e}, " +
                $"when {boltM18.Bolt_d} then {boltM18.Bolt_e}, " +
                $"when {boltM20.Bolt_d} then {boltM20.Bolt_e}, " +
                $"when {boltM22.Bolt_d} then {boltM22.Bolt_e}, " +
                $"when {boltM24.Bolt_d} then {boltM24.Bolt_e}, " +
                $"when {boltM27.Bolt_d} then {boltM27.Bolt_e}, " +
                $"when {boltM30.Bolt_d} then {boltM30.Bolt_e}, " +
                $"when {boltM36.Bolt_d} then {boltM36.Bolt_e}, " +
                $"when {boltM42.Bolt_d} then {boltM42.Bolt_e}, " +
                $"when {boltM48.Bolt_d} then {boltM48.Bolt_e}, " +
                $"else \"100\" and Debug.PrintLn(\"Введите значение диаметра болта соответствующее ГОСТ\"))";

            string stringForBolt_k = $"=case([DIM_DIAMETER] " +
                $"when {boltM6.Bolt_d} then {boltM6.Bolt_k}, " +
                $"when {boltM8.Bolt_d} then {boltM8.Bolt_k}, " +
                $"when {boltM10.Bolt_d} then {boltM10.Bolt_k}, " +
                $"when {boltM12.Bolt_d} then {boltM12.Bolt_k}, " +
                $"when {boltM14.Bolt_d} then {boltM14.Bolt_k}, " +
                $"when {boltM16.Bolt_d} then {boltM16.Bolt_k}, " +
                $"when {boltM18.Bolt_d} then {boltM18.Bolt_k}, " +
                $"when {boltM20.Bolt_d} then {boltM20.Bolt_k}, " +
                $"when {boltM22.Bolt_d} then {boltM22.Bolt_k}, " +
                $"when {boltM24.Bolt_d} then {boltM24.Bolt_k}, " +
                $"when {boltM27.Bolt_d} then {boltM27.Bolt_k}, " +
                $"when {boltM30.Bolt_d} then {boltM30.Bolt_k}, " +
                $"when {boltM36.Bolt_d} then {boltM36.Bolt_k}, " +
                $"when {boltM42.Bolt_d} then {boltM42.Bolt_k}, " +
                $"when {boltM48.Bolt_d} then {boltM48.Bolt_k}, " +
                $"else \"100\" and Debug.PrintLn(\"Введите значение диаметра болта соответствующее ГОСТ\"))";

            //Параметры болта
            entity.GetElementData().AddParameter("DIM_DIAMETER").Value = "6";
            entity.GetElementData().AddParameter("bolt_e", "", "Диаметр описанной окружности", stringForBolt_e);
            entity.GetElementData().AddParameter("bolt_k", "", "Высота шляпки", stringForBolt_k);

            var boltGroup = entity.getElementParametric().AddGroup();
            boltGroup.GetParameter("Name").Value = "Шляпка";

            var boltBase = boltGroup.AddChild("EXTRUSION");
            boltBase.AddParameter("Height", "", "", "[bolt_k]");

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

            curDoc.Editor.WriteMessage($"Диаметр {boltM6.Bolt_d}");

        }
    }
}

//sqrt(((bolt_e/2)^2)+((bolt_e/4)^2)-2*(bolt_e/2)*(bolt_e/4)*0.5)