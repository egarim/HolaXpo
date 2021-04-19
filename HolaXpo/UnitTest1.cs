using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            var StoreType = s.GetType();
            Debug.WriteLine(StoreType);

            //DevExpress.Xpo.DB.SQLiteConnectionProvider

            XPDictionary d = new ReflectionDictionary();
            XpoDefault.Dictionary = d;
            XpoDefault.DataLayer = new SimpleDataLayer(d, new MyDataStore(s));

            XPClassInfo order = CreateObject(s, "Order", typeof(MyBaseObject));
            XPClassInfo customer = CreateObject(s, "Customer", typeof(MyBaseObject));


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



            MyBaseObject Joche = customer.CreateObject(WorkingSession) as MyBaseObject;

            Joche.SetMemberValue("Name", "Jose Manuel Ojeda Melgar");
            Joche.SetMemberValue("DoB",DateTime.Now);
            WorkingSession.CommitChanges();


            //Joche.SetMemberValue("Name", "Joche");
            //WorkingSession.CommitChanges();

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("Name", "Joche");
            keyValuePairs.Add("DoB", DateTime.Now);

            //CreateStatement(customer, Joche.GetMemberValue(customer.KeyProperty.Name), DateTime.Now, 1);
            CreateStatement(customer, Joche.GetMemberValue(customer.KeyProperty.Name), keyValuePairs);



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

        private ModificationStatement CreateStatement(XPClassInfo classInfo, object key, DateTime? timestamp, int gcRecord)
        {
            var propertyIndex = 0;
            if (classInfo == null)
                throw new ArgumentNullException(nameof(classInfo));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            DBColumnType keyType = classInfo.Table.GetColumn(classInfo.Table.PrimaryKey.Columns[0]).ColumnType;

            BinaryOperator critera = new BinaryOperator(new QueryOperand(classInfo.KeyProperty.MappingField, null, keyType), new ParameterValue(propertyIndex++) { Value = key }, BinaryOperatorType.Equal);

            UpdateStatement stm = new UpdateStatement(classInfo.Table, "TheTable")
            {
                Condition = critera
            };

            stm.Operands.Add(new QueryOperand(GCRecordField.StaticName, "N"+ propertyIndex++, DBColumnType.Int32));

            if (timestamp != null)
                stm.Operands.Add(new QueryOperand("TimestampColumnName", "N" + propertyIndex++, DBColumnType.DateTime));

            stm.Parameters.Add(new ParameterValue(propertyIndex++) { Value = gcRecord });

            if (timestamp != null)
                stm.Parameters.Add(new ParameterValue(propertyIndex++) { Value = timestamp });
            Debug.WriteLine(stm);
            return stm;
        }
        private ModificationStatement CreateStatement(XPClassInfo classInfo, object key, Dictionary<string, object> values)
        {
            if (classInfo == null)
                throw new ArgumentNullException(nameof(classInfo));

            var PropertyIndex = 0;

            DBColumnType keyType = classInfo.Table.GetColumn(classInfo.Table.PrimaryKey.Columns[0]).ColumnType;

            BinaryOperator critera = new BinaryOperator(new QueryOperand(classInfo.KeyProperty.MappingField, null, keyType), new ParameterValue(PropertyIndex) { Value = key }, BinaryOperatorType.Equal);

            UpdateStatement stm = new UpdateStatement(classInfo.Table, "TheTable")
            {
                Condition = critera
            };
            
            foreach (var item in values)
            {
                DBColumnType mappingFieldDBType = classInfo.GetPersistentMember(item.Key).MappingFieldDBType;
                stm.Operands.Add(new QueryOperand(item.Key,"N"+ PropertyIndex, mappingFieldDBType));
                stm.Parameters.Add(new ParameterValue(PropertyIndex) { Value = item.Value, DBType= mappingFieldDBType });
                PropertyIndex++;
            }
            Debug.WriteLine(stm.ToString());
            //stm.Operands.Add(new QueryOperand(GCRecordField.StaticName, null, DBColumnType.Int32));

            //if (timestamp != null)
            //    stm.Operands.Add(new QueryOperand(TimestampColumnName, null, DBColumnType.DateTime));

            //stm.Parameters.Add(new ParameterValue(propertyIndex++) { Value = gcRecord });

            //if (timestamp != null)
            //    stm.Parameters.Add(new ParameterValue(propertyIndex++) { Value = timestamp });

            return stm;
        }


        string connection = SQLiteConnectionProvider.GetConnectionString(@"nwind.sqlite");

     


    }
}