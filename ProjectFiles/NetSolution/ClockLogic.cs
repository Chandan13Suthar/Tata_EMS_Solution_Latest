#region Using directives
using FTOptix.NetLogic;
using System;
using UAManagedCore;
using FTOptix.AuditSigning;
using FTOptix.MicroController;
using FTOptix.CommunicationDriver;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
#endregion

public class ClockLogic : BaseNetLogic {
    public override void Start() {
        periodicTask = new PeriodicTask(UpdateTime, 1000, LogicObject);
        periodicTask.Start();
    }

    public override void Stop() {
        periodicTask.Dispose();
        periodicTask = null;
    }

    private void UpdateTime() {
        LogicObject.GetVariable("Time").Value = DateTime.Now;
        LogicObject.GetVariable("UTCTime").Value = DateTime.UtcNow;
    }

    private PeriodicTask periodicTask;
}
