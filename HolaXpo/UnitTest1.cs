using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using NUnit.Framework;
using System;

namespace HolaXpo
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {


        }

        [Test]
        public void Test1()
        {

            IDataStore s = XpoDefault.GetConnectionProvider(connection, AutoCreateOption.DatabaseAndSchema);
            XPDictionary d = new ReflectionDictionary();
            XpoDefault.Dictionary = d;
            XpoDefault.DataLayer = new SimpleDataLayer(d, s);

            XPClassInfo order = CreateObject(s, "Order", typeof(XPObject));
            XPClassInfo customer = CreateObject(s, "Customer", typeof(XPObject));


            AssociationAttribute a = new AssociationAttribute("CustomerOrders"); //, typeof(DetailDataObject)
            a.ElementTypeName = "Order";

            AssociationAttribute a1 = new AssociationAttribute("CustomerOrders"); //, typeof(ParentDataObject)

            order.CreateMember("Customer", customer, new Attribute[] { a1 });

            customer.CreateMember("Orders", typeof(XPCollection), true, new Attribute[] { a });

            customer.CreateMember("Name", typeof(string));
            customer.CreateMember("DoB", typeof(DateTime));

            Session session = new Session(XpoDefault.DataLayer);
            session.UpdateSchema(order, customer);

            UnitOfWork WorkingSession = new UnitOfWork(XpoDefault.DataLayer);



            XPBaseObject Joche = customer.CreateObject(WorkingSession) as XPBaseObject;

            Joche.SetMemberValue("Name", "Jose Manuel Ojeda Melgar");
            Joche.SetMemberValue("DoB",DateTime.Now);
            WorkingSession.CommitChanges();


            Assert.Pass();
        }
        private XPClassInfo CreateObject(IDataStore s, string name, Type baseType)
        {
            DBTable[] tables = ((IDataStoreSchemaExplorer)s).GetStorageTables(name);
            DBTable table = tables[0];
            XPClassInfo info = XpoDefault.Dictionary.CreateClass(XpoDefault.Dictionary.QueryClassInfo(baseType), table.Name);
            string key = "Oid";//table.PrimaryKey.Columns[0];
            foreach (DBColumn c in table.Columns)
            {
                XPMemberInfo mi = info.CreateMember(c.Name, DBColumn.GetType(c.ColumnType));
                if (key == c.Name)
                    mi.AddAttribute(new KeyAttribute(c.IsIdentity));
            }
            return info;
        }


        string connection = SQLiteConnectionProvider.GetConnectionString(@"nwind.sqlite");

     


    }
}