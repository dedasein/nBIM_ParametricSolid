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
            box.AddParameter("Height", "", "Высота параллелепипеда", "[DIM_WIDTH]");
            //Если добавляемых элементов несколько, либо присутствуют вспомогательные элементы, можно воспользоваться методом
            //AddGroupToSubEntity, группирующим несколько элементов и позволяющим управлять группой элементов, как единым примитивом. 


            //В созданный примитив нужно добавить необходимые ему параметры, стандартный набор для 3д примитива.
            box.AddSubEntityMainParams(true, true, false);

            //Обновляем геометрию объекта после добавления свойств
            entity.UpdateElements();

            //Добавление объекта в модель
            //Transaction - транзакции помогают расширить спектр связанных с объектом операций и избежать нежелательных конфликтных ситуаций.
            //Кроме того, они повышают производительность, позволяя отложить операции закрытия всех открытых объектов до момента завершения транзакции.

            using Transaction transaction = database.TransactionManager.StartTransaction();

            //Добавляет новый объект в пространство модели nanoCAD BIM Structure
            BIMStructureMgd.Common.Utilities.AddEntityToDatabase(database, transaction, entity);

            transaction.Commit();
        }
    }
}
