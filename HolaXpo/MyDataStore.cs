using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HolaXpo
{

    public class MyDataStore:IDataStore
    {
        IDataStore IDataStore;
        public MyDataStore()
        {
            
        }
        public MyDataStore(IDataStore iDataStore)
        {
            IDataStore = iDataStore;
        }

        public AutoCreateOption AutoCreateOption => this.IDataStore.AutoCreateOption;

        public ModificationResult ModifyData(params ModificationStatement[] dmlStatements)
        {
            ModificationResult modificationResult = this.IDataStore.ModifyData(dmlStatements);
            return modificationResult;
        }

        public SelectedData SelectData(params SelectStatement[] selects)
        {
           return    this.IDataStore.SelectData(selects);
        }

        public UpdateSchemaResult UpdateSchema(bool doNotCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            return this.IDataStore.UpdateSchema(doNotCreateIfFirstTableNotExist, tables);
        }
    }
    //public class MyDataStore : IDataStore
    //{
    //    private IDataStore dataStore;
    //    public MyDataStore(IDataStore dataStore, AutoCreateOption autoCreateOption)
    //    {
    //        this.dataStore = dataStore;
    //        AutoCreateOption = autoCreateOption;
    //    }
    //    public AutoCreateOption AutoCreateOption { get; set; }

    //    public ModificationResult ModifyData(params ModificationStatement[] dmlStatements)
    //    {
    //        foreach (ModificationStatement modificationStatement in dmlStatements)
    //        {
    //            Debug.WriteLine("Operands");
    //            foreach (DevExpress.Data.Filtering.CriteriaOperator criteriaOperator in modificationStatement.Operands)
    //            {
    //                Debug.WriteLine(criteriaOperator.GetType());
    //            }
    //            Debug.WriteLine("parameters");
    //            foreach (DevExpress.Data.Filtering.OperandValue operandValue in modificationStatement.Parameters)
    //            {
    //                Debug.WriteLine(operandValue.GetType());
    //            }
    //            Debug.WriteLine(modificationStatement.ToString());

    //        }

    //        return dataStore.ModifyData(dmlStatements);
    //    }

    //    public SelectedData SelectData(params SelectStatement[] selects)
    //    {
    //        return dataStore.SelectData(selects);
    //    }

    //    public UpdateSchemaResult UpdateSchema(bool doNotCreateIfFirstTableNotExist, params DBTable[] tables)
    //    {
    //        return dataStore.UpdateSchema(doNotCreateIfFirstTableNotExist, tables);
    //    }
    //}
}
