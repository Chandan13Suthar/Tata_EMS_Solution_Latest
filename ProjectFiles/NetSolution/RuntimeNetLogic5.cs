#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.DataLogger;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.UI;
using FTOptix.CoreBase;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using Store = FTOptix.Store;
using System.Text.RegularExpressions;
using FTOptix.SQLiteStore;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Reflection.Emit;
using FTOptix.MicroController;
using System.Collections.Generic;
#endregion
public class RuntimeNetLogic5 : BaseNetLogic
{


    public override void Start()
    {
        var owner = (HourlyDataAggregation)LogicObject.Owner;


        dateVariable = owner.DateVariable;
        buttonVariable = owner.ButtonVariable;
        consumptionVariable = owner.ConsumptionVariable;
        jaceVariable = owner.JaceVariable;
        meterVariable = owner.MeterVariable;

        periodicTask = new PeriodicTask(IncrementDecrementTask, 2000, LogicObject);
        periodicTask.Start();
    }

    public override void Stop()
    {

        periodicTask.Dispose();
        periodicTask = null;
    }

    public void IncrementDecrementTask()
    {

        string jace = jaceVariable.Value;
        string meter = meterVariable.Value;
        float consumption = consumptionVariable.Value;
        bool button = buttonVariable.Value;
        DateTime date = dateVariable.Value;

        var project = FTOptix.HMIProject.Project.Current;

        var myStore1 = project.GetObject("DataStores").Get<Store.Store>("ODBCDatabase");// Date 
        var myStore2 = project.GetObject("DataStores").Get<Store.Store>("ODBCDatabase");//Meter
        var myStore3 = project.GetObject("DataStores").Get<Store.Store>("ODBCDatabase");//Jace
        var myStore4 = project.GetObject("DataStores").Get<Store.Store>("ODBCDatabase");//Consumption


        object[,] resultSet1;
        string[] header1;
        object[,] resultSet2;
        string[] header2;
        object[,] resultSet3;
        string[] header3;
        object[,] resultSet4;
        string[] header4;


        if (button == true)
        {

            string query1 = "SELECT  Date FROM ConsumptionDistribution GROUP BY Date, Jace, Meter";
            string query2 = "SELECT  Jace FROM ConsumptionDistribution GROUP BY Date, Jace, Meter";
            string query3 = "SELECT  Meter FROM ConsumptionDistribution GROUP BY Date, Jace, Meter";
            string query4 = "SELECT  SUM(Consumption) AS Consumption FROM ConsumptionDistribution GROUP BY Date, Jace, Meter";


            myStore1.Query(query1, out header1, out resultSet1);
            myStore2.Query(query2, out header2, out resultSet2);
            myStore3.Query(query3, out header3, out resultSet3);
            myStore4.Query(query4, out header4, out resultSet4);





            var rowCount1 = resultSet1 != null ? resultSet1.GetLength(0) : 0;
            var columnCount1 = header1 != null ? header1.Length : 0;
            if (rowCount1 > 0 && columnCount1 > 0)
            {
                var column1 = Convert.ToDateTime(resultSet1[0, 0]);
                var Date = column1;
                date = Date;
            }



            var rowCount2 = resultSet2 != null ? resultSet2.GetLength(0) : 0;
            var columnCount2 = header2 != null ? header2.Length : 0;
            if (rowCount2 > 0 && columnCount2 > 0)
            {
                var column1 = Convert.ToString(resultSet2[0, 0]);
                var Jace = column1;
                jace = Jace;
            }


            var rowCount3 = resultSet3 != null ? resultSet3.GetLength(0) : 0;
            var columnCount3 = header3 != null ? header3.Length : 0;
            if (rowCount3 > 0 && columnCount3 > 0)
            {
                var column1 = Convert.ToString(resultSet3[0, 0]);
                var Meter = column1;
                meter = Meter;
            }


            var rowCount4 = resultSet4 != null ? resultSet4.GetLength(0) : 0;
            var columnCount4 = header4 != null ? header4.Length : 0;
            if (rowCount4 > 0 && columnCount4 > 0)
            {
                var column1 = Convert.ToInt32(resultSet4[0, 0]);
                var Consumption = column1;
                consumption = Consumption;
            }


            dateVariable.Value = date;
            jaceVariable.Value = jace;
            meterVariable.Value = meter;
            consumptionVariable.Value = consumption;
        }







    }

    private IUAVariable dateVariable;
    private IUAVariable buttonVariable;
    private IUAVariable consumptionVariable;
    private IUAVariable jaceVariable;
    private IUAVariable meterVariable;
    private PeriodicTask periodicTask;


}