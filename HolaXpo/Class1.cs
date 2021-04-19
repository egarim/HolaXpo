//using DevExpress.Data.Filtering;
//using DevExpress.Xpo;
//using DevExpress.Xpo.DB;
//using DevExpress.Xpo.Generators;
//using DevExpress.Xpo.Metadata;
//using DevExpress.Xpo.Metadata.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace HolaXpo
//{
//    public class OptimizedPersitentObjectDeleter
//    {
//        private int propertyIndex;

//        private static Random Randomizer
//        {
//            get;
//        }

//        public string TimestampColumnName
//        {
//            get;
//            set;
//        } = "Zeitstempel";

//        private Session Session
//        {
//            get;
//        }
//        public int BatchSize
//        {
//            get;
//            set;
//        } = 500;

//        public OptimizedPersitentObjectDeleter(Session session)
//        {
//            if (session == null)
//                throw new ArgumentNullException(nameof(session));

//            Session = session;
//        }

//        static OptimizedPersitentObjectDeleter()
//        {
//            Randomizer = CreateRandomizer();
//        }

//        public void Delete(IEnumerable<SyncObjectInfo> items, int? gcRecordValue = null)
//        {
//            if (items == null)
//                throw new ArgumentNullException(nameof(items));

//            LargeEnumerable largeList = new LargeEnumerable(items);

//            foreach (IEnumerable<object> batch in largeList.GetBatches(BatchSize))
//            {
//                IEnumerable<SyncObjectInfo> infos = batch.Cast<SyncObjectInfo>();

//                Delete(Session, infos, ref gcRecordValue);
//            }
//        }

//        private void Delete(Session session, IEnumerable<SyncObjectInfo> infos, ref int? gcRecordValue)
//        {
//            if (session == null)
//                throw new ArgumentNullException(nameof(session));
//            if (infos == null)
//                throw new ArgumentNullException(nameof(infos));

//            List<ModificationStatement> statements = new List<ModificationStatement>();

//            List<SyncObjectInfo> objectInfos = infos.OrderBy(i => i.Timestamp).ToList();

//            foreach (SyncObjectInfo info in objectInfos)
//                statements.AddRange(CreateStatements(session, info, ref gcRecordValue));

//            session.DataLayer.ModifyData(statements.ToArray());
//        }

//        private IEnumerable<ModificationStatement> CreateStatements(Session session, SyncObjectInfo info, ref int? gcRecordValue)
//        {
//            if (session == null)
//                throw new ArgumentNullException(nameof(session));
//            if (info == null)
//                throw new ArgumentNullException(nameof(info));

//            bool hasGcRecordField = ClassInfoCache.Instance.HasGcRecordField(info.ClassInfo);

//            if (!hasGcRecordField)
//            {
//                BinaryOperator criteria = new BinaryOperator(info.ClassInfo.KeyProperty.Name, info.KeyValue, BinaryOperatorType.Equal);

//                return CreateDeleteStatement(session, info.ClassInfo, criteria);
//            }

//            if (gcRecordValue == null)
//                gcRecordValue = Randomizer.Next(1, int.MaxValue);

//            return new[] { CreateUpdateGcRecordStatement(info, gcRecordValue.Value) };
//        }

//        private ModificationStatement CreateUpdateGcRecordStatement(SyncObjectInfo info, int gcRecordValue)
//        {
//            if (info == null)
//                throw new ArgumentNullException(nameof(info));

//            if (gcRecordValue <= 0)
//                throw new ArgumentOutOfRangeException(nameof(gcRecordValue), gcRecordValue, "Value for GCRecord must be greater than 0.");

//            DateTime? ts = info.Timestamp ?? DateTime.Now;

//            return CreateStatement(info.ClassInfo, info.KeyValue, ts, gcRecordValue);
//        }

//        private ModificationStatement CreateStatement(XPClassInfo classInfo, object key, DateTime? timestamp, int gcRecord)
//        {
//            if (classInfo == null)
//                throw new ArgumentNullException(nameof(classInfo));
//            if (key == null)
//                throw new ArgumentNullException(nameof(key));

//            DBColumnType keyType = classInfo.Table.GetColumn(classInfo.Table.PrimaryKey.Columns[0]).ColumnType;

//            BinaryOperator critera = new BinaryOperator(new QueryOperand(classInfo.KeyProperty.MappingField, null, keyType), new ParameterValue(propertyIndex++) { Value = key }, BinaryOperatorType.Equal);

//            UpdateStatement stm = new UpdateStatement(classInfo.Table, "TheTable")
//            {
//                Condition = critera
//            };

//            stm.Operands.Add(new QueryOperand(GCRecordField.StaticName, null, DBColumnType.Int32));

//            if (timestamp != null)
//                stm.Operands.Add(new QueryOperand(TimestampColumnName, null, DBColumnType.DateTime));

//            stm.Parameters.Add(new ParameterValue(propertyIndex++) { Value = gcRecord });

//            if (timestamp != null)
//                stm.Parameters.Add(new ParameterValue(propertyIndex++) { Value = timestamp });

//            return stm;
//        }

//        private static IEnumerable<ModificationStatement> CreateDeleteStatement(Session session, XPClassInfo classInfo, CriteriaOperator criteria)
//        {
//            var batchWideData = new BatchWideDataHolder4Modification(session);
//            int recordsAffected = (int)session.Evaluate(classInfo, CriteriaOperator.Parse("Count()"), criteria);

//            ObjectGeneratorCriteriaSet criteriaSet = ObjectGeneratorCriteriaSet.GetCommonCriteriaSet(criteria);

//            List<ModificationStatement> collection = DeleteQueryGenerator.GenerateDelete(classInfo, criteriaSet, batchWideData);

//            foreach (ModificationStatement item in collection)
//                item.RecordsAffected = recordsAffected;

//            return collection;
//        }

//        private static Random CreateRandomizer()
//        {
//            //Randmonizer erstellen und dann ein paar mal ausführen. Damit soll sichergestellt werden, dann der Wert wirklich möglichst Random ist. DateTime.Millisecond hat auch nur max. 1000 Werte ;)  
//            Random randomizer = new Random(DateTime.Now.Millisecond);

//            Random randomizer2 = new Random(DateTime.Now.Day);

//            int limit = randomizer2.Next(0, 100);

//            for (int i = 0; i < limit; i++)
//            {
//                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed  
//                randomizer.Next();
//            }

//            return randomizer;
//        }
//    }
//}
