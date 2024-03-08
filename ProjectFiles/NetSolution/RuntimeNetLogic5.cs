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
        int meter = meterVariable.Value;

        bool button = buttonVariable.Value;
        var project = FTOptix.HMIProject.Project.Current;
        var myStore1 = project.GetObject("DataStores").Get<Store.Store>("ODBCDatabase");

        object[,] resultSet1;
        string[] header1;


        if (button == true)
        {
            string currentHour = DateTime.Now.ToString("HH");
            string query1 = $"SELECT Meter FROM Home Page WHERE LocalTime  BETWEEN '" + currentHour + " 00:00:00' AND '" + currentHour + " 23:59:59' AND Jace = '33KV'  AND Meter = 'INCOMER1'  ";

            myStore1.Query(query1, out header1, out resultSet1);



            var rowCount1 = resultSet1 != null ? resultSet1.GetLength(0) : 0;
            var columnCount1 = header1 != null ? header1.Length : 0;
            if (rowCount1 > 0 && columnCount1 > 0)
            {
                var column1 = Convert.ToInt32(resultSet1[0, 0]);
                var Meter = column1;
                meter = Meter;
            }

            meterVariable.Value = meter;
        }
     }








    private IUAVariable dateVariable;
    private IUAVariable buttonVariable;
    private IUAVariable consumptionVariable;
    private IUAVariable jaceVariable;
    private IUAVariable meterVariable;
    private PeriodicTask periodicTask;


}