using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using LabMCESystem.DataBase;
using LabMCESystem.LabElement;
using LabMCESystem.BaseService.ExperimentDataExchange;

namespace LabMCESystem.BaseService
{
    [Serializable]
    public class MesCtrlDataCenterBase : IMesCtrlDataExchange, IMulDevDataConnect
    {
        #region Fields

        const string xmlFileName = @".\Experiment Exchange Data Set.xml";

        // use a data set to save all of the experiment data.
        // there is RT Data Load Temp Table,Data Buffer Table, Sensor Calibrate Info Table, Experiment Point RT Value Converters Info Table.
        private ExpDataSet _expDataSet;

        private ILabElementManageable _labElemntManger;

        // sensors calibraters
        private Dictionary<string, ISensorCalibrate> _sensorCalibraters;

        // experiment value converters
        private Dictionary<int, IExpValueConvert> _valueConverters;


        #endregion

        #region Properties

        public List<ExpDataExchangeLoadArgs> CurrentLoad
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISingleExpRTDataRead SingleDataReader
        {
            get
            {
                return this;
            }
        }

        public IExpDataPush DataPusher
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region Build

        public MesCtrlDataCenterBase()
        {
            _expDataSet = new ExpDataSet();
        }


        #endregion

        #region Methods

        public void WriteXml()
        {
            _expDataSet.WriteXml(xmlFileName);
        }

        public void ReadXml()
        {
            _expDataSet.ReadXml(xmlFileName);
        }

        public void ConnectLabElementManager(ILabElementManageable manager)
        {
            _labElemntManger = manager;
        }

        #region private

        private void InitialFromElemntManager(ILabElementManageable eManager)
        {
            List<LabChannel> chs = eManager.AllChannels;
            // Update RTDataLoadTable
            _expDataSet.RTDataLoadTable.Clear();
            foreach (var ch in chs)
            {
                //_expDataSet.RTDataLoadTable.AddRTDataLoadTableRow(ch.KeyCode, DateTime.Now, DateTime.Now, 0, 0, 0);
            }

            List<MeasureSensor> mss = eManager.AllSensors;
            foreach (var ms in mss)
            {

            }


        }

        #endregion

        #region Interfaces

        public ExpDataExchangeLoadArgs GetOneLoad(int chKeyCode)
        {
            throw new NotImplementedException();
        }

        public void Push(ExpSingleRTDataArgs e)
        {
            try
            {
                var row = _expDataSet.RTDataLoadTable.FindBy_labCh_keyCode(e.ChKeyCode);
                row._lrefDateTime = e.RefreshTime;
                row._refCount++;
                row._rcvDataCount++;
                row._rtValue = e.RTValue;

                if (row._fupDateTime != null)
                {
                    row._fupDateTime = e.RefreshTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Push Data exception: {ex.Message}\nChannel Key Code is {e.ChKeyCode}");
            }
        }

        public void Push(ExpSingleRTDataArgs[] es)
        {
            foreach (var e in es)
            {
                Push(e);
            }
        }

        public void MultiplePush(ExpMulRTDataArgs em)
        {
            try
            {
                var row = _expDataSet.RTDataLoadTable.FindBy_labCh_keyCode(em.ChKeyCode);
                row._lrefDateTime = em.RefreshTime;
                row._refCount++;
                row._rcvDataCount++;
                row._rtValue = em.RTValue;

                if (row._fupDateTime != null)
                {
                    row._fupDateTime = em.RefreshTime;
                }

                // 
                // ...
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Multiple push data exception: {ex.Message}\nChannel Key Code is {em.ChKeyCode}");
            }
        }

        public void MultiplePush(ExpMulRTDataArgs[] ems)
        {
            foreach (var em in ems)
            {
                MultiplePush(em);
            }
        }

        public void MultipleRead(ExpMulRTDataArgs em)
        {
            throw new NotImplementedException();
        }

        public void MultipleRead(ExpMulRTDataArgs[] ems)
        {
            throw new NotImplementedException();
        }

        public void Read(ExpSingleRTDataArgs e)
        {
            try
            {
                var row = _expDataSet.RTDataLoadTable.FindBy_labCh_keyCode(e.ChKeyCode);
                e.RefreshTime = row._lrefDateTime;
                e.RTValue = row._rtValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read data exception:{ex.Message}\nChannel Key Code is {e.ChKeyCode}");
                throw;
            }
        }

        public void Read(ExpSingleRTDataArgs[] es)
        {
            foreach (var e in es)
            {
                Read(e);
            }
        }

        #endregion

        #endregion
    }
}
