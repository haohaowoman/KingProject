using LabMCESystem.LabElement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
namespace LabMCESystem.Servers.HS.ChannelSet
{

    partial class HS_Elements
    {
        partial class SensorsDataTable
        {
            public int SensorsCount { get { return Rows.Count; } }

            public SensorsRow ConatinsSensor(LinerSensor ls)
            {
                return FindBy_lab(ls.Label);
            }

            public void AddSensor(LinerSensor ls)
            {
                try
                {
                    var nr = NewSensorsRow();
                    nr._lab = ls.Label;
                    nr._summary = ls.Summary;
                    nr._sensorNumber = ls.SensorNumber;
                    nr._unit = ls.Unit;
                    nr._elecRangeLow = ls.ElectricSignalRange.Low;
                    nr._elecRangeHigh = ls.ElectricSignalRange.Height;
                    nr._qRangeLow = ls.Range.Low;
                    nr._qRangeHigh = ls.Range.Height;

                    AddSensorsRow(nr);
                }
                catch (ConstraintException ex)
                {

                }
            }

            public void UpdateFromSensor(LinerSensor ls)
            {
                try
                {
                    var sr = FindBy_lab(ls.Label);
                    if (sr != null)
                    {
                        sr._summary = ls.Summary;
                        sr._sensorNumber = ls.SensorNumber;
                        sr._unit = ls.Unit;
                        sr._elecRangeLow = ls.ElectricSignalRange.Low;
                        sr._elecRangeHigh = ls.ElectricSignalRange.Height;
                        sr._qRangeLow = ls.Range.Low;
                        sr._qRangeHigh = ls.Range.Height;
                    }
                    
                }
                catch (MissingPrimaryKeyException ex)
                {

                }
            }

            public void UpdateToSensor(LinerSensor ls)
            {
                try
                {
                    var sr = FindBy_lab(ls.Label);
                    if (sr != null)
                    {
                        ls.Summary = sr._summary;
                        ls.SensorNumber = sr._sensorNumber;
                        ls.Unit = sr._unit;
                        ls.ElectricSignalRange = new QRange(sr._elecRangeLow, sr._elecRangeHigh);
                        ls.Range = new QRange(sr._qRangeLow, sr._qRangeHigh);
                    }                    
                }
                catch (MissingPrimaryKeyException ex)
                {
                    
                }
            }

            public LinerSensor GetSensor(string sensorLabel)
            {
                try
                {
                    var sr = FindBy_lab(sensorLabel);
                    if (sr != null)
                    {
                        var nls = new LinerSensor(new QRange(sr._elecRangeLow, sr._elecRangeHigh), new QRange(sr._qRangeLow, sr._qRangeHigh));
                        nls.Label = sr._lab;
                        nls.Summary = sr._summary;
                        nls.SensorNumber = sr._sensorNumber;
                        nls.Unit = sr._unit;
                        return nls;
                    }
                }
                catch (MissingPrimaryKeyException ex)
                {
                    return null;
                }
                return null;
            }

            public void DeleteSensor(LinerSensor ls)
            {
                var sr = FindBy_lab(ls.Label);
                if (sr != null)
                {
                    RemoveSensorsRow(sr);
                }
            }
        }

        partial class ChannelsDataTable
        {
            public int ChannelsCount { get { return Rows.Count; } }

            public void AddChannel(Channel ch)
            {
                try
                {
                    var nr = NewChannelsRow();
                    nr._lab = ch.Label;
                    nr._prompt = ch.Prompt;
                    nr._summary = ch.Summary;
                    nr._type = ch.Style.ToString();
                    IAnalogueMeasure am = ch as IAnalogueMeasure;
                    if (am != null)
                    {
                        LinerSensor ls = am.Collector as LinerSensor;
                        if (ls != null)
                        {
                            nr._sensor = ls.Label;
                            var st = ChildRelations["Channels_Sensors"].ChildTable as SensorsDataTable;
                            if (st.ConatinsSensor(ls) == null)
                            {
                                st.AddSensor(ls);
                            }
                        }
                    }
                    AddChannelsRow(nr);
                }
                catch (System.Exception e)
                {

                    throw;
                }
            }

            public void AddChannels(IEnumerable<Channel> chs)
            {
                try
                {
                    foreach (var ch in chs)
                    {
                        AddChannel(ch);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            public void UpdateFromChannel(Channel ch)
            {
                try
                {
                    var cr = FindBy_lab(ch.Label);
                    if (cr != null)
                    {
                        cr._prompt = ch.Prompt;
                        cr._summary = ch.Summary;
                        cr._type = ch.Style.ToString();
                        IAnalogueMeasure am = ch as IAnalogueMeasure;
                        if (am != null)
                        {
                            LinerSensor ls = am.Collector as LinerSensor;
                            if (ls != null)
                            {
                                cr._sensor = ls.Label;
                                var st = ChildRelations["Channels_Sensors"].ChildTable as SensorsDataTable;
                                if (st.ConatinsSensor(ls) != null)
                                {
                                    st.UpdateFromSensor(ls);
                                }
                                else
                                {
                                    st.AddSensor(ls);
                                }

                            }
                        }
                    }
                }
                catch (System.Data.MissingPrimaryKeyException ex)
                {

                }
            }

            public void UpdateFromChannels(IEnumerable<Channel> chs)
            {
                try
                {
                    foreach (var ch in chs)
                    {
                        UpdateFromChannel(ch);
                    }
                }
                catch (System.Exception e)
                {

                    throw;
                }
            }

            public void UpdateToChannel(Channel ch)
            {
                try
                {
                    var cr = FindBy_lab(ch.Label);
                    if (cr != null)
                    {
                        ch.Prompt = cr._prompt;
                        ch.Summary = cr._summary;

                        IAnalogueMeasure am = ch as IAnalogueMeasure;
                        if (am != null)
                        {
                            LinerSensor ls = am.Collector as LinerSensor;
                            if (ls != null)
                            {
                                var st = ChildRelations["Channels_Sensors"].ChildTable as SensorsDataTable;
                                if (st.ConatinsSensor(ls) != null)
                                {
                                    st.UpdateToSensor(ls);
                                }
                            }
                            else if (cr._sensor != null && cr._sensor != string.Empty)
                            {
                                var st = ChildRelations["Channels_Sensors"].ChildTable as SensorsDataTable;
                                am.Collector = st.GetSensor(cr._sensor);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            public void UpdateToChannels(IEnumerable<Channel> chs)
            {
                foreach (var ch in chs)
                {
                    UpdateToChannel(ch);
                }
            }

            public AnalogueMeasureChannel CreateAMeasureChannel(LabDevice dev, string label)
            {
                var fr = FindBy_lab(label);
                if (fr != null)
                {
                    var nch = dev.CreateAIChannelIn(label);
                    if (nch != null)
                    {
                        UpdateToChannel(nch);
                        return nch;
                    }
                }
                return null;
            }

            public void DeleteChannel(Channel ch)
            {
                var cr = FindBy_lab(ch.Label);
                if (cr != null)
                {
                    RemoveChannelsRow(cr);
                }
            }
        }
    }
}
